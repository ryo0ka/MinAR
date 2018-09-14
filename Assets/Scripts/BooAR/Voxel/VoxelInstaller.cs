using BooAR.Haptics;
using UnityEngine;
using Zenject;

namespace BooAR.Voxel
{
	public class VoxelInstaller : MonoInstaller
	{
		[SerializeField]
		VoxelSource _meshSource;

		[SerializeField]
		Chunk _chunkPrefab;

		[SerializeField]
		VoxelWorld _voxels;

		[SerializeField]
		BlockInventoryButton _inventoryButtonPrefab;

		[SerializeField]
		Transform _inventoryRoot;

		[SerializeField]
		BlockParticleSystem _particlesPrefab;

		[SerializeField]
		VoxelSettings _table;

		public override void InstallBindings()
		{
			Container.BindInstance(_meshSource);

			Container.BindMemoryPool<Chunk, Chunk.Pool>()
			         .FromComponentInNewPrefab(_chunkPrefab)
			         .UnderTransform(_voxels.transform);

			Container.BindInstance<IGlobalBlockLookup>(_voxels);

			Container.BindMemoryPool<BlockInventoryButton, BlockInventoryButton.Pool>()
			         .FromComponentInNewPrefab(_inventoryButtonPrefab)
			         .UnderTransform(_inventoryRoot);

			Container.QueueForInject(_table);
			Container.BindInstance<ITerrainGenerator>(_table);
			Container.BindInstance<IBlockAttributeTable>(_table);

			Container.BindMemoryPool<BlockParticleSystem, BlockParticleSystem.Pool>()
			         .FromComponentInNewPrefab(_particlesPrefab)
			         .UnderTransform(_voxels.transform);

			Container.Bind<BlockParticleSystemController>().AsSingle();

#if UNITY_IOS
			Container.Bind<IHapticFeedbackGenerator>()
			         .To<TouchHapticFeedbackGenerator>()
			         .AsSingle();
#endif

			_table.Initialize(); // initialize here
		}
	}
}