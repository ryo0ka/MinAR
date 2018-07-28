using System;
using BooAR.Contents.Characters;
using BooAR.Levels;
using BooAR.Players;
using UniRx.Async;
using UnityEngine;
using Zenject;
using UniRx;

namespace BooAR.Contents
{
	public class LevelBase : BaseBehaviour
	{
		[Inject]
		protected ILevelState _level;

		[Inject]
		protected IPlayer _player;

		[SerializeField]
		protected Transform _floor;

		[SerializeField]
		protected Destination _startPoint;

		void Start()
		{
			Begin().Forget(exc =>
			{
				if (exc is OperationCanceledException)
				{
					Log($"Cancelled level flow: {exc}");
				}
				else
				{
					LogException(exc);
				}
			});
		}

		protected virtual async UniTask Begin()
		{
			// Move the level to the AR floor position
			_floor.position = _level.Options.Position;
			_floor.eulerAngles = _level.Options.Rotation;

			Log("Move to the start point");

			await _startPoint.WaitUntilReached(_player.Transform);

			CancelIfLevelEnded();
		}

		protected void CancelIfLevelEnded()
		{
			if (_level.Failed) throw new OperationCanceledException("Level is failed");
			if (_level.Goaled) throw new OperationCanceledException("Level is goaled");
			if (!this) throw new OperationCanceledException("Level is unloaded");
		}
	}
}