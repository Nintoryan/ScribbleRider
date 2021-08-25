using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.Materials;
using XDPaint.Tools.Image.Base;
using Object = UnityEngine.Object;

namespace XDPaint.Tools.Image
{
	[Serializable]
    public class GaussianBlurTool : BasePaintTool
    {
	    public override PaintTool Type
        {
            get { return PaintTool.BlurGaussian; }
        }

        public override bool ShowPreview
        {
	        get { return false; }
        }
        
        public override bool RenderToPaintTexture
        {
	        get { return false; }
        }
        
        public override bool RenderToLineTexture
        {
	        get { return true; }
        }

        #region Blur settings

        public int KernelSize = 3;
        public float Spread = 5f;

        #endregion

        private BlurData[] _blurData;
		private Mesh _quadMesh;
		private CommandBuffer _commandBuffer;
		private const string MainTexParam = "_MainTex";
		private const string MaskTexParam = "_MaskTex";
		private const string KernelSizeParam = "_KernelSize";
		private const string SpreadParam = "_Spread";

		public override void Enter()
		{
			base.Enter();
			var activePainters = PaintController.Instance.ActivePaintManagers();
			_blurData = new BlurData[activePainters.Length];
			for (var i = 0; i < activePainters.Length; i++)
			{
				var paintManager = activePainters[i];
				paintManager.Render();
				_blurData[i] = new BlurData();
				_blurData[i].Enter(paintManager);
			}
			_commandBuffer = new CommandBuffer {name = "GaussianBlurToolBuffer"};
			InitQuadMesh();
		}

		public override void Exit()
		{
			base.Exit();
			foreach (var blurData in _blurData)
			{
				blurData.Exit();
			}
			_blurData = null;
			if (_commandBuffer != null)
			{
				_commandBuffer.Release();
			}
			if (_quadMesh != null)
			{
				Object.Destroy(_quadMesh);
			}
			_quadMesh = null;
		}

		public override void OnPaint(object sender, Vector2 paintPosition, float pressure)
		{
			base.OnPaint(sender, paintPosition, pressure);
			var blurData = GetBlurData(sender);
			UpdateRenderTextures(blurData);
			Render(blurData);
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

		private void UpdateRenderTextures(BlurData blurData)
		{
			if (blurData.PaintManager == null || !blurData.PaintManager.Initialized || blurData.BlurredTexture != null)
				return;
			
			var textureWidth = blurData.PaintManager.Material.Material.mainTexture.width;
			var textureHeight = blurData.PaintManager.Material.Material.mainTexture.height;
			blurData.BlurredTexture = CreateRenderTexture(textureWidth, textureHeight);
			blurData.PreBlurredTexture = CreateRenderTexture(textureWidth, textureHeight);
			blurData.BlurRenderTargetIdentifier = new RenderTargetIdentifier(blurData.BlurredTexture);
			blurData.PreBlurRenderTargetIdentifier = new RenderTargetIdentifier(blurData.PreBlurredTexture);
			blurData.MaskMaterial.SetTexture(MainTexParam, blurData.PreBlurredTexture);
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

		private BlurData GetBlurData(object sender)
		{
			return _blurData.First(x => x.PaintManager.PaintObject == sender);
		}

		private void Blur(Material blurMaterial, RenderTexture source, RenderTexture destination)
		{
			if (blurMaterial != null)
			{
				blurMaterial.SetFloat(KernelSizeParam, KernelSize);
				blurMaterial.SetFloat(SpreadParam, Spread);
				Graphics.Blit(source, destination, blurMaterial, 0);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}

		private void Render(BlurData blurData)
		{
			blurData.MaskMaterial.mainTexture = blurData.PaintManager.GetResultRenderTexture();
			
			//clear render texture
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(blurData.PreBlurRenderTargetIdentifier);
			_commandBuffer.ClearRenderTarget(false, true, Constants.ClearBlack);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
			
			//set new texture
			blurData.MaskMaterial.mainTexture = blurData.PreBlurredTexture;

			//blur
			var resultRenderTexture = blurData.PaintManager.GetResultRenderTexture();
			var renderTexture = RenderTexture.GetTemporary(resultRenderTexture.width, resultRenderTexture.height, 0, resultRenderTexture.format);
			Graphics.Blit(resultRenderTexture, renderTexture);
			Blur(blurData.BlurMaterial, renderTexture, blurData.PreBlurredTexture);
			RenderTexture.ReleaseTemporary(renderTexture);
			
			//render with mask
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(blurData.BlurRenderTargetIdentifier);
			_commandBuffer.ClearRenderTarget(false, true, Constants.ClearBlack);
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, blurData.MaskMaterial);
			Graphics.ExecuteCommandBuffer(_commandBuffer);

			DrawPostProcess = true;
			blurData.DrawPostProcess = true;
		}

		public override void OnDrawPostProcess(object sender, CommandBuffer commandBuffer, RenderTargetIdentifier rti, Material material)
		{
			base.OnDrawPostProcess(sender, commandBuffer, rti, material);
			var blurData = GetBlurData(sender);
			if (!blurData.DrawPostProcess)
			{
				DrawPostProcess = true;
				return;
			}
			blurData.DrawPostProcess = false;
			
			//copy params
			var mainTex = material.GetTexture(Brush.MainTextureShaderParam);
			var maskTex = material.GetTexture(Paint.PaintTextureShaderParam);
			
			//set new params
			material.SetTexture(Brush.MainTextureShaderParam, blurData.PaintManager.GetResultRenderTexture());
			material.SetTexture(Paint.PaintTextureShaderParam, blurData.BlurredTexture);
			
			//render
			GL.LoadOrtho();
			commandBuffer.Clear();
			commandBuffer.SetRenderTarget(rti);
			commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, material, 0, 0);
			commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, material, 0, 1);
			Graphics.ExecuteCommandBuffer(commandBuffer);
			
			//restore params
			material.SetTexture(Brush.MainTextureShaderParam, mainTex);
			material.SetTexture(Paint.PaintTextureShaderParam, maskTex);
		}
		
		public class BlurData
		{
			public PaintManager PaintManager;
			public Material BlurMaterial;
			public Material MaskMaterial;
			public RenderTexture PreBlurredTexture;
			public RenderTexture BlurredTexture;
			public RenderTargetIdentifier BlurRenderTargetIdentifier;
			public RenderTargetIdentifier PreBlurRenderTargetIdentifier;
			public bool DrawPostProcess;
			private const string MainTexParam = "_MainTex";
			private const string MaskTexParam = "_MaskTex";
		
			public void Enter(PaintManager paintManager)
			{
				PaintManager = paintManager;
				InitMaterial();
			}
		
			public void Exit()
			{
				if (PreBlurredTexture != null)
				{
					RenderTexture.ReleaseTemporary(PreBlurredTexture);
				}
				if (BlurredTexture != null)
				{
					RenderTexture.ReleaseTemporary(BlurredTexture);
				}
				if (BlurMaterial != null)
				{
					Object.Destroy(BlurMaterial);
				}
				BlurMaterial = null;
				if (MaskMaterial != null)
				{
					Object.Destroy(MaskMaterial);
				}
				MaskMaterial = null;
			}
		
			private void InitMaterial()
			{
				if (MaskMaterial == null)
				{
					MaskMaterial = new Material(Settings.Instance.BrushBlurShader);
				}
				MaskMaterial.SetTexture(MainTexParam, PreBlurredTexture);
				MaskMaterial.SetTexture(MaskTexParam, PaintManager.GetRenderTextureLine());

				if (BlurMaterial == null)
				{
					BlurMaterial = new Material(Settings.Instance.GaussianBlurShader) {mainTexture = BlurredTexture};
				}
			}
		}
    }
}