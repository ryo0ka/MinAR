using AnimeRx;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR
{
	public class GameUiAnimator : MonoBehaviour
	{
		[Inject]
		IAppState _app;

		[Inject]
		IGameState _game;

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

		void Awake()
		{
			_startOverlay.gameObject.SetActive(false);
			_failOverlay.gameObject.SetActive(false);
		}

		void Start()
		{
			_app.OnAppInitializing().SubscribeAway(async onInitialized =>
			{
				_initializeOverlay.gameObject.SetActive(true);
				_initializeOverlay.alpha = 1f;

				await onInitialized;

				await Anime.Play(1f, 0f, Easing.InSine(.5f.Seconds()))
				           .Do(a => _initializeOverlay.alpha = a);

				_initializeOverlay.gameObject.SetActive(false);

				_controlPanel.SetActive(false);
				_resultPanel.SetActive(false);
				_resultControlPanel.SetActive(false);
			});

			_app.OnGameLoading().SubscribeAway(async onLoaded =>
			{
				_resultPanel.SetActive(false);
				_resultControlPanel.SetActive(true); // debug

				_startOverlay.gameObject.SetActive(true);
				_startOverlay.alpha = 1f;

				await onLoaded;

				await Anime.Play(1f, 1f, Easing.InSine(.5f.Seconds()))
				           .Play(1f, 0f, Easing.InSine(.5f.Seconds()))
				           .Do(a => _startOverlay.alpha = a);

				_startOverlay.gameObject.SetActive(false);

				_controlPanel.SetActive(true);
			});

			_game.OnEnded().SubscribeAway(async goaled =>
			{
				_resultControlPanel.SetActive(true);
				_resultPanel.SetActive(true);
				_clearIndicator.gameObject.SetActive(goaled);
				_failIndicator.SetActive(!goaled);

				if (!goaled) // game over
				{
					_failOverlay.gameObject.SetActive(true);

					await Anime.Play(1f, 0f, Easing.InSine(.5f.Seconds()))
					           .Do(a => _failOverlay.alpha = a);

					_failOverlay.gameObject.SetActive(false);
				}
			});
		}
	}
}