using System;
using System.Collections.Generic;
using UnityEngine;

namespace XDPaint.Tools.Raycast
{
	[Serializable]
	public class Triangle
	{
		//Triangle id
		public ushort Id;
		//Index 0
		public ushort I0;
		//Index 1
		public ushort I1;
		//Index 2
		public ushort I2;		
		//Neighbors
		public List<ushort> N = new List<ushort>();
		
		private RaycastMeshData _meshData;
		private Barycentric _barycentricLocal;

		public Transform Transform
		{
			get { return _meshData.Transform; }
		}

		public Vector3 Position0
		{
			get
			{
				if (_meshData.UseLossyScale)
				{
					var scale = new Vector3(1f / Transform.lossyScale.x, 1f / Transform.lossyScale.y, 1f / Transform.lossyScale.z);
					return Vector3.Scale(_meshData.Vertices[I0], scale);
				}
				return _meshData.Vertices[I0];
			}
		}

		public Vector3 Position1
		{
			get
			{
				if (_meshData.UseLossyScale)
				{
					var scale = new Vector3(1f / Transform.lossyScale.x, 1f / Transform.lossyScale.y, 1f / Transform.lossyScale.z);
					return Vector3.Scale(_meshData.Vertices[I1], scale);
				}
				return _meshData.Vertices[I1];
			}
		}
		
		public Vector3 Position2
		{
			get
			{
				if (_meshData.UseLossyScale)
				{
					var scale = new Vector3(1f / Transform.lossyScale.x, 1f / Transform.lossyScale.y, 1f / Transform.lossyScale.z);
					return Vector3.Scale(_meshData.Vertices[I2], scale);
				}
				return _meshData.Vertices[I2];
			}
		}
		
		public Vector3 Hit
		{
			get
			{
				if (_barycentricLocal == null)
				{
					_barycentricLocal = new Barycentric();
				}
				return _barycentricLocal.Interpolate(Position0, Position1, Position2);
			}
			
			set
			{
				_barycentricLocal = new Barycentric(Position0, Position1, Position2, value);
			}
		}

		public Vector3 WorldHit
		{
			get
			{
				var localHit = Hit;
				return Transform.localToWorldMatrix.MultiplyPoint(localHit);
			}
		}

		private Vector2 _uvHit;
		public Vector2 UVHit
		{
			get { return _uvHit; }
			set { _uvHit = value; }
		}

		public Vector2 UV0
		{
			get { return _meshData.UV[I0]; }
		}
		
		public Vector2 UV1
		{
			get { return _meshData.UV[I1]; }
		}
		
		public Vector2 UV2
		{
			get { return _meshData.UV[I2]; }
		}

		public Triangle(ushort id, ushort index0, ushort index1, ushort index2)
		{
			Id = id;
			I0 = index0;
			I1 = index1;
			I2 = index2;
		}

		public void SetTrianglesContainer(RaycastMeshData container)
		{
			_meshData = container;
		}
	}
}