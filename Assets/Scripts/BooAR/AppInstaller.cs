using BooAR.Apps;
using BooAR.ARs;
using BooAR.Games;
using BooAR.Haptics;
using BooAR.Voxel;
using UnityEngine;
using Zenject;

namespace BooAR
{
	public class AppInstaller : MonoInstaller
	{
		[Header("Voxels:")]
		[SerializeField]
		VoxelSource _meshSource;

		[SerializeField]
		Chunk _chunkPrefab;

		[SerializeField]
		VoxelWorld _voxels;

		[SerializeField]
		VoxelSettings _table;

		[SerializeField]
		BlockParticleSystem _particlesPrefab;

		[Header("ARs:")]

		[SerializeField]
		UnityARCameraManager _arkitCamera;

		[Header("UIs:")]
		[SerializeField]
		InventoryButton _inventoryButtonPrefab;

		[SerializeField]
		Transform _inventoryButtonRoot;

		[SerializeField]
		GameListItem _gameListItemPrefab;

		[SerializeField]
		Transform _gameListItemRoot;

		[Header("Controllers:")]

		[SerializeField]
		GameController _game;

		[SerializeField]
		AppController _app;

		public override void InstallBindings()
		{
			InstallVoxelGame();
			InstallHapticFeedback();
			InstallPersistence();
			InstallApp();
		}

		void InstallVoxelGame()
		{
			// VOXEL

			Container.BindInstance(_meshSource);

			Container.QueueForInject(_table);
			Container.BindInstance<ITerrainGenerator>(_table);
			Container.BindInstance<IBlockAttributeTable>(_table);
			_table.Initialize(); // initialize here

			Container.BindMemoryPool<Chunk, Chunk.Pool>()
			         .FromComponentInNewPrefab(_chunkPrefab)
			         .UnderTransform(_voxels.transform);

			Container.BindInstance<IGlobalBlockLookup>(_voxels);

			Container.BindMemoryPool<BlockParticleSystem, BlockParticleSystem.Pool>()
			         .FromComponentInNewPrefab(_particlesPrefab)
			         .UnderTransform(_voxels.transform);

			Container.Bind<IBlockParticlePresenter>().To<BlockParticlePresenter>().AsSingle();

			// INVENTORY

			Container.Bind<IInventory>().To<Inventory>().AsSingle();

			Container.BindMemoryPool<InventoryButton, InventoryButton.Pool>()
			         .FromComponentInNewPrefab(_inventoryButtonPrefab)
			         .UnderTransform(_inventoryButtonRoot);
			
			Container.BindInstance<IInventoryToolController>(_game);
		}

		void InstallHapticFeedback()
		{
#if UNITY_IOS
			Container.Bind<IHapticFeedbackGenerator>()
			         .To<TouchHapticFeedbackGenerator>()
			         .AsSingle();
#endif
		}

		void InstallPersistence()
		{
			Container.BindInstance<IVoxelPersistence>(_voxels);
			Container.Bind<IGamePersistence>().To<GamePersistence>().AsSingle();
			
			// AR
#if UNITY_IOS
			Container.BindInstance(_arkitCamera);
			Container.Bind<IArWorldPersistence>().To<ArKitWorldPersistence>().AsSingle();
#endif

			Container.Bind<AppPersistenceFacade>().AsSingle();
			Container.Bind<IAppPersistence>().To<AppPersistenceFacade>().FromResolve();
			Container.Bind<IGameList>().To<AppPersistenceFacade>().FromResolve();
		}

		void InstallApp()
		{
			Container.BindMemoryPool<GameListItem, GameListItem.Pool>()
			         .FromComponentInNewPrefab(_gameListItemPrefab)
			         .UnderTransform(_gameListItemRoot);

			Container.BindInstance<IAppController>(_app);
		}
	}
}