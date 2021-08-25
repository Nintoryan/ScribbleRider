using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Tools.Image.Base;

namespace XDPaint.Tools.Image
{
	public class EyedropperTool : BasePaintTool
	{
		public override PaintTool Type
		{
			get { return PaintTool.Eyedropper; }
		}

		public override bool RenderToLineTexture
		{
			get { return false; }
		}

		public override bool ShowPreview
		{
			get { return false; }
		}
		
		private Material _material;
		private RenderTexture _brushTexture;
		private Mesh _quadMesh;
		private CommandBuffer _commandBuffer;
		private RenderTargetIdentifier _brushRti;
		private const string MainTexParam = "_MainTex";
		private const string BrushTexParam = "_BrushTex";
		private const string BrushOffsetShaderParam = "_BrushOffset";

		public override void Enter()
		{
			base.Enter();
			RenderToPaintTexture = false;
			_commandBuffer = new CommandBuffer {name = "EyedropperToolBuffer"};
			InitMaterial();
			InitQuadMesh();
		}

		public override void UpdatePress(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			base.UpdatePress(sender, uv, paintPosition, pressure);

			var activePainters = PaintController.Instance.ActivePaintManagers();
			var paintManager = activePainters.First(x => x.PaintObject == sender);
			var brushOffset = GetPreviewVector(paintManager, paintPosition, pressure);
			_material.SetTexture(MainTexParam, paintManager.GetResultRenderTexture());
			_material.SetVector(BrushOffsetShaderParam, brushOffset);
			UpdateRenderTexture();
			Render(paintManager);
		}

		private Vector4 GetPreviewVector(PaintManager paintManager, Vector2 paintPosition, float pressure)
		{
			var brushRatio = new Vector2(
				paintManager.Material.SourceTexture.width,
				paintManager.Material.SourceTexture.height) / PaintController.Instance.Brush.Size / pressure;
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
				_material = new Material(Settings.Instance.EyedropperShader);
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
		/// Renders pixel to RenderTexture and set a new brush color
		/// </summary>
		/// <param name="paintManager"></param>
		private void Render(PaintManager paintManager)
		{
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_brushRti);
			_commandBuffer.ClearRenderTarget(false, true, Constants.ClearBlack);
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, _material);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
			
			var previousRenderTexture = RenderTexture.active;
			var texture2D = new Texture2D(_brushTexture.width, _brushTexture.height, TextureFormat.ARGB32, false);
			RenderTexture.active = _brushTexture;
			texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0, false);
			RenderTexture.active = previousRenderTexture;

			var pixelColor = texture2D.GetPixel(0, 0);
			PaintController.Instance.Brush.SetColor(pixelColor);
		}

		/// <summary>
		/// Creates 1x1 texture
		/// </summary>
		private void UpdateRenderTexture()
		{
			if (_brushTexture != null)
				return;
			_brushTexture = CreateRenderTexture(1, 1);
			_material.SetTexture(BrushTexParam, _brushTexture);
			_brushRti = new RenderTargetIdentifier(_brushTexture);
		}
	}
}