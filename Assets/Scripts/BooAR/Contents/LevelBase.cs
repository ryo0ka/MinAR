using System;
using BooAR.Cameras;
using BooAR.Contents.Characters;
using BooAR.Levels;
using BooAR.Players;
using UniRx.Async;
using UnityEngine;
using Zenject;
using UniRx;

namespace BooAR.Contents
{
	// Base class for level controllers.
	// Should do some initialilization and provide utility methods
	// so that the child classes don't have to re-implement common things.
	public class LevelBase : BaseBehaviour
	{
		[Inject]
		protected ILevelState _level;

		[Inject]
		protected IPlayer _player;

		[Inject]
		protected ICameraState _camera;

		[SerializeField]
		protected Transform _floor;

		[SerializeField]
		protected Destination _startPoint;

		protected readonly CompositeDisposable _life;
		protected bool _dontFailLevelOnPlayerKilled;

		protected LevelBase()
		{
			_life = new CompositeDisposable();
		}

		void Awake()
		{
			_life.AddTo(this);
			_dontFailLevelOnPlayerKilled = false;
		}

		void Start()
		{
			// Start the level controller with common error handling
			_level.Begun
			      .Where(o => o.HasValue) // skip when invalidly begun
			      .Subscribe(_ => OnShouldBegin())
			      .AddTo(this);
		}

		void OnShouldBegin()
		{
			Begin().Forget(exc =>
			{
				if (exc is OperationCanceledException)
				{
					Log($"Cancelled level: {exc}");
				}
				else
				{
					LogWarning($"Thrown in level controller: {exc.GetType()}");
					LogException(exc);
				}
			});
		}

		// Should be called at the beginning of overriding methods
		protected virtual async UniTask Begin()
		{
			_life.Clear();

			if (!_dontFailLevelOnPlayerKilled)
			{
				// Fail the level when player is killed
				_player.OnKilled
				       .Subscribe(_ => _level.Fail())
				       .AddTo(_life);
			}

			// Move the level to the AR floor position
			_floor.position = _level.Options().Position;
			_floor.eulerAngles = _level.Options().Rotation;

			Log("Move to the start point");

			// Wait until player stands at the start point
			await _startPoint.OnPlayerReachedAsObservable();
			CancelIfLevelEnded();
		}

		protected void CancelIfLevelEnded()
		{
			if (_level.IsFailed()) throw new OperationCanceledException("Level is failed");
			if (_level.IsGoaled()) throw new OperationCanceledException("Level is goaled");
			if (!this) throw new OperationCanceledException("Level is unloaded");
		}
	}
}