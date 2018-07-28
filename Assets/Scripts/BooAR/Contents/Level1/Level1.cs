using BooAR.Cameras;
using BooAR.Contents.Characters;
using BooAR.Games;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BooAR.Contents.Level1
{
	public class Level1 : LevelBase
	{
		[Inject]
		GhostSiren.Pool _sirenPool;

		[Inject]
		ICameraState _camera;

		[Inject]
		IScreenState _screen;

		[SerializeField]
		Destination _midPoint;

		[SerializeField]
		Destination _goalPoint;

		[SerializeField]
		Toggle _jackPrefab;

		GhostSiren _siren;

		protected override async UniTask Begin()
		{
			await base.Begin();

			{
				_siren = _sirenPool.Spawn(new GhostSiren.Params
				{
					Player = _player.Transform,
				});

				_siren.transform.SetParent(_floor);
				_siren.transform.position = InitialSirenPosition();

				_screen.Instantiate(_jackPrefab, out Toggle jack)
				       .AddTo(this);

				jack.OnValueChangedAsObservable()
				    .Subscribe(OnJackToggled)
				    .AddTo(this);

				jack.isOn = false;
			}

			await _midPoint.WaitUntilReached(_player.Transform);

			CancelIfLevelEnded();

			await _goalPoint.WaitUntilReached(_player.Transform);

			CancelIfLevelEnded();

			_level.Goal();
		}

		void OnJackToggled(bool jack)
		{
			_siren.SetActiveRendering(jack);
			if (jack) _camera.SetToSubCamera(_siren.View);
			else _camera.SetToMainCamera();
		}

		Vector3 InitialSirenPosition()
		{
			Vector3 startPos = _startPoint.transform.position;
			Vector3 midPos = _midPoint.transform.position;

			Vector3 pos = (startPos + midPos) / 2f;
			pos.y = _floor.position.y; // set it on the floor

			return pos;
		}
	}
}