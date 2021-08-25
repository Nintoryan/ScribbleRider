using System;
using UnityEngine;
using XDPaint.Tools.Raycast;

namespace XDPaint.Tools
{
    public static class TrianglesData
    {
        public static Action<float> OnUpdate;
        public static Action OnFinish;
        
        private static bool _shouldBreak;
        public static void Break()
        {
            _shouldBreak = true;
        }
        
        public static Triangle[] GetData(Mesh mesh, bool fillNeighbors = true)
        {
            var indices = mesh.triangles;
            if (indices.Length == 0)
            {
                Debug.LogError("Mesh doesn't have indices!");
                return new Triangle[0];
            }
            if (mesh.uv.Length == 0)
            {
                Debug.LogError("Mesh doesn't have UV!");
                return new Triangle[0];
            }

            var indexesCount = indices.Length;
            var triangles = new Triangle[indexesCount / 3];
            for (var i = 0; i < indexesCount; i += 3)
            {
                var index = i / 3;
                var index0 = indices[i + 0];
                var index1 = indices[i + 1];
                var index2 = indices[i + 2];
                
                triangles[index] = new Triangle((ushort)index, (ushort)index0, (ushort)index1, (ushort)index2);
            }

            if (fillNeighbors)
            {
                var positions = mesh.vertices;
                for (var i = 0; i < triangles.Length; i++)
                {
                    if (OnUpdate != null)
                    {
                        OnUpdate(i / (float)triangles.Length);
                    }
                    if (_shouldBreak)
                        break;
                    
                    var triangle = triangles[i];
                    var index0 = triangle.I0;
                    var index1 = triangle.I1;
                    var index2 = triangle.I2;

                    foreach (var triangleFind in triangles)
                    {
                        var indexFind0 = triangleFind.I0;
                        var indexFind1 = triangleFind.I1;
                        var indexFind2 = triangleFind.I2;

                        if (triangleFind.Id != triangle.Id)
                        {
                            if (index0 == indexFind0 || index0 == indexFind1 || index0 == indexFind2 ||
                                index1 == indexFind0 || index1 == indexFind1 || index1 == indexFind2 ||
                                index2 == indexFind0 || index2 == indexFind1 || index2 == indexFind2)
                            {
                                if (!triangle.N.Contains(triangleFind.Id))
                                {
                                    triangle.N.Add(triangleFind.Id);
                                }
                            }
                        }
                    }
                }
                _shouldBreak = false;
                if (OnFinish != null)
                {
                    OnFinish();
                }
            }
            return triangles;
        }
    }
}