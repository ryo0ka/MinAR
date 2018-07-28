using BooAR.Contents.Characters;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;

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

			{
				// Spawn a boo
				GhostBoo boo = _booPool.Spawn(new GhostBoo.Params
				{
					Player = _player.Transform,
				});

				// Move the boo to the mid point
				boo.transform.SetParent(_floor);
				boo.transform.position = _midPoint.transform.position;
				boo.transform.SetPosition(y: _player.Transform.position.y);
			}

			await _midPoint.WaitUntilReached(_player.Transform);

			CancelIfLevelEnded();

			await _goalPoint.WaitUntilReached(_player.Transform);

			CancelIfLevelEnded();

			_level.Goal();
		}
	}
}