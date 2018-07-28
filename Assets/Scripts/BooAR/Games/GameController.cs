using System;
using System.Threading;
using BooAR.Cameras;
using BooAR.Levels;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Sirenix.OdinInspector;
using Utils;

namespace BooAR.Games
{
	public class GameController : BaseBehaviour
	{
		[Inject]
		ILevelCollection _levels;

		[Inject]
		ICameraState _camera;

		[SerializeField]
		FloorLayoutPrompter _layoutPrompter;

		[SerializeField]
		Button _exitButton;

		[SerializeField]
		Button _retryLevelButton;

		[SerializeField]
		Button _nextLevelButton;

		[SerializeField]
		Button _lastLevelButton;

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

		[SerializeField, ReadOnly]
		int _levelIndex;

		readonly CompositeDisposable _level = new CompositeDisposable();
		FloorOptions _levelOptions;

		void Start()
		{
			_camera.SetToMainCamera();

			// Pressing the exit button should re-initialize the game anytime
			_exitButton.OnClickAsObservable().Subscribe(_ =>
			{
				Initialize().Away();
			});

			Initialize().Away();
		}

		async UniTask Initialize()
		{
			_level.Clear();

			await _levels.UnloadAll();

			_controlPanel.SetActive(false);
			_resultPanel.SetActive(false);
			_resultControlPanel.SetActive(false);

			_levelIndex = 0;
			_levelOptions = await _layoutPrompter.PromptLayout();

			_controlPanel.SetActive(true);

			StartLevel(_levelIndex).Away();
		}

		// ReSharper disable once FunctionRecursiveOnAllPaths
		async UniTask StartLevel(int index)
		{
			_level.Clear();
			CancellationToken canceller = _level.NewCancellationToken();

			await _levels.UnloadAll();

			_resultPanel.SetActive(false);

			//debug
			{
				_resultControlPanel.SetActive(true);

				ObserveNextLevelInput()
					.Subscribe(_ =>
					{
						StartLevel(_levelIndex).Away();
					})
					.AddTo(_level);
			}

			// No levels should be already loaded at this point
			Debug.Assert(_levels.Current == null);

			// Load and start the level
			ILevelState level = await _levels.Initialize(index);
			level.Begin(_levelOptions);

			// Wait until the level ends
			await level.OnEnded().ToUniTask(canceller, true);

			_resultControlPanel.SetActive(true);
			_resultPanel.SetActive(true);
			_clearIndicator.gameObject.SetActive(level.Goaled);
			_failIndicator.SetActive(level.Failed);

			// Let user pick the next level and start the level
//			await ObserveNextLevelInput().ToUniTask(canceller, true);
//			StartLevel(_levelIndex).Away();
		}

		IObservable<Unit> ObserveNextLevelInput()
		{
			return UniRxUtils.First(
				_retryLevelButton.OnClickAsObservable(),
				_nextLevelButton.OnClickAsObservable().Do(_ =>
				{
					_levelIndex += 1;
				}),
				_lastLevelButton.OnClickAsObservable().Do(_ =>
				{
					_levelIndex -= 1;
				}));
		}
	}
}