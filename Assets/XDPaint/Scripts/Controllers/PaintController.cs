using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XDPaint.Core;
using XDPaint.Core.Materials;
using XDPaint.Tools;

namespace XDPaint.Controllers
{
	public class PaintController : Singleton<PaintController>
	{
		private ToolsManager _toolsManager;
		public ToolsManager ToolsManager
		{
			get { return _toolsManager; }
		}

		[SerializeField] private PaintTool paintTool;
		public PaintTool Tool
		{
			get { return _toolsManager.CurrentTool.Type; }
			set
			{
				paintTool = value;
				if (_initialized)
				{
					brush.SetPaintTool(value);
					foreach (var paintManager in _allPaintManagers)
					{
						paintManager.Material.SetPreviewTexture(brush.RenderTexture);
					}
					if (_toolsManager != null)
					{
						_toolsManager.SetTool(value);
					}
				}
			}
		}

		[SerializeField] private bool preview = true;
		public bool Preview
		{
			get { return preview; }
			set
			{
				if (_initialized && preview != value)
				{
					preview = value;
					foreach (var paintManager in _allPaintManagers)
					{
						paintManager.UpdatePreviewInput();
					}
				}
			}
		}
        
		[SerializeField] private Brush brush = new Brush();
		public Brush Brush
		{
			get { return brush; }
		}
		
		private List<PaintManager> _allPaintManagers = new List<PaintManager>();
		private bool _initialized;

		public void Init(PaintManager paintManager)
		{
			if (!_initialized)
			{
				_toolsManager = new ToolsManager();
				brush.Init();
				brush.SetPaintTool(paintTool);
				_initialized = true;
			}
			_allPaintManagers.Add(paintManager);
			_toolsManager.Init(paintManager);
			paintManager.Material.SetPreviewTexture(Brush.RenderTexture);
			brush.OnChangeTexture += paintManager.Material.SetPreviewTexture;
		}

		private void OnDestroy()
		{
			brush.Destroy();
		}

		public PaintManager[] ActivePaintManagers()
		{
			return _allPaintManagers.Where(paintManager => paintManager != null && paintManager.gameObject.activeInHierarchy && paintManager.enabled && paintManager.Initialized).ToArray();
		}

		public PaintManager[] AllPaintManagers()
		{
			return _allPaintManagers.Where(paintManager => paintManager != null).ToArray();
		}
	}
}