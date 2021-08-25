﻿using UnityEngine;

namespace XDPaint.Core
{
	public interface IRenderComponentsHelper
	{
		ObjectComponentType ComponentType { get; }
		Component PaintComponent { get; }
		Component RendererComponent { get; }
		Material Material { get; set; }

		void Init(GameObject gameObject, out ObjectComponentType componentType);
		bool IsMesh();
		void SetSourceMaterial(Material material, int index);
		Texture GetSourceTexture(Material material, string shaderTextureName, int width, int height);
		Mesh GetMesh();
		int GetMaterialIndex(Material material);
	}
}