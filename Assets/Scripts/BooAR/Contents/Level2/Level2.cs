using BooAR.Contents.Characters;
using BooAR.Levels;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;
using AnimeRx;

namespace BooAR.Contents.Level2
{
	public class Level2 : LevelBase
	{
		[Inject]
		GhostBoo.Pool _booPool;

		[Inject]
		GhostSiren.Pool _sirenPool;

		[SerializeField]
		Destination _midPoint;

		[SerializeField]
		Destination _goalPoint;

		GhostBoo _boo;
		GhostSiren _siren;

		protected override async UniTask Begin()
		{
			await base.Begin();

			SpawnBoo();
			SpawnSiren();

			// Wait until player reaches the mid point
			await _midPoint.OnPlayerReachedAsObservable();
			CancelIfLevelEnded();

			// Wait until player reaches the goal
			await _goalPoint.OnPlayerReachedAsObservable();
			CancelIfLevelEnded();

			_level.Goal();
		}

		void SpawnBoo()
		{
			_boo = _booPool.Spawn(new GhostBoo.Params(), _life);
			_boo.transform.SetParent(_floor);
			_boo.transform.position = _midPoint.transform.position;
			_boo.transform.SetPosition(y: _player.Transform.position.y);

			// Kill player when boo hits it
			_boo.OnReachedAsObservable
			    .First()
			    .Subscribe(_ => _player.Kill())
			    .AddTo(_life);
		}

		void SpawnSiren()
		{
			_siren = _sirenPool.Spawn(new GhostSiren.Params(), _life);
			_siren.transform.SetParent(_floor);
			_siren.transform.position = (_startPoint.Position + _midPoint.Position) / 2f;

			// Kill player when siren hits it
			_siren.OnReachedPlayerAsObservable
			      .First()
			      .Subscribe(_ => _player.Kill())
			      .AddTo(_life);

			// Move siren between player and the goal
			Observable.EveryFixedUpdate()
			          .Select(f => f * Time.smoothDeltaTime) // seconds past
			          .Select(s => Mathf.Sin(s / 2) * 3) // meters
			          .SubscribeToLocalPositionX(_siren.transform)
			          .AddTo(_life);
		}
	}
}