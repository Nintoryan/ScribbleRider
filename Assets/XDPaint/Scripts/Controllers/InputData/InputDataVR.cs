using UnityEngine;
using XDPaint.Controllers.InputData.Base;
using XDPaint.Tools.Raycast;

namespace XDPaint.Controllers.InputData
{
    public class InputDataVR : InputDataBase
    {
        private Ray? _ray;
        private Triangle _triangle;
        private Transform _penTransform;
        private Vector3 _penDirection;
        private Vector3 _screenPoint = -Vector3.one;

        public override void Init(PaintManager paintManager, Camera camera)
        {
            base.Init(paintManager, camera);
            _penTransform = InputController.Instance.PenTransform;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _ray = null;
            _triangle = null;
        }

        public override void OnHover(Vector3 position)
        {
            _screenPoint = -Vector3.one;
            _penDirection = _penTransform.forward;
   
            _ray = new Ray(_penTransform.position, _penDirection);
            RaycastController.Instance.Raycast(_ray.Value, out _triangle);
            if (_triangle != null)
            { 
                _screenPoint = Camera.WorldToScreenPoint(_triangle.Hit);
            }
            PaintManager.PaintObject.OnMouseHover(_screenPoint, _triangle);
        }

        public override void OnDown(Vector3 position, float pressure = 1.0f)
        {
            if (_ray == null)
            {
                _ray = Camera.ScreenPointToRay(position);
            }
            if (_triangle == null)
            {
                RaycastController.Instance.Raycast(_ray.Value, out _triangle);
            }
            if (_triangle != null)
            {
                _screenPoint = Camera.WorldToScreenPoint(_triangle.Hit);
            }
            PaintManager.PaintObject.OnMouseDown(_screenPoint, pressure, _triangle);
        }

        public override void OnPress(Vector3 position, float pressure = 1.0f)
        {
            if (_ray == null)
            {
                _ray = Camera.ScreenPointToRay(position);
            }
            if (_triangle == null)
            {
                RaycastController.Instance.Raycast(_ray.Value, out _triangle);
            }
            if (_triangle != null)
            {
                _screenPoint = Camera.WorldToScreenPoint(_triangle.Hit);
            }
            PaintManager.PaintObject.OnMouseButton(position, pressure, _triangle);
        }

        public override void OnUp(Vector3 position)
        {
            PaintManager.PaintObject.OnMouseUp(position);
        }
    }
}