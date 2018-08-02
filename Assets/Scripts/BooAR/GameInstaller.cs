using BooAR.Cameras;
using BooAR.Contents;
using BooAR.Games;
using BooAR.Games.Persistences;
using BooAR.Games.Views;
using BooAR.Levels;
using BooAR.Players;
using UnityEngine;
using Zenject;

namespace BooAR
{
	public class GameInstaller : MonoInstaller
	{
		[SerializeField]
		LevelCollection _levels;

		[SerializeField]
		Player _player;

		[SerializeField]
		CameraState _camera;

		[SerializeField]
		bool useDebugPersistence;

		[SerializeField]
		EditorGamePersistence _persistence;

		public override void InstallBindings()
		{
			Container.Bind<IGameState>()
			         .To<GameState>()
			         .AsSingle();

			if (Application.isEditor || Debug.isDebugBuild || useDebugPersistence)
			{
				Container.BindInstance<IGamePersistence>(_persistence);
			}
			else
			{
				Container.Bind<IGamePersistence>()
				         .To<GamePersistence>()
				         .AsSingle();
			}

			Container.BindInstance<ILevelCollection>(_levels);

			Container.BindInstance<ICameraState>(_camera);

			Container.BindInstance<IPlayer>(_player);

#if UNITY_IOS
			Container.Bind<IHapticFeedbackGenerator>()
			         .To<TouchHapticFeedbackGenerator>()
			         .AsSingle();
#endif
		}
	}
}