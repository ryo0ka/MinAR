using BooAR.Cameras;
using BooAR.Contents;
using BooAR.Games;
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
		ScreenState _screen;

		public override void InstallBindings()
		{
			Container.BindInstance<ILevelCollection>(_levels);
			
			Container.BindInstance<ICameraState>(_camera);

			Container.BindInstance<IPlayer>(_player);

			Container.BindInstance<IScreenState>(_screen);
		}
	}
}