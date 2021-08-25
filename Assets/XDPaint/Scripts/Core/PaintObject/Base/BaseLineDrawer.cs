using System;
using System.Collections.Generic;
using UnityEngine;
using XDPaint.Controllers;
using XDPaint.Tools;
using XDPaint.Tools.Raycast;

namespace XDPaint.Core.PaintObject.Base
{
	public class BaseLineDrawer
    {
        private bool _useNeighborsVertices;
        public bool UseNeighborsVertices
        {
            set { _useNeighborsVertices = value; }
        }

        private Camera _camera;
        public Camera Camera
        {
            set { _camera = value; }
        }

        private Transform Transform
        {
            get { return _firstTriangle.Transform; }
        }

        private Action<Vector3[], Vector2[], int[], Color[]> _onDrawLine;
        private List<Vector2> _drawPositions = new List<Vector2>();
        private Triangle _firstTriangle, _lastTriangle;
        private Vector3 _normal, _cameraLocalPosition;
        private Vector2 _sourceTextureSize;
        
        private const int IterationsMaxCount = 256;
        private const float OffsetValue = 0.0001f;
        private const float MinNormalLength = 0.000001f;

        private Vector3 IntersectionOffset
        {
            get
            {
                return (_firstTriangle.Hit - _lastTriangle.Hit) * OffsetValue;
            }
        }

        public void Init(Camera camera, Vector2 sourceTextureSize, Action<Vector3[], Vector2[], int[], Color[]> onDrawLine)
        {
            _camera = camera;
            _sourceTextureSize = sourceTextureSize;
            _onDrawLine = onDrawLine;
        }

        public Vector2[] GetLinePositions(Vector2 paintUV1, Vector2 paintUV2, Triangle triangleA, Triangle triangleB, bool canRetry = true)
        {
            _firstTriangle = triangleA;
            _lastTriangle = triangleB;
            
            if (canRetry)
            {
                var firstWorld = Transform.TransformPoint(_firstTriangle.Hit);
                var lastWorld = Transform.TransformPoint(_lastTriangle.Hit);
                _camera.WorldToScreenPoint(firstWorld);
                _camera.WorldToScreenPoint(lastWorld);
                
                _cameraLocalPosition = Transform.InverseTransformPoint(_camera.transform.position);
                _normal = Vector3.Cross(_firstTriangle.Hit - _cameraLocalPosition, _lastTriangle.Hit - _cameraLocalPosition);
                if (_normal.magnitude < MinNormalLength)
                {
                    _drawPositions.Add(paintUV1);
                    _drawPositions.Add(paintUV1);
                    _drawPositions.Add(paintUV2);
                    _drawPositions.Add(paintUV2);
                    return _drawPositions.ToArray();
                }
            }

            var iterationsCount = 0;
            var currentTriangle = _firstTriangle;
            var triangles = new List<int>();
            
            if (Mathf.Abs(IntersectionOffset.magnitude) < Mathf.Epsilon)
                return _drawPositions.ToArray();

            Vector3 intersection;
            var uvFirst = GetIntersectionUV(_firstTriangle, _lastTriangle.Hit, out intersection);
            _drawPositions.Add(paintUV1);
            _drawPositions.Add(uvFirst);
            var beginExit = intersection;
            
            while (iterationsCount < IterationsMaxCount && currentTriangle.Id != _lastTriangle.Id)
            {
                iterationsCount++;
                intersection -= IntersectionOffset;
                Triangle triangle;
                var ray = GetRay(intersection);
                if (_useNeighborsVertices)
                {
                    RaycastController.Instance.NeighborsRaycast(currentTriangle, ray, out triangle);
                }
                else
                {
                    RaycastController.Instance.RaycastLocal(ray, Transform, out triangle);
                }

                if (triangle == null)
                {
                    if (canRetry)
                    {
                        return GetLinePositions(paintUV2, paintUV1, _lastTriangle, _firstTriangle, false);
                    }
                    break;
                }

                if (triangle.Id != _lastTriangle.Id && currentTriangle.Id != triangle.Id)
                {
                    currentTriangle = triangle;
                    if (_useNeighborsVertices)
                    {
                        if (triangles.Contains(currentTriangle.Id))
                        {
                            break;
                        }
                        triangles.Add(currentTriangle.Id);
                    }
                    
                    intersection = MathHelper.GetExitPointFromTriangle(_camera, currentTriangle, beginExit, _lastTriangle.Hit, _normal);
                    beginExit = intersection;
                    ray = GetRay(intersection);
                    var uv = MathHelper.GetIntersectionUV(currentTriangle, ray);
                    var uvRaycast = new Vector2(triangle.UVHit.x * _sourceTextureSize.x, triangle.UVHit.y * _sourceTextureSize.y);
                    uv = new Vector2(uv.x * _sourceTextureSize.x, uv.y * _sourceTextureSize.y);
                    _drawPositions.Add(uvRaycast);
                    _drawPositions.Add(uv);
                }
                else
                {
                    break;
                }
            }

            var uvLast = GetIntersectionUV(_lastTriangle, beginExit, out intersection);
            _drawPositions.Add(uvLast);
            _drawPositions.Add(paintUV2);
            return _drawPositions.ToArray();
        }

        private Vector2 GetIntersectionUV(Triangle triangle, Vector3 exit, out Vector3 exitPosition)
        {
            exitPosition = MathHelper.GetExitPointFromTriangle(_camera, triangle, triangle.Hit, exit, _normal);
            var ray = GetRay(exitPosition);
            var uv = MathHelper.GetIntersectionUV(triangle, ray);
            return new Vector2(uv.x * _sourceTextureSize.x, uv.y * _sourceTextureSize.y);
        }

        private Ray GetRay(Vector3 point)
        {
            var normal = (point - _cameraLocalPosition);
            return new Ray(point + normal, -normal);
        }

        private float GetRatio(float totalDistanceInPixels, float brushSize, float[] brushSizes)
        {
            var brushPressureStart = brushSizes[0];
            var brushPressureEnd = brushSizes[1];
            var pressureDifference = Mathf.Abs(brushPressureStart - brushPressureEnd);
            var brushCenterPartWidth = Mathf.Clamp(Settings.Instance.BrushDuplicatePartWidth * brushSize, 1f, 100f);
            var ratioBrush = totalDistanceInPixels * pressureDifference / brushCenterPartWidth;
            var ratioSource = totalDistanceInPixels / brushCenterPartWidth;
            var ratio = (ratioSource + ratioBrush) / totalDistanceInPixels;
            return ratio;
        }

        public void RenderLine(Action<Vector2> onDraw, Vector2[] drawPositions, Texture brushTexture, float brushSizeActual, float[] brushSizes, bool isUndo = false)
        {
            var sourceTextureSize = new Vector2(_sourceTextureSize.x, _sourceTextureSize.y);
            var brushPressureStart = brushSizes[0];
            var brushPressureEnd = brushSizes[1];
            var brushWidth = brushTexture.width;
            var brushHeight = brushTexture.height;
            var maxBrushPressure = Mathf.Max(brushPressureStart, brushPressureEnd);
            var brushOffset = new Vector2(brushWidth, brushHeight) * maxBrushPressure;
            var distances = new float[drawPositions.Length / 2];
            var totalDistance = 0f;
            for (var i = 0; i < drawPositions.Length - 1; i += 2)
            {
                var from = drawPositions[i + 0];
                from = from.Clamp(Vector2.zero - brushOffset, sourceTextureSize + brushOffset);
                var to = drawPositions[i + 1];
                to = to.Clamp(Vector2.zero - brushOffset, sourceTextureSize + brushOffset);
                drawPositions[i + 0] = from;
                drawPositions[i + 1] = to;
                distances[i / 2] = Vector2.Distance(from, to);
                totalDistance += distances[i / 2];
            }
            var ratio = GetRatio(totalDistance, brushSizeActual, brushSizes) * 2f;
            var verticesCount = 0;
            for (var i = 0; i < drawPositions.Length - 1; i += 2)
            {
                verticesCount += (int)(distances[i / 2] * ratio + 1);
            }
            verticesCount = Mathf.Clamp(verticesCount, drawPositions.Length / 2, 16384);
            var positions = new Vector3[verticesCount * 4];
            var colors = new Color[verticesCount * 4];
            var indices = new int[verticesCount * 6];
            var uv = new Vector2[verticesCount * 4];
            var count = 0;
            for (var i = 0; i < drawPositions.Length - 1; i += 2)
            {
                var from = drawPositions[i + 0];
                var to = drawPositions[i + 1];
                var currentDistance = Mathf.Max(1, (int)(distances[i / 2] * ratio));
                for (var j = 0; j < currentDistance; j++)
                {
                    var minDistance = Mathf.Max(1, (float) (verticesCount - 1));
                    var t = Mathf.Clamp(count / minDistance, 0, 1);
                    var thickness = Mathf.Lerp(brushPressureStart, brushPressureEnd, t);
                    var holePosition = from + (to - from) / currentDistance * j;
                    
                    var positionRect = new Rect(
                        (holePosition.x - 0.5f * brushWidth * thickness) / _sourceTextureSize.x,
                        (holePosition.y - 0.5f * brushHeight * thickness) / _sourceTextureSize.y,
                        brushWidth  * thickness / _sourceTextureSize.x,
                        brushHeight  * thickness / _sourceTextureSize.y
                    );

                    positions[count * 4 + 0] = new Vector3(positionRect.xMin, positionRect.yMax, 0);
                    positions[count * 4 + 1] = new Vector3(positionRect.xMax, positionRect.yMax, 0);
                    positions[count * 4 + 2] = new Vector3(positionRect.xMax, positionRect.yMin, 0);
                    positions[count * 4 + 3] = new Vector3(positionRect.xMin, positionRect.yMin, 0);

                    colors[count * 4 + 0] = Color.white;
                    colors[count * 4 + 1] = Color.white;
                    colors[count * 4 + 2] = Color.white;
                    colors[count * 4 + 3] = Color.white;

                    uv[count * 4 + 0] = Vector2.up;
                    uv[count * 4 + 1] = Vector2.one;
                    uv[count * 4 + 2] = Vector2.right;
                    uv[count * 4 + 3] = Vector2.zero;

                    indices[count * 6 + 0] = 0 + count * 4;
                    indices[count * 6 + 1] = 1 + count * 4;
                    indices[count * 6 + 2] = 2 + count * 4;
                    indices[count * 6 + 3] = 2 + count * 4;
                    indices[count * 6 + 4] = 3 + count * 4;
                    indices[count * 6 + 5] = 0 + count * 4;

                    count++;
                }
            }

            if (positions.Length > 0)
            {
                //BasePaintObjectRenderer.RenderLine
                _onDrawLine(positions, uv, indices, colors);
            }
            
            if (!isUndo)
            {
                if (onDraw != null)
                {
                    //BasePaintTool.OnPaintPosition
                    onDraw(Vector2.zero);
                }
            }

            _drawPositions.Clear();
        }
    }
}