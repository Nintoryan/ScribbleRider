namespace XDPaint.Core
{
	public enum ObjectComponentType
	{
		Unknown,
		RawImage,
		MeshFilter,
		SkinnedMeshRenderer,
		SpriteRenderer
	}
	
	public enum PaintTool
	{
		Brush,
		Erase,
		Eyedropper,
		BrushSampler,
		Clone,
		Blur,
		BlurGaussian
	}
}