using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Apps
{
	public class AppMenu : BaseBehaviour
	{
#pragma warning disable 649
		[Inject]
		IAppController _app;

		[SerializeField]
		GameObject _menuRoot;

		[SerializeField]
		Button _showMenuButton;

		[SerializeField]
		Button _hideMenuButton;

		[SerializeField]
		Button _showGameListButton;

		[SerializeField]
		Button _startNewGameButton;

		[SerializeField]
		Button _showSaveGameButton;

		[SerializeField]
		GameList _gameList;
 
		[SerializeField]
		GameSavePrompt _savePrompt;
#pragma warning restore 649

		void Start()
		{
			_showMenuButton
				.OnClickAsObservable()
				.Subscribe(_ => SetMenuVisible(true));

			_hideMenuButton
				.OnClickAsObservable()
				.Subscribe(_ => SetMenuVisible(false));

			_startNewGameButton
				.OnClickAsObservable()
				.Subscribe(_ => StartNewGame());

			_showGameListButton
				.OnClickAsObservable()
				.Subscribe(_ => ShowGameList().Away());

			_showSaveGameButton
				.OnClickAsObservable()
				.Subscribe(_ => ShowGameSavePrompt().Away());

			// Show menu on app launch
			SetMenuVisible(true);
		}

		void StartNewGame()
		{
			_app.StartNewGame();
			SetMenuVisible(false);
		}

		async UniTask ShowGameList()
		{
			string selectedID = await _gameList.Open();
			if (selectedID != null)
			{
				await _app.LoadGame(selectedID);
				SetMenuVisible(false);
			}
		}

		async UniTask ShowGameSavePrompt()
		{
			string selectedID = await _savePrompt.Open();
			Debug.Log(selectedID);
			if (selectedID != null)
			{
				await _app.SaveGame(selectedID);
				SetMenuVisible(false);
			}
		}

		void SetMenuVisible(bool visible)
		{
			//Debug.Log($"SetMenuVisible({visible})");
			_menuRoot.SetActive(visible);
			_app.SetPause(visible);
		}
	}
}