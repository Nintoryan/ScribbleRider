using System;
using UnityEngine;
using UnityEngine.Rendering;
using XDPaint.Controllers;
using XDPaint.Tools;

namespace XDPaint.Core.Materials
{
	[Serializable]
	public class Brush
	{
		#region Properties and variables
		[SerializeField] private Material material;
		public Material Material
		{
			get { return material; }
		}

		[SerializeField] private Color color = Color.white;
		public Color Color
		{
			get { return color; }
		}
		
		[SerializeField] private Texture sourceTexture;
		public Texture SourceTexture
		{
			get { return sourceTexture; }
		}
                 
		[SerializeField] private RenderTexture renderTexture;
		public RenderTexture RenderTexture
		{
			get { return renderTexture; }
		}

		[SerializeField] private float size = 1f;
		public float Size
		{
			get { return size; }
			set
			{
				size = value;
			}
		}
		
		[SerializeField] private float hardness = 0.9f;
		public float Hardness
		{
			get { return hardness; }
			set
			{
				hardness = value;
				if (_initialized)
				{
					_renderMaterial.SetFloat(HardnessParam, hardness);
					Render();
				}
			}
		}

		public delegate void ChangeColorHandler(Color color);
		public delegate void ChangeTextureHandler(Texture texture);
		public ChangeColorHandler OnChangeColor;
		public ChangeTextureHandler OnChangeTexture;
		
		private Material _renderMaterial;
		private Mesh _quadMesh;
		private CommandBuffer _commandBuffer;
		private RenderTargetIdentifier _rti;
		private bool _initialized;

		public const string MainTextureShaderParam = "_MainTex";
		private const string SrcColorBlend = "_SrcColorBlend";
		private const string DstColorBlend = "_DstColorBlend";
		private const string SrcAlphaBlend = "_SrcAlphaBlend";
		private const string DstAlphaBlend = "_DstAlphaBlend";
		private const string BlendOpColor = "_BlendOpColor";
		private const string BlendOpAlpha = "_BlendOpAlpha";
		private const string HardnessParam = "_Hardness";
		private const int Padding = 2;
		#endregion

		public void Init()
		{
			if (OnChangeColor != null)
			{
				OnChangeColor(color);
			}
			if (OnChangeTexture != null)
			{
				OnChangeTexture(renderTexture);
			}
			if (sourceTexture == null)
			{
				sourceTexture = Settings.Instance.DefaultBrush;
			}
			_commandBuffer = new CommandBuffer {name = "XDPaintBrush"};
			InitQuadMesh();
			InitRenderTexture();
			InitMaterials();
			Render();
			_initialized = true;
		}

		public void Destroy()
		{
			if (_quadMesh != null)
			{
				UnityEngine.Object.Destroy(_quadMesh);
			}
			if (material != null)
			{
				UnityEngine.Object.Destroy(material);
			}
			if (renderTexture != null && renderTexture.IsCreated())
			{
				if (RenderTexture.active == renderTexture)
				{
					RenderTexture.active = null;
				}
				renderTexture.Release();
				UnityEngine.Object.Destroy(renderTexture);
			}
		}
		
		private void InitQuadMesh()
		{
			_quadMesh = new Mesh
			{
				vertices = new[] {Vector3.up, new Vector3(1, 1, 0), Vector3.right, Vector3.zero},
				uv = new[] {Vector2.up, Vector2.one, Vector2.right, Vector2.zero,},
				triangles = new[] {0, 1, 2, 2, 3, 0},
				colors = new[] {Color.white, Color.white, Color.white, Color.white}
			};
		}
		
		private void InitRenderTexture()
		{
			renderTexture = new RenderTexture(sourceTexture.width + Padding, sourceTexture.height + Padding, 0, RenderTextureFormat.ARGB32)
			{
				filterMode = FilterMode.Point,
				autoGenerateMips = false,
				useMipMap = false,
				anisoLevel = 0
			};
			renderTexture.Create();
			_rti = new RenderTargetIdentifier(renderTexture);
		}

		private void InitMaterials()
		{
			material = new Material(Settings.Instance.BrushShader) {color = color, mainTexture = renderTexture};
			SetBlendingOptions(PaintController.Instance.Tool);
			_renderMaterial = new Material(Settings.Instance.BrushRenderShader)
			{
				mainTexture = sourceTexture, color = color
			};
			_renderMaterial.SetFloat(HardnessParam, hardness);
		}

		private void Render()
		{
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_rti);
			_commandBuffer.ClearRenderTarget(false, true, Constants.ClearWhite);
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, _renderMaterial);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
			material.mainTexture = renderTexture;
		}

		private void SetBlendingOptions(PaintTool paintTool)
		{
			if (paintTool == PaintTool.Erase)
			{
				material.SetInt(BlendOpColor, (int) BlendOp.Add);
				material.SetInt(BlendOpAlpha, (int) BlendOp.ReverseSubtract);

				material.SetInt(SrcColorBlend, (int) BlendMode.Zero);
				material.SetInt(DstColorBlend, (int) BlendMode.One);
				material.SetInt(SrcAlphaBlend, (int) BlendMode.SrcAlpha);
				material.SetInt(DstAlphaBlend, (int) BlendMode.OneMinusSrcAlpha);
			}
			else
			{
				material.SetInt(BlendOpColor, (int) BlendOp.Add);
				material.SetInt(BlendOpAlpha, (int) BlendOp.Add);

				material.SetInt(SrcColorBlend, (int) BlendMode.SrcAlpha);
				material.SetInt(DstColorBlend, (int) BlendMode.OneMinusSrcAlpha);
				material.SetInt(SrcAlphaBlend, (int) BlendMode.SrcAlpha);
				material.SetInt(DstAlphaBlend, (int) BlendMode.One);
			}
		}

		public void SetColor(Color colorValue, bool render = true, bool sendToEvent = true)
		{
			color = colorValue;
			if (!_initialized) 
				return;
			
			material.color = color;
			_renderMaterial.color = color;
			if (render)
			{
				Render();
			}
			if (sendToEvent && OnChangeColor != null)
			{
				OnChangeColor(color);
			}
		}
		
		public void SetTexture(Texture texture, bool render = true, bool sendToEvent = true)
		{
			sourceTexture = texture;
			if (!_initialized)
				return;

			_renderMaterial.mainTexture = sourceTexture;
			material.SetTexture(MainTextureShaderParam, sourceTexture);
			if (render)
			{
				Render();
			}
			if (sendToEvent && OnChangeTexture != null)
			{
				OnChangeTexture(texture);
			}
		}

		public void SetPaintTool(PaintTool paintTool)
		{
			if (!_initialized)
				return;

			SetBlendingOptions(paintTool);
		}
	}
}