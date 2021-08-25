using System.Collections.Generic;
using UnityEngine;
using XDPaint.Core;
using XDPaint.Tools.Image;
using XDPaint.Tools.Image.Base;

namespace XDPaint.Tools
{
	public class ToolsManager
	{
		public BasePaintTool CurrentTool
		{
			get { return _currentTool; }
		}

		public readonly KeyValuePair<PaintTool, BasePaintTool>[] AllTools;
		private BasePaintTool _currentTool;

		public ToolsManager()
		{
			AllTools = new[]
			{
				new KeyValuePair<PaintTool, BasePaintTool>(PaintTool.Brush, new BasePaintTool()),
				new KeyValuePair<PaintTool, BasePaintTool>(PaintTool.BrushSampler, new BrushSamplerTool()),
				new KeyValuePair<PaintTool, BasePaintTool>(PaintTool.Erase, new EraseTool()),
				new KeyValuePair<PaintTool, BasePaintTool>(PaintTool.Eyedropper, new EyedropperTool()),
				new KeyValuePair<PaintTool, BasePaintTool>(PaintTool.Clone, new CloneTool()),
				new KeyValuePair<PaintTool, BasePaintTool>(PaintTool.Blur, new BlurTool()),
				new KeyValuePair<PaintTool, BasePaintTool>(PaintTool.BlurGaussian, new GaussianBlurTool()),
			};
			_currentTool = AllTools[0].Value;
			_currentTool.Enter();
		}

		public void Init(PaintManager paintManager)
		{
			paintManager.PaintObject.OnPaintHandler -= Paint;
			paintManager.PaintObject.OnPaintHandler += Paint;
			paintManager.PaintObject.OnMouseHoverHandler -= OnMouseHover;
			paintManager.PaintObject.OnMouseHoverHandler += OnMouseHover;
			paintManager.PaintObject.OnMouseDownHandler -= OnMouseDown;
			paintManager.PaintObject.OnMouseDownHandler += OnMouseDown;
			paintManager.PaintObject.OnMouseHandler -= OnMouse;
			paintManager.PaintObject.OnMouseHandler += OnMouse;
			paintManager.PaintObject.OnMouseUpHandler -= OnMouseUp;
			paintManager.PaintObject.OnMouseUpHandler += OnMouseUp;
			paintManager.PaintObject.OnUndoHandler -= OnUndo;
			paintManager.PaintObject.OnUndoHandler += OnUndo;
			paintManager.PaintObject.OnRedoHandler -= OnRedo;
			paintManager.PaintObject.OnRedoHandler += OnRedo;
		}
		
		public void SetTool(PaintTool newTool)
		{
			foreach (var tool in AllTools)
			{
				if (tool.Key == newTool)
				{
					_currentTool.Exit();
					_currentTool = tool.Value;
					_currentTool.Enter();
					break;
				}
			}
		}

		private void Paint(object sender, Vector2 paintPosition, float pressure)
		{			
			_currentTool.OnPaint(sender, paintPosition, pressure);
		}
		
		private void OnMouseHover(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			_currentTool.UpdateHover(sender, uv, paintPosition, pressure);
		}

		private void OnMouseDown(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			_currentTool.UpdateDown(sender, uv, paintPosition, pressure);
		}
		
		private void OnMouse(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
		{
			_currentTool.UpdatePress(sender, uv, paintPosition, pressure);
		}
		
		private void OnMouseUp(object sender, bool inBounds)
		{
			_currentTool.UpdateUp(sender, inBounds);
		}
		
		private void OnUndo(object sender)
		{
			_currentTool.OnUndo(sender);
		}
		
		private void OnRedo(object sender)
		{
			_currentTool.OnRedo(sender);
		}
	}
}