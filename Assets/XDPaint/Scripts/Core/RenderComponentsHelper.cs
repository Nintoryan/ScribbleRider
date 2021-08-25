using UnityEngine;
using UnityEngine.UI;
using XDPaint.Tools;

namespace XDPaint.Core
{
    [System.Serializable]
    public class RenderComponentsHelper : IRenderComponentsHelper
    {
        public ObjectComponentType ComponentType { get; private set; }

        public Component PaintComponent { get; private set; }
        public Component RendererComponent { get; private set; }

        private readonly Color _defaultTextureColor = Constants.ClearWhite;
        private const string MainTexProp = "_MainTex";

        public Material Material
        {
            get
            {
                if (ComponentType == ObjectComponentType.RawImage)
                {
                    return ((RawImage) RendererComponent).material;
                }
                return ((Renderer) RendererComponent).sharedMaterial;
            }
            set
            {
                var rawImage = PaintComponent as RawImage;
                if (rawImage != null)
                {
                    rawImage.material = value;
                    return;
                }

                var rendererComponent = PaintComponent as Renderer;
                if (rendererComponent != null)
                {
                    rendererComponent.sharedMaterial = value;
                }
            }
        }

        public void Init(GameObject gameObject, out ObjectComponentType componentType)
        {
            var canvasImage = gameObject.GetComponent<RawImage>();
            if (canvasImage != null)
            {
                PaintComponent = canvasImage;
                RendererComponent = PaintComponent;
                ComponentType = componentType = ObjectComponentType.RawImage;
                return;
            }

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                PaintComponent = meshFilter;
                RendererComponent = gameObject.GetComponent<MeshRenderer>();
                if (RendererComponent == null)
                {
                    Debug.LogError("Can't find MeshRenderer component!");
                }
                ComponentType = componentType = ObjectComponentType.MeshFilter;
                return;
            }

            var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                PaintComponent = skinnedMeshRenderer;
                RendererComponent = PaintComponent;
                ComponentType = componentType = ObjectComponentType.SkinnedMeshRenderer;
                return;
            }

            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                PaintComponent = spriteRenderer;
                RendererComponent = PaintComponent;
                ComponentType = componentType = ObjectComponentType.SpriteRenderer;
                return;
            }

            Debug.LogError("Can't find render component in ObjectForPainting field!");
            ComponentType = componentType = ObjectComponentType.Unknown;
        }

        public bool IsMesh()
        {
            return RendererComponent is MeshRenderer || RendererComponent is SkinnedMeshRenderer;
        }

        public Texture GetSourceTexture(Material material, string shaderTextureName, int width, int height)
        {
            if (ComponentType == ObjectComponentType.SkinnedMeshRenderer || ComponentType == ObjectComponentType.MeshFilter)
            {
                if (!string.IsNullOrEmpty(shaderTextureName))
                {
                    var texture = material.GetTexture(shaderTextureName);
                    if (texture == null)
                    {
                        texture = CreateTexture(width, height);
                    }
                    return texture;
                }
            }
            else if (ComponentType == ObjectComponentType.SpriteRenderer)
            {
                var spriteRenderer = RendererComponent as SpriteRenderer;
                if (spriteRenderer != null)
                {
                    if (spriteRenderer.sprite == null)
                    {
                        var texture = CreateTexture(width, height);
                        var pixelPerUnit = Settings.Instance.PixelPerUnit;
                        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one / 2f, pixelPerUnit);
                    }
                    return spriteRenderer.sprite.texture;
                }
            }
            else if (ComponentType == ObjectComponentType.RawImage)
            {
                var image = RendererComponent as RawImage;
                if (image != null)
                {
                    if (image.texture == null)
                    {
                        var rect = image.rectTransform.rect;
                        image.texture = CreateTexture((int)rect.width, (int)rect.height);
                    }
                    return image.texture;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates clear texture
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Texture2D CreateTexture(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, _defaultTextureColor);
                }
            }
            texture.Apply();
            return texture;
        }
        
        public void SetSourceMaterial(Material material, int index = -1)
        {
            if (ComponentType == ObjectComponentType.SkinnedMeshRenderer || ComponentType == ObjectComponentType.MeshFilter || ComponentType == ObjectComponentType.SpriteRenderer)
            {
                var renderer = RendererComponent as Renderer;
                if (renderer != null)
                {
                    if (index == -1)
                    {
                        renderer.material = material;
                    }
                    else
                    {
                        var sharedMaterials = renderer.sharedMaterials;
                        sharedMaterials[index] = material;
                        renderer.sharedMaterials = sharedMaterials;
                    }
                    
                    var spriteRenderer = renderer as SpriteRenderer;
                    if (spriteRenderer == null) 
                        return;
                    var materialPropertyBlock = new MaterialPropertyBlock();
                    materialPropertyBlock.SetTexture(MainTexProp, material.mainTexture);
                    spriteRenderer.SetPropertyBlock(materialPropertyBlock);
                }
            }
            else if (ComponentType == ObjectComponentType.RawImage)
            {
                var image = RendererComponent as RawImage;
                if (image != null)
                {
                    image.material = material;
                    image.texture = material.mainTexture;
                }
            }
        }

        public Mesh GetMesh()
        {
            if (IsMesh())
            {
                var meshFilter = PaintComponent as MeshFilter;
                if (meshFilter != null)
                {
                    return meshFilter.sharedMesh;
                }
                
                var skinnedMeshRenderer = PaintComponent as SkinnedMeshRenderer;
                if (skinnedMeshRenderer != null)
                {
                    return skinnedMeshRenderer.sharedMesh;
                }
            }
            Debug.LogError("Can't find MeshFilter or SkinnedMeshRenderer component!");
            return null;
        }

        public int GetMaterialIndex(Material material)
        {
            if (ComponentType == ObjectComponentType.SkinnedMeshRenderer || ComponentType == ObjectComponentType.MeshFilter || ComponentType == ObjectComponentType.SpriteRenderer)
            {
                var renderer = RendererComponent as Renderer;
                if (renderer != null)
                {
                    var index = 0;
                    var sharedMaterials = renderer.sharedMaterials;
                    for (var i = 0; i < sharedMaterials.Length; i++)
                    {
                        if (sharedMaterials[i] == material)
                        {
                            index = i;
                            break;
                        }
                    }
                    return index;
                }
            }
            return -1;
        }
    }
}