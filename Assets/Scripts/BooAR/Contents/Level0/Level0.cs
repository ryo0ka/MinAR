using BooAR.Contents.Characters;
using BooAR.Levels;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;
using Utils;

namespace BooAR.Contents.Level0
{
	public class Level0 : LevelBase
	{
		[Inject]
		GhostBoo.Pool _booPool;

		[SerializeField]
		Destination _midPoint;

		[SerializeField]
		Destination _goalPoint;

		protected override async UniTask Begin()
		{
			await base.Begin();

			// Wait until player reaches the mid point
			await _midPoint.OnPlayerReachedAsObservable();
			CancelIfLevelEnded();

			SpawnBoo();

			// Wait unitl player reaches the goal
			await _goalPoint.OnPlayerReachedAsObservable();
			CancelIfLevelEnded();

			_level.Goal();
		}

		void SpawnBoo()
		{
			GhostBoo boo = _booPool.Spawn(new GhostBoo.Params(), _life);

			boo.transform.SetParent(_floor);
			boo.transform.position = _goalPoint.transform.position;
			boo.transform.SetPosition(y: _player.Transform.position.y);

			// Kill player when boo hits it
			boo.OnReachedAsObservable
			   .First()
			   .Subscribe(_ => _player.Kill())
			   .AddTo(_life);
		}
	}
}