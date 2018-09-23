using UnityEngine;
using UnityEngine.Rendering;
using Utils;
using Zenject;

namespace BooAR.Voxel
{
	public class BlockDamageMeshGenerator : BaseBehaviour, IBlockDamagePresenter
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
		VoxelMeshBuilder _meshBuilder;

		void Start()
		{
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

		public void ResetDamage()
		{
			_meshBuilder.Clear();
			_mesh.Clear();
		}

		public void UpdateHealth(Vector3i position, float health)
		{
			_meshBuilder.Clear();
			_mesh.Clear();

			_meshBuilder.AddCube(position, GetSubmeshID(health));
			_meshBuilder.Apply(_mesh);
		}

		int GetSubmeshID(float health)
		{
			return Mathf.FloorToInt(health * VoxelConsts.DamageLength);
		}
	}
}