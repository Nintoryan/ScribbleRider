using XDPaint.Core;
using XDPaint.Tools.Image.Base;

namespace XDPaint.Tools.Image
{
	public class EraseTool : BasePaintTool
	{
		public override PaintTool Type
		{
			get { return PaintTool.Erase; }
		}
	
		public override bool DrawPreview
		{
			get { return true; }
		}
	}
}