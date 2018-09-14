using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace BooAR.Voxel
{
	public class VoxelMeshBuilder
	{
		const int QuadVerts = 4;

		readonly List<Vector3> _vertices;
		readonly List<Vector3> _normals;
		readonly List<Vector2> _uvs;
		readonly List<int>[] _triangles;
		readonly Pool<List<int>> _trianglesPool;
		readonly int _submeshLength;

		int _lastVertIndex;

		public VoxelMeshBuilder(int initCapacity, int submeshLength)
		{
			_vertices = new List<Vector3>(QuadVerts * initCapacity);
			_normals = new List<Vector3>(QuadVerts * initCapacity);
			_uvs = new List<Vector2>(QuadVerts * initCapacity);
			_triangles = new List<int>[submeshLength];
			_trianglesPool = new Pool<List<int>>(submeshLength, () => new List<int>(32), e => e.Clear());
			_submeshLength = submeshLength;
		}

		public void Apply(Mesh mesh)
		{
			using (UnityUtils.Sample("QuadMeshBuilder.Apply()"))
			{
				mesh.Clear();
				mesh.SetVertices(_vertices);
				mesh.SetNormals(_normals);
				mesh.SetUVs(0, _uvs);
				mesh.subMeshCount = _submeshLength;

				for (int i = 0; i < _submeshLength; i++)
				{
					if (_triangles.TryGetValue(i, out List<int> submesh))
					{
						mesh.SetTriangles(submesh, i);
					}
				}
			}
		}

		public void AddCube(Vector3 position, int submeshId)
		{
			for (int i = 0; i < 6; i++)
			{
				Directions d = (Directions) i;
				VoxelUtils.Quad(d, out Vector3 v0, out Vector3 v1, out Vector3 v2, out Vector3 v3);
				Quad q = new Quad(v0, v1, v2, v3, 1, 1) + position;
				Add(q, submeshId);
			}
		}

		public void Clear()
		{
			_lastVertIndex = 0;
			_vertices.Clear();
			_normals.Clear();
			_uvs.Clear();

			foreach (List<int> p in _triangles)
			{
				if (p != null)
				{
					_trianglesPool.Enpool(p);
				}
			}

			_triangles.Clear();
		}

		public void Add(Quad quad, int submeshId)
		{
			AddVertices(quad);
			AddTriangles(submeshId);
			AddNormals();
			AddUVs(quad.Width, quad.Height);

			_lastVertIndex += QuadVerts;
		}

		void AddVertices(Quad q)
		{
			_vertices.Add(q.A);
			_vertices.Add(q.B);
			_vertices.Add(q.C);
			_vertices.Add(q.D);
		}

		void AddTriangles(int submeshId)
		{
			List<int> tris = GetSubTriangles(submeshId);

			tris.Add(_lastVertIndex + 0);
			tris.Add(_lastVertIndex + 1);
			tris.Add(_lastVertIndex + 3);
			tris.Add(_lastVertIndex + 3);
			tris.Add(_lastVertIndex + 1);
			tris.Add(_lastVertIndex + 2);
		}

		List<int> GetSubTriangles(int submeshId)
		{
			if (_triangles.TryGetValue(submeshId, out List<int> tris)) return tris;
			return _triangles[submeshId] = _trianglesPool.Unpool();
		}

		void AddNormals()
		{
			Vector3 v0 = _vertices[_lastVertIndex + 0];
			Vector3 v1 = _vertices[_lastVertIndex + 1];
			Vector3 v2 = _vertices[_lastVertIndex + 2];

			AddNormals(VoxelUtils.Normal(v0, v1, v2));
		}

		void AddNormals(Vector3 n)
		{
			_normals.Add(n);
			_normals.Add(n);
			_normals.Add(n);
			_normals.Add(n);
		}

		void AddUVs(int width, int height)
		{
			_uvs.Add(new Vector2(0f * width, 0f * height));
			_uvs.Add(new Vector2(0f * width, 1f * height));
			_uvs.Add(new Vector2(1f * width, 1f * height));
			_uvs.Add(new Vector2(1f * width, 0f * height));
		}
	}
}