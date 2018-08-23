using System.Collections.Generic;
using System.Threading;
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
		readonly Dictionary<Blocks, List<int>> _triangles;
		readonly Pool<List<int>> _trianglesPool;

		int _lastVertIndex;

		public VoxelMeshBuilder(int initCapacity)
		{
			_vertices = new List<Vector3>(QuadVerts * initCapacity);
			_normals = new List<Vector3>(QuadVerts * initCapacity);
			_uvs = new List<Vector2>(QuadVerts * initCapacity);
			_triangles = new Dictionary<Blocks, List<int>>(BlocksUtils.All.Length);
			_trianglesPool = new Pool<List<int>>(BlocksUtils.All.Length, () => new List<int>(32), e => e.Clear());
		}

		public void Apply(Mesh mesh)
		{
			using (UnityUtils.Sample("QuadMeshBuilder.Apply()"))
			{
				mesh.Clear();
				mesh.SetVertices(_vertices);
				mesh.SetNormals(_normals);
				mesh.SetUVs(0, _uvs);
				mesh.subMeshCount = BlocksUtils.All.Length;

				foreach (Blocks block in BlocksUtils.All)
				{
					if (_triangles.TryGetValue(block, out List<int> submesh))
					{
						mesh.SetTriangles(submesh, (int) block);
					}
				}
			}
		}

		public void Update(IEnumerable<Quad> quads, CancellationToken canceller)
		{
			Clear();

			foreach (Quad quad in quads)
			{
				canceller.ThrowIfCancellationRequested();

				Add(quad);
			}
		}

		public void Clear()
		{
			_lastVertIndex = 0;
			_vertices.Clear();
			_normals.Clear();
			_uvs.Clear();

			foreach (var p in _triangles)
			{
				_trianglesPool.Enpool(p.Value);
			}

			_triangles.Clear();
		}

		void Add(Quad q)
		{
			AddVertices(q);
			AddTriangles(q.Block);
			AddNormals();
			//TODO UV

			_lastVertIndex += QuadVerts;
		}

		void AddVertices(Quad q)
		{
			_vertices.Add(q.A);
			_vertices.Add(q.B);
			_vertices.Add(q.C);
			_vertices.Add(q.D);
		}

		void AddTriangles(Blocks block)
		{
			List<int> tris = GetSubTriangles(block);

			tris.Add(_lastVertIndex + 0);
			tris.Add(_lastVertIndex + 1);
			tris.Add(_lastVertIndex + 3);
			tris.Add(_lastVertIndex + 3);
			tris.Add(_lastVertIndex + 1);
			tris.Add(_lastVertIndex + 2);
		}

		List<int> GetSubTriangles(Blocks block)
		{
			if (_triangles.TryGetValue(block, out List<int> tris)) return tris;
			return _triangles[block] = _trianglesPool.Unpool();
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
	}
}