using BooAR.Haptics;
using UnityEngine;
using Zenject;

namespace BooAR.Voxel
{
	public class VoxelInstaller : MonoInstaller
	{
		public enum Ids
		{
			ChunkLength,
			BlockSize,
		}

		[SerializeField]
		int _chunkLength;

		[SerializeField]
		float _blockSize;

		[SerializeField]
		VoxelMeshSource _meshSource;

		[SerializeField]
		Chunk _chunkPrefab;

		[SerializeField]
		VoxelWorld _voxels;

		[SerializeField]
		InventoryButton _inventoryButtonPrefab;

		[SerializeField]
		Transform _inventoryRoot;

		[SerializeField]
		VoxelSettings _table;

		public override void InstallBindings()
		{
			Container.BindInstance(_chunkLength).WithId(Ids.ChunkLength);
			Container.BindInstance(_blockSize).WithId(Ids.BlockSize);
			Container.BindInstance(_meshSource);

			Container.BindMemoryPool<Chunk, Chunk.Pool>()
			         .FromComponentInNewPrefab(_chunkPrefab)
			         .UnderTransform(_voxels.transform);

			Container.BindInstance<IGlobalBlockLookup>(_voxels);

			Container.BindMemoryPool<InventoryButton, InventoryButton.Pool>()
			         .FromComponentInNewPrefab(_inventoryButtonPrefab)
			         .UnderTransform(_inventoryRoot);

			Container.QueueForInject(_table);
			Container.BindInstance<ITerrainGenerator>(_table);
			Container.BindInstance<IBlockAttributeTable>(_table);

#if UNITY_IOS
			Container.Bind<IHapticFeedbackGenerator>()
			         .To<TouchHapticFeedbackGenerator>()
			         .AsSingle();
#endif

			_table.Initialize(); // initialize here
		}
	}
}