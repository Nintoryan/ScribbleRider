using UnityEngine;

namespace XDPaint.Core
{
	public interface IRenderTextureHelper
	{
		RenderTexture PaintTexture { get; }
		RenderTexture CombinedTexture { get; }
		RenderTexture PaintLine { get; }

		void Init(int width, int height);
		void ReleaseTextures();
	}
}