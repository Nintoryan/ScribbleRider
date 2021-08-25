using UnityEngine;
using UnityEngine.Rendering;
using XDPaint.Controllers;
using XDPaint.Core;
using XDPaint.Core.PaintObject.Base;

namespace XDPaint.Tools.Image.Base
{
    public class BasePaintTool
    {
        /// <summary>
        /// Type of the tool
        /// </summary>
        public virtual PaintTool Type
        {
            get { return PaintTool.Brush; }
        }
        
        public virtual bool ShowPreview
        {
            get { return PaintController.Instance.Preview; }
        }

        public virtual bool DrawPreview
        {
            get { return false; }
        }
        
        public virtual bool RenderToPaintTexture
        {
            get;
            protected set;
        }
        
        public virtual bool RenderToLineTexture
        {
            get;
            protected set;
        }

        public virtual bool AllowRender
        {
            get { return RenderToPaintTexture || RenderToLineTexture || ShowPreview || DrawPreview; }
        }
        
        public virtual bool RenderToTextures
        {
            get { return RenderToPaintTexture || RenderToLineTexture; }
        }
        
        public virtual bool DrawPostProcess
        {
            get;
            protected set;
        }

        protected bool Preview;

        /// <summary>
        /// Enter the tool
        /// </summary>
        public virtual void Enter()
        {
            DrawPostProcess = false;
            RenderToPaintTexture = true;
            RenderToLineTexture = true;
            Preview = PaintController.Instance.Preview;
            PaintController.Instance.Preview = ShowPreview;
        }

        /// <summary>
        /// Exit from the tool
        /// </summary>
        public virtual void Exit()
        {
            PaintController.Instance.Preview = Preview;
        }

        /// <summary>
        /// On Mouse Hover handler
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="paintPosition"></param>
        /// <param name="pressure"></param>
        public virtual void UpdateHover(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
        {
        }
        
        /// <summary>
        /// On Mouse Down handler
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="paintPosition"></param>
        /// <param name="pressure"></param>
        public virtual void UpdateDown(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
        {
        }
        
        /// <summary>
        /// On Mouse Press handler
        /// </summary>
        /// <param name="uv"></param>
        /// <param name="paintPosition"></param>
        /// <param name="pressure"></param>
        public virtual void UpdatePress(object sender, Vector2 uv, Vector2 paintPosition, float pressure)
        {
        }

        /// <summary>
        /// On Paint Line handler (BasePaintObject.OnPaintLineHandler)
        /// </summary>
        /// <param name="paintPosition"></param>
        /// <param name="pressure"></param>
        public virtual void OnPaint(object sender, Vector2 paintPosition, float pressure)
        {
        }
        
        /// <summary>
        /// On Mouse Up handler
        /// </summary>
        public virtual void UpdateUp(object sender, bool inBounds)
        {
        }

        /// <summary>
        /// Post Process handler
        /// </summary>
        /// <param name="commandBuffer"></param>
        /// <param name="rti"></param>
        /// <param name="material"></param>
        public virtual void OnDrawPostProcess(object sender, CommandBuffer commandBuffer, RenderTargetIdentifier rti, Material material)
        {
            DrawPostProcess = false;
        }
        
        /// <summary>
        /// On Undo handler
        /// </summary>
        /// <param name="sender"></param>
        public virtual void OnUndo(object sender)
        {
            RenderPaintObject(sender);
        }
        
        /// <summary>
        /// On Redo handler
        /// </summary>
        /// <param name="sender"></param>
        public virtual void OnRedo(object sender)
        {
            RenderPaintObject(sender);
        }
        
        private void RenderPaintObject(object sender)
        {
            var paintObject = sender as BasePaintObject;
            if (paintObject == null) 
                return;
            var previousRenderToPaintTexture = RenderToPaintTexture;
            RenderToPaintTexture = true;
            paintObject.OnRender();
            paintObject.RenderCombined();
            RenderToPaintTexture = previousRenderToPaintTexture;
        }
    }
}