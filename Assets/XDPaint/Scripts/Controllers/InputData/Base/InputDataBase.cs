using UnityEngine;

namespace XDPaint.Controllers.InputData.Base
{
    public abstract class InputDataBase
    {
        protected PaintManager PaintManager;
        protected Camera Camera;
        
        public virtual void Init(PaintManager paintManager, Camera camera)
        {
            PaintManager = paintManager;
            Camera = camera;
        }
        
        public virtual void OnDestroy()
        {
        }

        public virtual void OnUpdate()
        {
        }
        
        public abstract void OnHover(Vector3 position);
        public abstract void OnDown(Vector3 position, float pressure = 1.0f);
        public abstract void OnPress(Vector3 position, float pressure = 1.0f);
        public abstract void OnUp(Vector3 position);
    }
}