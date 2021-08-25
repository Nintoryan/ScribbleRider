using UnityEngine;
using XDPaint.Controllers.InputData.Base;

namespace XDPaint.Controllers.InputData
{
    public class InputDataDefault : InputDataBase
    {
        public override void OnHover(Vector3 position)
        {
            PaintManager.PaintObject.OnMouseHover(position);
        }

        public override void OnDown(Vector3 position, float pressure = 1.0f)
        {
            PaintManager.PaintObject.OnMouseDown(position, pressure);
        }

        public override void OnPress(Vector3 position, float pressure = 1.0f)
        {
            PaintManager.PaintObject.OnMouseButton(position, pressure);
        }

        public override void OnUp(Vector3 position)
        {
            PaintManager.PaintObject.OnMouseUp(position);
        }
    }
}