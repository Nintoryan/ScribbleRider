using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.Materials;
using XDPaint.Tools.Image.Base;

namespace XDPaint.Tools.Image
{
    public class CloneTool : BasePaintTool
    {
	    public override PaintTool Type
		{
			get { return PaintTool.Clone; }
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
		    get { return _renderToLine; }
	    }

	    public bool CopyTextureOnPressDown = true;

	    private CloneData[] _cloneData;
	    private Material _brushMaterial;
	    private Texture _previousBrushTexture;
	    private RenderTexture _brushTexture;
	    private CommandBuffer _commandBuffer;
	    private Mesh _quadMesh;
		private RenderTargetIdentifier _brushRti;
		private bool _brushTextureRendered;
		private bool _renderToLine;
		private bool _preview;

		private const int PaddingPixels = 2;
		private const string BrushMainTexParam = "_MainTex";
		private const string BrushTexParam = "_BrushTex";
		private const string BrushOffsetShaderParam = "_BrushOffset";
		private const string BrushHardnessParam = "_Hardness";

		public override void Enter()
		{
			RenderToLineTexture = false;
			_preview = PaintController.Instance.Preview;
			base.Enter();
			var activePainters = PaintController.Instance.ActivePaintManagers();
			_cloneData = new CloneData[activePainters.Length];
			for (var i = 0; i < activePainters.Length; i++)
			{
				var paintManager = activePainters[i];
				paintManager.Render();
				_cloneData[i] = new CloneData();
				_cloneData[i].Enter(paintManager);
			}
			_commandBuffer = new CommandBuffer {name = "CloneToolBuffer"};
			InitBrushMaterial();
			InitQuadMesh();
			_brushTextureRendered = false;
			_previousBrushTexture = PaintController.Instance.Brush.SourceTexture;
			
			PaintController.Instance.Brush.SetColor(new Color(1, 1, 1, PaintController.Instance.Brush.Color.a), false, false); 
			PaintController.Instance.Brush.SetTexture(Settings.Instance.DefaultBrush, true, false);
			//set new preview texture
			foreach (var paintManager in activePainters)
			{
				paintManager.Material.Material.SetTexture(Paint.BrushTextureShaderParam, Settings.Instance.DefaultCircleBrush);
			}
		}

		public override void Exit()
		{
			base.Exit();
			foreach (var cloneData in _cloneData)
			{
				cloneData.Exit();
			}
			_cloneData = null;
			if (_brushTexture != null)
			{
				RenderTexture.ReleaseTemporary(_brushTexture);
			}
			if (_brushTextureRendered)
			{
				PaintController.Instance.Brush.SetTexture(_previousBrushTexture, true, false);
			}
			if (_quadMesh != null)
			{
				Object.Destroy(_quadMesh);
				_quadMesh = null;
			}
			if (_commandBuffer != null)
			{
				_commandBuffer.Release();
				_commandBuffer = null;
			}
		}

		public override void UpdateHover(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			base.UpdateHover(sender, uv, paintPosition, pressure);
			var cloneData = GetCloneData(sender);
			if (cloneData.ClickCount > 1 && PaintController.Instance.Preview && (cloneData.PrevPaintPosition != paintPosition || cloneData.PrevUV != uv || cloneData.PrevPressure != pressure))
			{
				//render new brush
				var paintOffset = paintPosition - cloneData.PaintPosition;
				var brushOffset = GetPreviewVector(cloneData.PaintManager, paintOffset, pressure);
				_brushMaterial.SetVector(BrushOffsetShaderParam, brushOffset);
				var width = PaintController.Instance.Brush.RenderTexture.width;
				var height = PaintController.Instance.Brush.RenderTexture.height;
				UpdateBrushRenderTexture(cloneData.PaintManager, width, height);
				
				_preview = false;
				RenderBrush(cloneData.PaintManager);
				_preview = true;

				cloneData.PrevUV = uv;
				cloneData.PrevPaintPosition = paintPosition;
				cloneData.PrevPressure = pressure;
			}
		}

		public override void UpdateDown(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			base.UpdateDown(sender, uv, paintPosition, pressure);
			_preview = true;
		}

		public override void UpdatePress(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			base.UpdatePress(sender, uv, paintPosition, pressure);
			var cloneData = GetCloneData(sender);
			if (cloneData.IsUp)
			{
				if (CopyTextureOnPressDown)
				{
					_preview = false;
					cloneData.PaintManager.Render();
					_preview = true;
					Graphics.Blit(cloneData.PaintManager.GetResultRenderTexture(), cloneData.CopiedTexture);
				}
				if (cloneData.ClickCount == 1)
				{
					cloneData.UVSecond = uv;
					cloneData.PaintPositionSecond = paintPosition;
					cloneData.UV = cloneData.UVFirst - cloneData.UVSecond;
					cloneData.PaintPosition = cloneData.PaintPositionSecond - cloneData.PaintPositionFirst;
				}
				cloneData.IsUp = false;
			}

			if (cloneData.IsFirstClick)
			{
				//render new brush
				var brushOffset = GetPreviewVector(cloneData.PaintManager, paintPosition, pressure);
				_brushMaterial.SetVector(BrushOffsetShaderParam, brushOffset);
				var width = PaintController.Instance.Brush.RenderTexture.width;
				var height = PaintController.Instance.Brush.RenderTexture.height;
				UpdateBrushRenderTexture(cloneData.PaintManager, width, height);
				RenderBrush(cloneData.PaintManager);

				cloneData.PaintPositionFirst = paintPosition;
				cloneData.UVFirst = uv;
				return;
			}

			UpdateRenderTexture(cloneData);
			cloneData.UpdateMaterial(cloneData.UV);
			Render(cloneData);
		}

		public override void UpdateUp(object sender, bool inBounds)
		{
			base.UpdateUp(sender, inBounds);
			if (inBounds)
			{
				var cloneData = GetCloneData(sender);
				_renderToLine = true;
				cloneData.IsUp = true;
				_preview = false;
				cloneData.IsFirstClick = false;
				cloneData.ClickCount++;
			}
		}

		#region Initialization

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
		
		private void InitBrushMaterial()
		{
			if (_brushMaterial == null)
			{
				_brushMaterial = new Material(Settings.Instance.BrushSamplerShader);
			}
			_brushMaterial.SetFloat(BrushHardnessParam, PaintController.Instance.Brush.Hardness);
		}
		
		private void UpdateBrushRenderTexture(PaintManager paintManager, int width, int height)
		{
			if (_brushMaterial.GetTexture(BrushMainTexParam) != paintManager.GetResultRenderTexture())
			{
				_brushMaterial.SetTexture(BrushMainTexParam, paintManager.GetResultRenderTexture());
			}

			if (_brushTexture != null && _brushTexture.width == width + PaddingPixels && _brushTexture.height == height + PaddingPixels)
				return;
			
			_brushTexture = CreateRenderTexture(width + PaddingPixels, height + PaddingPixels);
			_brushMaterial.SetTexture(BrushTexParam, _brushTexture);
			_brushRti = new RenderTargetIdentifier(_brushTexture);
			
		}
		private void UpdateRenderTexture(CloneData cloneData)
		{
			var renderTexture = cloneData.PaintManager.GetResultRenderTexture();
			if (cloneData.RenderTexture != null && cloneData.RenderTexture.width - PaddingPixels == renderTexture.width && cloneData.RenderTexture.height - PaddingPixels == renderTexture.height) 
				return;
			
			_preview = false;
			cloneData.PaintManager.Render();
			renderTexture = cloneData.PaintManager.GetResultRenderTexture(); 
			_preview = true;
			cloneData.RenderTexture = CreateRenderTexture(renderTexture.width + PaddingPixels, renderTexture.height + PaddingPixels);
			cloneData.CopiedTexture = CreateRenderTexture(renderTexture.width + PaddingPixels, renderTexture.height + PaddingPixels);
			Graphics.Blit(renderTexture, cloneData.CopiedTexture);
			cloneData.InitRenderTargetIdentifier();
		}

		private RenderTexture CreateRenderTexture(int width, int height)
		{
			var renderTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
			if (!renderTexture.IsCreated())
			{			
				renderTexture.filterMode = FilterMode.Point;
                renderTexture.autoGenerateMips = false;
                renderTexture.useMipMap = false;
			}
			return renderTexture;
		}
		
		#endregion
		
		private CloneData GetCloneData(object sender)
		{
			return _cloneData.First(x => x.PaintManager.PaintObject == sender);
		}

		private Vector4 GetPreviewVector(PaintManager paintManager, Vector2 paintPosition, float pressure)
		{
			var brushRatio = new Vector2(
				paintManager.Material.SourceTexture.width / (float) PaintController.Instance.Brush.RenderTexture.width + PaddingPixels,
				paintManager.Material.SourceTexture.height / (float) PaintController.Instance.Brush.RenderTexture.height + PaddingPixels) / PaintController.Instance.Brush.Size / pressure;
			var brushOffset = new Vector4(
				paintPosition.x / paintManager.Material.SourceTexture.width * brushRatio.x,
				paintPosition.y / paintManager.Material.SourceTexture.height * brushRatio.y,
				1f / brushRatio.x, 1f / brushRatio.y);
			return brushOffset;
		}
		
		private void RenderBrush(PaintManager paintManager)
		{
			_preview = false;
			paintManager.Render();
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_brushRti);
			_commandBuffer.ClearRenderTarget(false, true, Constants.ClearBlack);
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, _brushMaterial);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
			PaintController.Instance.Brush.SetTexture(_brushTexture, true, false);
			_brushTextureRendered = true;
			_preview = true;
		}

		private void Render(CloneData cloneData)
		{
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(cloneData.RenderTargetIdentifier);
			_commandBuffer.ClearRenderTarget(false, true, Constants.ClearBlack);
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, cloneData.CloneMaterial);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
			DrawPostProcess = true;
			cloneData.DrawPostProcess = true;
		}
		
		public override void OnDrawPostProcess(object sender, CommandBuffer commandBuffer, RenderTargetIdentifier rti, Material material)
		{
			base.OnDrawPostProcess(sender, commandBuffer, rti, material);
			var cloneData = GetCloneData(sender);
			if (!cloneData.DrawPostProcess)
			{
				DrawPostProcess = true;
				return;
			}
			cloneData.DrawPostProcess = false;
			
			//copy params
			var mainTex = material.GetTexture(Brush.MainTextureShaderParam);
			var maskTex = material.GetTexture(Paint.PaintTextureShaderParam);

			//set new params
			material.SetTexture(Brush.MainTextureShaderParam, cloneData.PaintManager.GetPaintTexture());
			material.SetTexture(Paint.PaintTextureShaderParam, cloneData.RenderTexture);
	
			//render
			GL.LoadOrtho();
			commandBuffer.Clear();
			commandBuffer.SetRenderTarget(rti);
			commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, material, 0, 1);
			Graphics.ExecuteCommandBuffer(commandBuffer);
			
			//restore params
			material.SetTexture(Brush.MainTextureShaderParam, mainTex);
			material.SetTexture(Paint.PaintTextureShaderParam, maskTex);
		}
		
	    private class CloneData
	    {
		    public PaintManager PaintManager;
		    public RenderTexture CopiedTexture;
		    public RenderTexture RenderTexture;
		    public Material CloneMaterial;
		    public RenderTargetIdentifier RenderTargetIdentifier;
		    public Vector2 UVFirst;
		    public Vector2 UVSecond;
		    public Vector2 UV;
		    public Vector2 PaintPositionFirst;
		    public Vector2 PaintPositionSecond;
		    public Vector2 PaintPosition;
		    public Vector2 PrevUV = -Vector2.one;
		    public Vector2 PrevPaintPosition = -Vector2.one;
		    public float PrevPressure = -1f;
		    public int ClickCount;
		    public bool IsUp;
		    public bool IsFirstClick = true;
		    public bool DrawPostProcess;
		
		    private const string MainTexParam = "_MainTex";
		    private const string MaskTexParam = "_MaskTex";
		    private const string OffsetParam = "_Offset";

		    public void Enter(PaintManager paintManager)
		    {
			    PaintManager = paintManager;
			    InitMaterial();
		    }
		
		    public void Exit()
		    {
			    ClickCount = 0;
			    IsFirstClick = true;
			    IsUp = false;
			    if (CopiedTexture != null)
			    {
				    RenderTexture.ReleaseTemporary(CopiedTexture);
			    }
			    if (RenderTexture != null)
			    {
				    RenderTexture.ReleaseTemporary(RenderTexture);
			    }
			    if (CloneMaterial != null)
			    {
				    Object.Destroy(CloneMaterial);
			    }
		    }
		
		    private void InitMaterial()
		    {
			    if (CloneMaterial == null)
			    {
				    CloneMaterial = new Material(Settings.Instance.BrushCloneShader);
			    }
			    CloneMaterial.SetTexture(MainTexParam, PaintManager.GetResultRenderTexture());
			    CloneMaterial.SetTexture(MaskTexParam, PaintManager.GetRenderTextureLine());
		    }

		    public void InitRenderTargetIdentifier()
		    {
			    RenderTargetIdentifier = new RenderTargetIdentifier(RenderTexture);
		    }

		    public void UpdateMaterial(Vector2 uv)
		    {
			    CloneMaterial.SetVector(OffsetParam, uv);
			    CloneMaterial.SetTexture(MainTexParam, CopiedTexture);
		    }
	    }
    }
}