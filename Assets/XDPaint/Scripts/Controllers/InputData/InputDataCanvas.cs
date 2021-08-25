using UnityEngine;
using UnityEngine.UI;
using XDPaint.Controllers.InputData.Base;
using XDPaint.Tools;
using XDPaint.Tools.Raycast;

namespace XDPaint.Controllers.InputData
{
    public class InputDataCanvas : InputDataBase
    {
        private CanvasGraphicRaycaster _canvasGraphicRaycaster;
        private bool _paintingStarted;

        public override void Init(PaintManager paintManager, Camera camera)
        {
            base.Init(paintManager, camera);
            if (Settings.Instance.CheckCanvasRaycasts)
            {
                var rawImage = paintManager.ObjectForPainting.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    _canvasGraphicRaycaster = rawImage.canvas.GetComponent<CanvasGraphicRaycaster>();
                    if (_canvasGraphicRaycaster == null)
                    {
                        _canvasGraphicRaycaster = rawImage.canvas.gameObject.AddComponent<CanvasGraphicRaycaster>();
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_canvasGraphicRaycaster != null)
            {
                Object.Destroy(_canvasGraphicRaycaster);
            }
        }

        public override void OnHover(Vector3 position)
        {
            if (Settings.Instance.CheckCanvasRaycasts && _canvasGraphicRaycaster != null)
            {
                var raycasts = _canvasGraphicRaycaster.GetRaycasts(position);
                if (raycasts != null && raycasts.Count > 0 && raycasts[0].gameObject == PaintManager.ObjectForPainting.gameObject)
                {
                    PaintManager.PaintObject.OnMouseHover(position);
                }
                else
                {
                    PaintManager.PaintObject.OnMouseHover(Vector2.zero);
                }
            }
            else
            {
                PaintManager.PaintObject.OnMouseHover(position);
            }
        }

        public override void OnDown(Vector3 position, float pressure = 1.0f)
        {
            if (Settings.Instance.CheckCanvasRaycasts && _canvasGraphicRaycaster != null)
            {
                var raycasts = _canvasGraphicRaycaster.GetRaycasts(position);
                if (raycasts != null && raycasts.Count > 0 && raycasts[0].gameObject == PaintManager.ObjectForPainting.gameObject)
                {
                    PaintManager.PaintObject.OnMouseDown(position, pressure);
                    _paintingStarted = true;
                }
                else
                {
                    _paintingStarted = false;
                }
            }
            else
            {
                PaintManager.PaintObject.OnMouseDown(position, pressure);
                _paintingStarted = true;
            }
        }

        public override void OnPress(Vector3 position, float pressure = 1.0f)
        {
            if (_paintingStarted)
            {
                PaintManager.PaintObject.OnMouseButton(position, pressure);
            }
        }

        public override void OnUp(Vector3 position)
        {
            PaintManager.PaintObject.OnMouseUp(Input.mousePosition);
        }
    }
}