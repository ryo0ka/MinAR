using BooAR.ARs;
using BooAR.Cameras;
using BooAR.Characters;
using BooAR.Haptics;
using BooAR.Persistences;
using UnityEngine;
using Zenject;

namespace BooAR
{
	public class AppInstaller : MonoInstaller
	{
		[SerializeField]
		Room _roomPrefab;

		[SerializeField]
		Transform _roomRoot;

		public override void InstallBindings()
		{
			Container.Bind<IAppState>()
			         .To<AppController>()
			         .FromComponentInHierarchy();

			Container.Bind<IGameState>()
			         .To<GameController>()
			         .FromComponentInHierarchy();

			Container.Bind<IGameController>()
			         .To<GameController>()
			         .FromComponentInHierarchy();

			Container.Bind<IGameConsts>()
			         .To<GameConsts>()
			         .FromComponentInHierarchy();

			Container.Bind<IPlayer>()
			         .To<Player>()
			         .FromComponentInHierarchy();

			Container.Bind<IPauseController>()
			         .To<PauseController>()
			         .AsSingle();

			Container.Bind<IFloorController>()
			         .To<FloorController>()
			         .FromComponentInHierarchy();

			Container.Bind<IPersistenceController>()
			         .To<PersistenceController>()
			         .FromComponentInHierarchy();

			Container.Bind<ICameraController>()
			         .To<CameraController>()
			         .FromComponentInHierarchy();

			Container.Bind<IArCamera>()
			         .To<ArCamera>()
			         .FromComponentInHierarchy();

			Container.Bind<IGridWorld>()
			         .To<GridWorld>()
			         .AsSingle();

			Container.BindMemoryPool<Room, Room.Pool>()
			         .FromComponentInNewPrefab(_roomPrefab)
			         .UnderTransform(_roomRoot);

#if UNITY_IOS
			Container.Bind<IHapticFeedbackGenerator>()
			         .To<TouchHapticFeedbackGenerator>()
			         .AsSingle();
#endif
		}
	}
}