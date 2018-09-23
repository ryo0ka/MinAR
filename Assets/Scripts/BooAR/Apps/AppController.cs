using BooAR.Games;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Apps
{
	public class AppController : BaseBehaviour, IAppController
	{
#pragma warning disable 649
		[Inject]
		IAppPersistence _persistence;

		[SerializeField]
		GameController _game;
#pragma warning restore 649

		public async UniTask LoadGame(string id)
		{
			_game.Initialize();
			await _persistence.LoadAll(id);
		}

		public async UniTask SaveGame(string id)
		{
			await _persistence.SaveAll(id);
		}

		public void StartNewGame()
		{
			_game.Initialize();
			_game.TendSpawn();
		}

		public void SetPause(bool pause)
		{
			_game.SetPause(pause);
		}
	}
}