using System.Collections.Generic;
using UnityEngine;
using XDPaint.Tools.Raycast;

namespace XDPaint.Core.PaintObject.Base
{
	public class LineData
	{
		private List<Triangle> _triangles = new List<Triangle>();
		private List<Vector2> _paintPositions = new List<Vector2>();
		private List<float> _brushSizes = new List<float>();

		public void AddBrush(float brushSize)
		{
			if (_brushSizes.Count > 1)
			{
				_brushSizes.RemoveAt(0);
			}

			_brushSizes.Add(brushSize);
		}

		public void AddPosition(Vector2 position)
		{
			if (_paintPositions.Count > 1)
			{
				_paintPositions.RemoveAt(0);
			}

			_paintPositions.Add(position);
		}

		public void AddTriangleBrush(Triangle triangle, float brushSize)
		{
			if (_triangles.Count > 1)
			{
				_triangles.RemoveAt(0);
			}

			_triangles.Add(triangle);

			if (_brushSizes.Count > 1)
			{
				_brushSizes.RemoveAt(0);
			}

			_brushSizes.Add(brushSize);
		}

		public float[] GetBrushes()
		{
			return _brushSizes.ToArray();
		}
		
		public Triangle[] GetTriangles()
		{
			return _triangles.ToArray();
		}
		
		public Vector2[] GetPositions()
		{
			return _paintPositions.ToArray();
		}

		public bool HasOnePosition()
		{
			return _paintPositions.Count == 1;
		}

		public bool HasNotSameTriangles()
		{
			return _triangles.Count == 2 && _triangles[0].Id != _triangles[1].Id;
		}

		public void Clear()
		{
			_triangles.Clear();
			_paintPositions.Clear();
			_brushSizes.Clear();
		}
	}
}