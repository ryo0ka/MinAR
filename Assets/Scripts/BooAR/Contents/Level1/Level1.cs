using AnimeRx;
using BooAR.Contents.Characters;
using BooAR.Levels;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Contents.Level1
{
	public class Level1 : LevelBase
	{
		[Inject]
		GhostSiren.Pool _sirenPool;

		[SerializeField]
		Destination _midPoint;

		[SerializeField]
		Destination _goalPoint;

		GhostSiren _siren;

		protected override async UniTask Begin()
		{
			await base.Begin();

			// Wait until player reaches the mid point
			await _midPoint.OnPlayerReachedAsObservable();
			CancelIfLevelEnded();

			SpawnSiren();

			// Wait until player interact with jack toggle
			await _siren.OnJackedAsObservable.ToUniTask(_level.OnEnded());
			CancelIfLevelEnded();

			// Wait until player reaches the goal
			await _goalPoint.OnPlayerReachedAsObservable();
			CancelIfLevelEnded();

			_level.Goal();
		}

		void SpawnSiren()
		{
			_siren = _sirenPool.Spawn(new GhostSiren.Params(), _life);
			_siren.transform.SetParent(_floor);
			_siren.transform.position = (_startPoint.Position + _midPoint.Position) / 2f;

			// Kill player when siren hits it
			_siren.OnReachedPlayerAsObservable
			      .FirstOrDefault()
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