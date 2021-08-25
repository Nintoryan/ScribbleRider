using UnityEngine;

namespace XDPaint.Tools
{
    [CreateAssetMenu(fileName = "XDPaintSettings", menuName = "XDPaint Settings", order = 100)]
    public class Settings : SingletonScriptableObject<Settings>
    {
        public Shader BrushShader;
        public Shader BrushRenderShader;
        public Shader EyedropperShader;
        public Shader BrushSamplerShader;
        public Shader BrushCloneShader;
        public Shader BlurShader;
        public Shader GaussianBlurShader;
        public Shader BrushBlurShader;
        public Shader PaintShader;
        public Shader AverageColorShader;
        public Texture DefaultBrush;
        public Texture DefaultCircleBrush;
        public bool UndoRedoEnabled = true;
        public uint UndoRedoMaxActionsCount = 20;
        public bool PressureEnabled = true;
        public bool CheckCanvasRaycasts = true;
        public uint BrushDuplicatePartWidth = 4;
        public float PixelPerUnit = 100f;
        public string ContainerGameObjectName = "[XDPaintContainer]";
    }
}