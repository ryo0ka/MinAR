using System;
using AnimeRx;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Games.Views
{
	public class GameAnimator : MonoBehaviour
	{
		[Inject]
		IGameState _game;

		[Inject]
		IHapticFeedbackGenerator _haptic;

		[SerializeField]
		GameObject _controlPanel;

		[SerializeField]
		GameObject _resultControlPanel;

		[SerializeField]
		GameObject _resultPanel;

		[SerializeField]
		GameObject _clearIndicator;

		[SerializeField]
		GameObject _failIndicator;

		[SerializeField]
		CanvasGroup _initializeOverlay;

		[SerializeField]
		CanvasGroup _startOverlay;

		[SerializeField]
		CanvasGroup _failOverlay;

		[SerializeField]
		Text _levelIndexText;

		void Awake()
		{
			_startOverlay.gameObject.SetActive(false);
			_failOverlay.gameObject.SetActive(false);
		}

		void Start()
		{
			_game.OnInitializing().SubscribeAway(async onInitialized =>
			{
				_initializeOverlay.gameObject.SetActive(true);
				_initializeOverlay.alpha = 1f;

				_haptic.Trigger(HapticFeedbackTypes.Selection);

				await onInitialized;

				await Anime.Play(1f, 0f, Easing.InSine(.5f.Seconds()))
				           .Do(a => _initializeOverlay.alpha = a);

				_initializeOverlay.gameObject.SetActive(false);

				_controlPanel.SetActive(false);
				_resultPanel.SetActive(false);
				_resultControlPanel.SetActive(false);
			});

			_game.OnLevelLoading().SubscribeAway(async p =>
			{
				(int index, UniTask onLoaded) = p;

				_resultPanel.SetActive(false);
				_resultControlPanel.SetActive(true); // debug

				_startOverlay.gameObject.SetActive(true);
				_startOverlay.alpha = 1f;
				_levelIndexText.text = $"{index}";

				_haptic.Trigger(HapticFeedbackTypes.Selection);

				await onLoaded;

				await Anime.Play(1f, 1f, Easing.InSine(.5f.Seconds()))
				           .Play(1f, 0f, Easing.InSine(.5f.Seconds()))
				           .Do(a => _startOverlay.alpha = a);

				_startOverlay.gameObject.SetActive(false);

				_controlPanel.SetActive(true);
			});

			_game.OnLevelEnded().SubscribeAway(async goaled =>
			{
				_resultControlPanel.SetActive(true);
				_resultPanel.SetActive(true);
				_clearIndicator.gameObject.SetActive(goaled);
				_failIndicator.SetActive(!goaled);

				if (!goaled)
				{
					_failOverlay.gameObject.SetActive(true);

					await Anime.Play(1f, 0f, Easing.InSine(.5f.Seconds()))
					           .Do(a => _failOverlay.alpha = a);

					_failOverlay.gameObject.SetActive(false);
				}
			});

			_game.Pause.Subscribe(paused =>
			{
				Time.timeScale = paused ? 0f : 1f;
				_haptic.Trigger(HapticFeedbackTypes.Selection);
			});
		}
	}
}