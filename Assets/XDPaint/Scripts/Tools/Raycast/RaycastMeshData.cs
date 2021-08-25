using System.Collections.Generic;
using UnityEngine;

namespace XDPaint.Tools.Raycast
{	
	public class RaycastMeshData
	{
		public Transform Transform;
		public Vector3[] Vertices;
		public Vector2[] UV;

		public bool UseLossyScale
		{
			get
			{
				return _skinnedMeshRenderer != null;
			}
		}
		
		private SkinnedMeshRenderer _skinnedMeshRenderer;
		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;
		private Mesh _mesh;
		private Triangle[] _triangles;

		public void Init(Component paintComponent, Component rendererComponent, Triangle[] triangles)
		{
			_mesh = new Mesh();
			_triangles = triangles;
			Transform = paintComponent.transform;
			_skinnedMeshRenderer = rendererComponent as SkinnedMeshRenderer;
			_meshRenderer = rendererComponent as MeshRenderer;
			_meshFilter = paintComponent as MeshFilter;
			if (_meshFilter != null)
			{
				var sharedMesh = _meshFilter.sharedMesh;
				Vertices = sharedMesh.vertices;
				UV = sharedMesh.uv;
			}
		}

		public bool IsRendererEquals(Component rendererComponent)
		{
			return _skinnedMeshRenderer != null && _skinnedMeshRenderer == rendererComponent as SkinnedMeshRenderer || 
			       _meshRenderer != null && _meshRenderer == rendererComponent as MeshRenderer;
		}

		public void Destroy()
		{
			if (_mesh != null)
			{
				Object.Destroy(_mesh);
			}
		}

		public IEnumerable<Triangle> GetNeighborsRaycasts(Triangle currentTriangle, Ray ray)
		{
			var intersects = new List<Triangle>();
			foreach (var triangleId in currentTriangle.N)
			{
				var triangle = _triangles[triangleId];
				var isIntersected = IsIntersected(triangle, ray, false);
				if (isIntersected)
				{
					intersects.Add(triangle);
				}
			}
			return intersects;
		}

		public IEnumerable<Triangle> GetRaycasts(Ray ray, bool useWorld = true)
		{
			if (useWorld)
			{
				bool boundsIntersect;
				if (_skinnedMeshRenderer != null)
				{
					_skinnedMeshRenderer.BakeMesh(_mesh);
					Vertices = _mesh.vertices;
					UV = _mesh.uv;
					boundsIntersect = _skinnedMeshRenderer.bounds.IntersectRay(ray);
				}
				else
				{
					boundsIntersect = _meshRenderer.bounds.IntersectRay(ray);
				}

				if (!boundsIntersect)
				{
					return null;
				}
				
				var origin = Transform.InverseTransformPoint(ray.origin);
				var direction = Transform.InverseTransformDirection(ray.direction);
				ray = new Ray(origin, direction);
			}
			
			var intersects = new List<Triangle>();
			foreach (var triangle in _triangles)
			{
				var isIntersected = IsIntersected(triangle, ray, useWorld);
				if (isIntersected)
				{
					intersects.Add(triangle);
				}
			}
			return intersects;
		}

		private bool IsIntersected(Triangle triangle, Ray ray, bool writeHit = true)
		{
			var eps = Mathf.Epsilon;
			var p1 = triangle.Position0;
			var p2 = triangle.Position1;
			var p3 = triangle.Position2;
			var e1 = p2 - p1;
			var e2 = p3 - p1;
			var p = Vector3.Cross(ray.direction, e2);
			var det = Vector3.Dot(e1, p);
			if (det.IsNaNOrInfinity() || det > eps && det < -eps)
			{
				return false;
			}
			var invDet = 1.0f / det;
			var t = ray.origin - p1;
			var u = Vector3.Dot(t, p) * invDet;
			if (u.IsNaNOrInfinity() || u < 0f || u > 1f)
			{
				return false;
			}
			var q = Vector3.Cross(t, e1);
			var v = Vector3.Dot(ray.direction, q) * invDet;
			if (v.IsNaNOrInfinity() || v < 0f || u + v > 1f)
			{
				return false;
			}
			if ((Vector3.Dot(e2, q) * invDet) > eps)
			{
				var hit = p1 + u * e1 + v * e2;
				if (writeHit)
				{
					triangle.Hit = hit;
				}
				triangle.UVHit = triangle.UV0 + ((triangle.UV1 - triangle.UV0) * u) + (triangle.UV2 - triangle.UV0) * v;
				return true;
			}
			return false;
		}
	}
}