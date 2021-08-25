using UnityEngine;

namespace XDPaint.Tools
{
    public class Barycentric
    {
        private float _u;
        private float _v;
        private float _w;

        public Barycentric()
        {
        }

        public Barycentric(Vector3 aV1, Vector3 aV2, Vector3 aV3, Vector3 aP)
        {
            Vector3 a = aV2 - aV3, b = aV1 - aV3, c = aP - aV3;
            var aLen = a.x * a.x + a.y * a.y + a.z * a.z;
            var bLen = b.x * b.x + b.y * b.y + b.z * b.z;
            var ab = a.x * b.x + a.y * b.y + a.z * b.z;
            var ac = a.x * c.x + a.y * c.y + a.z * c.z;
            var bc = b.x * c.x + b.y * c.y + b.z * c.z;
            var d = aLen * bLen - ab * ab;
            _u = (aLen * bc - ab * ac) / d;
            _v = (bLen * ac - ab * bc) / d;
            _w = 1.0f - _u - _v;
        }

        public Vector3 Interpolate(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return v1 * _u + v2 * _v + v3 * _w;
        }
    }
}