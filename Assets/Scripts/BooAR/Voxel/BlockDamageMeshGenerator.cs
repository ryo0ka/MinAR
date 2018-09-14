using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;
using Zenject;

namespace BooAR.Voxel
{
	public class BlockDamageMeshGenerator : BaseBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		MeshFilter _filter;

		[SerializeField]
		MeshRenderer _renderer;

		[Inject]
		VoxelSource _source;
#pragma warning restore 

		Mesh _mesh;
		Dictionary<Vector3i, float> _healths;
		VoxelMeshBuilder _meshBuilder;
		bool _updatedQuads;

		void Awake()
		{
			_healths = new Dictionary<Vector3i, float>();
			_meshBuilder = new VoxelMeshBuilder(128, VoxelConsts.DamageLength);

			// Initialize mesh
			_mesh = new Mesh {indexFormat = IndexFormat.UInt16};
			_filter.sharedMesh = _mesh;

			// Initialize materials
			Material[] materials = new Material[VoxelConsts.DamageLength];
			for (int i = 0; i < VoxelConsts.DamageLength; i++)
			{
				Material mat = Instantiate(_source.DamageMaterial);
				mat.SetFloat(VoxelConsts.DamageIndexKey, i);
				materials[i] = mat;
			}

			_renderer.sharedMaterials = materials;
		}

		public void UpdateHealth(Vector3i position, float health)
		{
			_healths[position] = health;
			_updatedQuads = true;
		}

		public void ResetHealth(Vector3i position)
		{
			_healths.Remove(position);
			_updatedQuads = true;
		}

		public void ResetHealthAll()
		{
			_healths.Clear();
			_updatedQuads = true;
		}

		public void UpdateQuads()
		{
			// Don't update if updated already
			if (!_updatedQuads) return;
			_updatedQuads = false;
			
			_meshBuilder.Clear();

			foreach (var p in _healths)
			{
				Vector3i position = p.Key;
				float health = p.Value;

				int submeshId = Mathf.FloorToInt(health * VoxelConsts.DamageLength);
				_meshBuilder.AddCube(position, submeshId);
			}
		}

		public void UpdateMesh()
		{
			_mesh.Clear();
			_meshBuilder.Apply(_mesh);
		}
	}
}