using System;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR
{
	public class PauseController : IPauseController
	{
		[Inject]
		IGameConsts _consts;

		[Inject]
		IPlayer _player;

		readonly Subject<Unit> _onPaused = new Subject<Unit>();
		readonly Subject<Unit> _onResumeStarted = new Subject<Unit>();
		readonly Subject<Unit> _onResumeCompleted = new Subject<Unit>();

		TimeSpan ResumeTime => _consts.ResumeTime;
		float MaxDistance => _consts.ArIdDistance;
		float MaxAngle => _consts.ArIdAngle;
		float BounceLength => _consts.WallNormalLength;

		public bool IsPaused { get; private set; }
		public Vector3 RecoveryPosition { get; private set; }
		public Vector3 RecoveryRotation { get; private set; }

		public IObservable<Unit> OnPaused => _onPaused;
		public IObservable<Unit> OnResumeStarted => _onResumeStarted;
		public IObservable<Unit> OnResumeCompleted => _onResumeCompleted;

		public IDisposable SubscribeWall(Collider wall, Vector3 normal)
		{
			return _player.Core.OnTriggerEnterAsObservable(wall).Subscribe(_ =>
			{
				Debug.Log($"Off wall: {wall.transform.parent}");
				Vector3 position = _player.Transform.position + normal * BounceLength;
				Vector3 rotation = _player.Transform.eulerAngles;
				Pause(position, rotation);
			});
		}

		public void Pause(Vector3 position, Vector3 rotation)
		{
			DoPause(position, rotation).Away();
		}

		async UniTask DoPause(Vector3 position, Vector3 rotation)
		{
			if (IsPaused) return;

			IsPaused = true;
			RecoveryPosition = position;
			RecoveryRotation = rotation;

			bool canResume = false;
			while (!canResume)
			{
				_onPaused.OnNext();

				await Observable.EveryUpdate().First(_ => CanResume());

				_onResumeStarted.OnNext();

				canResume = await UniRxUtils.First(
					UniRxUtils.TimerUnscaled(ResumeTime).Select(_ => true),
					Observable.EveryUpdate().Where(_ => !CanResume()).Select(_ => false));
			}

			IsPaused = false;
			_onResumeCompleted.OnNext();
		}

		bool CanResume()
		{
			return CanResume((RecoveryPosition, RecoveryRotation), _player.Transform);
		}

		bool CanResume((Vector3, Vector3) original, Transform current)
		{
			return Vector3.Distance(original.Item1, current.position) < MaxDistance
			       && Vector3.Angle(original.Item2, current.eulerAngles) < MaxAngle;
		}
	}
}