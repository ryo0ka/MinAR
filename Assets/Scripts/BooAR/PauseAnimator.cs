using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR
{
	public class PauseAnimator : BaseBehaviour
	{
		[Inject]
		IPauseController _controller;

		[Inject]
		IGameConsts _consts;

		[SerializeField]
		GameObject _uiRoot;

		[SerializeField]
		CanvasGroup _overlay;

		[SerializeField]
		Transform _recoveryIndicator;

		[SerializeField]
		Text _resumeText;

		TimeSpan ResumeTime => _consts.ResumeTime;

		void Start()
		{
			SetPausing(false);

			_controller.OnPaused.Subscribe(_ => Log("Paused"));
			_controller.OnResumeStarted.Subscribe(_ => Log("Resume started"));
			_controller.OnResumeCompleted.Subscribe(_ => Log("Resume completed"));

			_controller.OnPaused.Subscribe(_ =>
			{
				SetPausing(true);
			});

			_controller.OnResumeStarted.Subscribe(_ =>
			{
				_resumeText.gameObject.SetActive(true);

				UniRxUtils
					.IntervalUnscaled(TimeSpan.FromTicks(ResumeTime.Ticks / 3))
					.StartWithUnit()
					.Scan(4, (tick, __) => tick - 1)
					.Take(3)
					.TakeUntil(_controller.OnPaused)
					.Subscribe(tick => _resumeText.text = $"{tick}");
			});

			_controller.OnResumeCompleted.Subscribe(_ =>
			{
				SetPausing(false);
			});
		}

		void SetPausing(bool pausing)
		{
			Time.timeScale = (pausing) ? 0f : 1f;
			_uiRoot.SetActive(pausing);
			_recoveryIndicator.gameObject.SetActive(pausing);
			_resumeText.gameObject.SetActive(false);
			_overlay.alpha = (pausing) ? 1f : 0f;

			if (pausing)
			{
				_recoveryIndicator.position = _controller.RecoveryPosition;
				_recoveryIndicator.eulerAngles = _controller.RecoveryRotation;
			}
		}
	}
}