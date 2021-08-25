using UnityEngine;

namespace XDPaint.Tools
{
    public static class ExtendedMethods
    {
        public static Vector2 Clamp(this Vector2 val, Vector2 from, Vector2 to)
        {
            if (val.x < from.x)
            {
                val.x = from.x;
            }
            if (val.y < from.y)
            {
                val.y = from.y;
            }
            if (val.x > to.x)
            {
                val.x = to.x;
            }
            if (val.y > to.y)
            {
                val.y = to.y;
            }
            return val;
        }
        
        public static bool IsNaNOrInfinity(this float val)
        {
            return float.IsInfinity(val) || float.IsNaN(val);
        }
    }
}