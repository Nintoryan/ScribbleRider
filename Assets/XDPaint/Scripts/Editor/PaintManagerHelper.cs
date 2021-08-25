using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Component = UnityEngine.Component;

namespace XDPaint.Editor
{
    public static class PaintManagerHelper
    {        
        public const float MinHardnessValue = -10f;
        public const float MaxHardnessValue = 1f;
        public const float MinValue = 0.01f;
        public const float MaxValue = 8f;
        public const string TrianglesDataWindowTitle = "Triangles Data";
        public static readonly Vector2 TrianglesDataWindowSize = new Vector2(300, 100);
        public const string BrushTooltip = "Texture of brush";
        public const string BrushColorTooltip = "Color of brush";
        public const string BrushSizeTooltip = "Scale of brush";
        public const string BrushHardnessTooltip = "Hardness of brush";
        public const string CameraTooltip = "GameObject with Camera component";
        public const string MaterialTooltip = "Material for painting";
        public const string ObjectForPaintingTooltip = "Drag here GameObject for painting";
        public const string OverrideCameraTooltip = "Override Camera for transforming screen to world coordinates";
        public const string TextureSizeTip = "Texture size";
        public const string PaintingModeTooltip = "Painting mode";
        public const string PreviewTooltip = "If enabled, will be active preview of painting";
        public const string UseNeighborsVerticesForRaycastTooltip = "Use neighbors vertices data for raycasts. Improves performance, but reduces accuracy of painting";
        public const string UseSourceTextureAsBackgroundTooltip = "Use source texture as background.";
        public const string CopySourceTextureToPaintTextureTooltip = "Copies source texture to paint texture.";
        public const string ShaderTextureNameTooltip = "Shader texture name, which texture will be painted";
        public const string CloneMaterialTooltip = "Clones Material into new file";
        public const string CloneTextureTooltip = "Clones selected texture (Shader Texture Name) into new file";
        public const string UndoTooltip = "Undo painting";
        public const string RedoTooltip = "Redo painting";
        public const string BakeTooltip = "Bake modified texture to source texture. Note that it will not modify source texture file and stores in memory";
        public const string SaveToFileTooltip = "Save modified texture to file";
        public const string AutoFillButtonTooltip = "Trying to fill ObjectForPainting and Material fields automatically";

        private const string FilenamePostfix = " copy";
        private const string DefaultTextureFilename = "Texture.png";
        private static readonly string[] TextureImportPlatforms =
        {
            "Standalone", "Web", "iPhone", "Android", "WebGL", "Windows Store Apps", "PS4", "XboxOne", "Nintendo 3DS", "tvOS"
        };
        
        private static readonly Type[] SupportedTypes =
        {
            typeof(RawImage),
            typeof(MeshRenderer),
            typeof(SkinnedMeshRenderer),
            typeof(SpriteRenderer)
        };

        public static Component GetSupportedComponent(GameObject gameObject)
        {
            if (gameObject == null)
                return null;
            foreach (var supportedType in SupportedTypes)
            {
                var component = gameObject.GetComponent(supportedType);
                if (component != null)
                {
                    return component;
                }
            }
            return null;
        }
        
        public static bool IsMeshObject(Component component)
        {
            return component is MeshRenderer || component is SkinnedMeshRenderer;
        }

        public static string[] GetTexturesListFromShader(Material objectMaterial)
        {
            var allTexturesNames = new List<string>();
            var shader = objectMaterial.shader;
            var propertyCount = ShaderUtil.GetPropertyCount(shader);
            for (var i = 0; i < propertyCount; i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    allTexturesNames.Add(ShaderUtil.GetPropertyName(shader, i));
                }
            }
            return allTexturesNames.ToArray();
        }

        public static Material CloneMaterial(Material material)
        {
            var materialPath = AssetDatabase.GetAssetPath(material);
            var directoryName = Path.GetDirectoryName(materialPath);
            var fileName = Path.GetFileNameWithoutExtension(materialPath);
            var extension = Path.GetExtension(materialPath);
            string materialNewPath;
            do
            {
                fileName += FilenamePostfix;
                materialNewPath = Path.Combine(directoryName, fileName);
            } while (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path.Combine(directoryName, fileName + extension)));

            materialNewPath += extension;
            if (AssetDatabase.CopyAsset(materialPath, materialNewPath))
            {
                return AssetDatabase.LoadAssetAtPath<Material>(materialNewPath);
            }
            return null;
        }

        public static void CloneTexture(Material material, string textureName)
        {
            var texture = material.GetTexture(textureName);
            if (texture != null)
            {
                var texturePath = AssetDatabase.GetAssetPath(texture);
                var directoryName = Path.GetDirectoryName(texturePath);
                var fileName = Path.GetFileNameWithoutExtension(texturePath);
                var extension = Path.GetExtension(texturePath);
                string textureNewPath;
                do
                {
                    fileName += FilenamePostfix;
                    textureNewPath = Path.Combine(directoryName, fileName);
                } while (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(Path.Combine(directoryName, fileName + extension)));

                textureNewPath += extension;
                if (AssetDatabase.CopyAsset(texturePath, textureNewPath))
                {
                    var newTexture = AssetDatabase.LoadAssetAtPath<Texture>(textureNewPath);
                    material.SetTexture(textureName, newTexture);
                }
            }
        }

        public static void Bake(Texture sourceTexture, Action onBake)
        {
            var texturePath = AssetDatabase.GetAssetPath(sourceTexture);
            var textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.isReadable = true;
                
                if (onBake != null)
                {
                    onBake();
                }
            }
            AssetDatabase.Refresh();
        }

        public static void SaveToFile(PaintManager paintManager)
        {
            var sourceTexture = paintManager.Material.Material.mainTexture;
            var texturePath = AssetDatabase.GetAssetPath(sourceTexture);
            if (string.IsNullOrEmpty(texturePath))
            {
                texturePath = Application.dataPath + "/" + DefaultTextureFilename;
            }
            var textureImporterSettings = new TextureImporterSettings();
            var assetImporter = AssetImporter.GetAtPath(texturePath);
            var defaultPlatformSettings = new TextureImporterPlatformSettings();
            var platformsSettings = new Dictionary<string, TextureImporterPlatformSettings>();
            if (assetImporter != null)
            {
                var textureImporter = (TextureImporter)assetImporter;
                textureImporter.ReadTextureSettings(textureImporterSettings);
                defaultPlatformSettings = textureImporter.GetDefaultPlatformTextureSettings();
                foreach (var platform in TextureImportPlatforms)
                {
                    var platformSettings = textureImporter.GetPlatformTextureSettings(platform);
                    if (platformSettings != null)
                    {
                        platformsSettings.Add(platform, platformSettings);
                    }
                }
            }
            
            var directoryInfo = new FileInfo(texturePath).Directory;
            if (directoryInfo != null)
            {
                var directory = directoryInfo.FullName;
                var fileName = Path.GetFileName(texturePath);
                var path = EditorUtility.SaveFilePanel("Save texture as PNG", directory, fileName, "png");
                if (path.Length > 0)
                {
                    var texture2D = paintManager.GetResultTexture();
                    var pngData = texture2D.EncodeToPNG();
                    if (pngData != null)
                    {
                        File.WriteAllBytes(path, pngData);
                    }
                    
                    var importPath = path.Replace(Application.dataPath, "Assets");
                    AssetImporter importer = AssetImporter.GetAtPath(importPath);
                    if (importer != null)
                    {
                        var texture2DImporter = (TextureImporter)importer;
                        texture2DImporter.SetTextureSettings(textureImporterSettings);
                        texture2DImporter.SetPlatformTextureSettings(defaultPlatformSettings);
                        foreach (var platform in platformsSettings)
                        {
                            texture2DImporter.SetPlatformTextureSettings(platform.Value);
                        }
                        AssetDatabase.ImportAsset(importPath, ImportAssetOptions.ForceUpdate);
                    }
                    AssetDatabase.Refresh();
                }
            }
        }

        public static bool HasTexture(PaintManager paintManager)
        {
            var rawImage = paintManager.ObjectForPainting.GetComponent<RawImage>();
            if (rawImage != null)
            {
                return rawImage.texture != null;
            }
            
            var spriteRenderer = paintManager.ObjectForPainting.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                return spriteRenderer.sprite != null;
            }

            var meshRenderer = paintManager.ObjectForPainting.GetComponent<Renderer>();
            if (meshRenderer != null)
            {
                var shaderTextureName = paintManager.Material.ShaderTextureName;
                return paintManager.Material.SourceMaterial.GetTexture(shaderTextureName) != null;
            }
            return false;
        }
    }
}