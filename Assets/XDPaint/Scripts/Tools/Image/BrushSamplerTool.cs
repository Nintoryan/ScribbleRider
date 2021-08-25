using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.Materials;
using XDPaint.Tools.Image.Base;

namespace XDPaint.Tools.Image
{
	public class BrushSamplerTool : BasePaintTool
	{
		public override PaintTool Type
		{
			get { return PaintTool.BrushSampler; }
		}

		public override bool ShowPreview
		{
			get { return _preview; }
		}

		public override bool RenderToPaintTexture
		{
			get { return false; }
		}
        
		public override bool RenderToLineTexture
		{
			get { return false; }
		}

		private Material _material;
		private RenderTexture _brushTexture;
		private Mesh _quadMesh;
		private CommandBuffer _commandBuffer;
		private RenderTargetIdentifier _brushRti;
		private bool _preview;
		private bool _shouldSetBrushTextureParam;
		private const string MainTexParam = "_MainTex";
		private const string BrushTexParam = "_BrushTex";
		private const string BrushOffsetShaderParam = "_BrushOffset";

		public override void Enter()
		{
			_preview = PaintController.Instance.Preview;
			base.Enter();
			_commandBuffer = new CommandBuffer {name = "BrushSamplerToolBuffer"};
			InitMaterial();
			InitQuadMesh();

			PaintController.Instance.Brush.SetColor(new Color(1, 1, 1, PaintController.Instance.Brush.Color.a), false, false); 
			PaintController.Instance.Brush.SetTexture(Settings.Instance.DefaultBrush, true, false);
			//set new preview texture
			var activePainters = PaintController.Instance.ActivePaintManagers();
			foreach (var paintManager in activePainters)
			{
				paintManager.Material.Material.SetTexture(Paint.BrushTextureShaderParam, Settings.Instance.DefaultCircleBrush);
			}
		}

		public override void Exit()
		{
			base.Exit();
			if (_brushTexture != null && _brushTexture.IsCreated())
			{
				if (RenderTexture.active == _brushTexture)
				{
					RenderTexture.active = null;
				}
				//Don't destroy texture because it can be used later
				// _brushTexture.Release();
				// Object.Destroy(_brushTexture);
			}
			if (_material != null)
			{
				Object.Destroy(_material);
				_material = null;
			}
			if (_commandBuffer != null)
			{
				_commandBuffer.Release();
			}
			if (_quadMesh != null)
			{
				Object.Destroy(_quadMesh);
				_quadMesh = null;
			}
		}

		public override void UpdatePress(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			base.UpdatePress(sender, uv, paintPosition, pressure);
			var activePainters = PaintController.Instance.ActivePaintManagers();
			var paintManager = activePainters.First(x => x.PaintObject == sender);
			var brushOffset = GetPreviewVector(paintManager, paintPosition, pressure);
			_material.SetVector(BrushOffsetShaderParam, brushOffset);
			Render(paintManager);
		}

		public override void UpdateDown(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			base.UpdateDown(sender, uv, paintPosition, pressure);
			var width = PaintController.Instance.Brush.RenderTexture.width;
			var height = PaintController.Instance.Brush.RenderTexture.height;
			UpdateRenderTexture(width, height);
			if (_shouldSetBrushTextureParam)
			{
				_material.SetTexture(BrushTexParam, _brushTexture);
				_shouldSetBrushTextureParam = false;
			}
		}

		private Vector4 GetPreviewVector(PaintManager paintManager, Vector2 paintPosition, float pressure)
		{
			var brushRatio = new Vector2(
				paintManager.Material.SourceTexture.width / (float) PaintController.Instance.Brush.RenderTexture.width,
				paintManager.Material.SourceTexture.height / (float) PaintController.Instance.Brush.RenderTexture.height) / PaintController.Instance.Brush.Size / pressure;
			var brushOffset = new Vector4(
				paintPosition.x / paintManager.Material.SourceTexture.width * brushRatio.x,
				paintPosition.y / paintManager.Material.SourceTexture.height * brushRatio.y,
				1f / brushRatio.x, 1f / brushRatio.y);
			return brushOffset;
		}

		private void InitMaterial()
		{
			if (_material == null)
			{
				_material = new Material(Settings.Instance.BrushSamplerShader);
				_shouldSetBrushTextureParam = true;
			}
		}
		
		private void InitQuadMesh()
		{
			if (_quadMesh == null)
			{
				_quadMesh = new Mesh
				{
					vertices = new[] {Vector3.up, new Vector3(1, 1, 0), Vector3.right, Vector3.zero},
					uv = new[] {Vector2.up, Vector2.one, Vector2.right, Vector2.zero,},
					triangles = new[] {0, 1, 2, 2, 3, 0},
					colors = new[] {Color.white, Color.white, Color.white, Color.white}
				};
			}
		}

		private RenderTexture CreateRenderTexture(int width, int height)
		{
			var renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
			{
				filterMode = FilterMode.Point,
				autoGenerateMips = false
			};
			renderTexture.Create();
			return renderTexture;
		}

		/// <summary>
		/// Renders part of Result texture into RenderTexture, set new brush
		/// </summary>
		/// <param name="paintManager"></param>
		private void Render(PaintManager paintManager)
		{
			//set preview to false
			_preview = false;
			paintManager.Render();
			
			_material.SetTexture(MainTexParam, paintManager.GetResultRenderTexture());
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_brushRti);
			_commandBuffer.ClearRenderTarget(false, true, Constants.ClearBlack);
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, _material);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
			PaintController.Instance.Brush.SetTexture(_brushTexture, true, false);
			//restore preview
			_preview = true;
		}

		/// <summary>
		/// Creates new brush texture
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		private void UpdateRenderTexture(int width, int height)
		{
			if (_brushTexture != null && _brushTexture.width == width && _brushTexture.height == height)
				return;
			_brushTexture = CreateRenderTexture(width, height);
			_brushRti = new RenderTargetIdentifier(_brushTexture);
		}
	}
}