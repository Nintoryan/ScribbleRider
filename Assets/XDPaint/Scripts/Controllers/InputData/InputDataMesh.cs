using UnityEngine;
using XDPaint.Controllers.InputData.Base;
using XDPaint.Tools.Raycast;

namespace XDPaint.Controllers.InputData
{
    public class InputDataMesh : InputDataBase
    {
        private Ray? _ray;
        private Triangle _triangle;
        
        public override void OnUpdate()
        {
            base.OnUpdate();
            _ray = null;
            _triangle = null;
        }
        
        public override void OnHover(Vector3 position)
        {
            _ray = Camera.ScreenPointToRay(Input.mousePosition);
            RaycastController.Instance.Raycast(_ray.Value, out _triangle);
            PaintManager.PaintObject.OnMouseHover(position, _triangle);
        }

        public override void OnDown(Vector3 position, float pressure = 1.0f)
        {
            if (_ray == null)
            {
                _ray = Camera.ScreenPointToRay(Input.mousePosition);
            }
            if (_triangle == null)
            {
                RaycastController.Instance.Raycast(_ray.Value, out _triangle);
            }
            PaintManager.PaintObject.OnMouseDown(position, pressure, _triangle);
        }

        public override void OnPress(Vector3 position, float pressure = 1.0f)
        {
            if (_ray == null)
            {
                _ray = Camera.ScreenPointToRay(Input.mousePosition);
            }
            if (_triangle == null)
            {
                RaycastController.Instance.Raycast(_ray.Value, out _triangle);
            }
            PaintManager.PaintObject.OnMouseButton(position, pressure, _triangle);
        }

        public override void OnUp(Vector3 position)
        {
            PaintManager.PaintObject.OnMouseUp(position);
        }
    }
}