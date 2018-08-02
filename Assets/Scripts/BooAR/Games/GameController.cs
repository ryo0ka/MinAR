using System;
using BooAR.Cameras;
using BooAR.Games.Persistences;
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
		IGameState _game;

		[Inject]
		IGamePersistence _persistence;

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
		Toggle _pauseToggle;

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
				InitializeGame().Away();
			});

			// Let user pick the next level and start the level
			LevelSelectionAsObservable().Subscribe(index =>
			{
				_levelIndex = index;
				StartLevel(_levelIndex).Away();
			});

			// Ensure user can't start nonexistent levels
			_game.OnIndexChanged().Subscribe(index =>
			{
				_lastLevelButton.interactable = index > 0f;
				_nextLevelButton.interactable = (index + 1) < _levels.Count;

				// next level is only accessible with current level goaled
				_nextLevelButton.interactable &= _persistence.HasGoaled(index);
			});

			// Save persistence on application quit
			Observable.OnceApplicationQuit().Subscribe(_ =>
			{
				_persistence.Save();
			});

			// Toggle pause
			_pauseToggle.isOn = false;
			_pauseToggle.OnValueChangedAsObservable().Subscribe(_game.SetPause);

			InitializeGame().Away();
		}

		async UniTask InitializeGame()
		{
			_level.Clear();

			_game.SetState(GameStateTypes.GameInitializeStart);

			await _levels.UnloadAll();

			_game.SetState(GameStateTypes.GameInitialize);

			_levelIndex = _persistence.LatestLevel;
			_levelOptions = await _layoutPrompter.PromptLayout();

			StartLevel(_levelIndex).Away();
		}

		async UniTask StartLevel(int index)
		{
			_level.Clear();

			_game.Index = index;
			_game.SetState(GameStateTypes.LevelLoadStart);

			await _levels.UnloadAll();

			// Load and start the level
			ILevelState level = await _levels.Initialize(index);

			try
			{
				level.Begin(_levelOptions);

				_game.Level = level;
				_game.SetState(GameStateTypes.LevelStart);

				_persistence.LatestLevel = index;

				await level.OnEnded().ToUniTask(_level.CancellationToken(), true);

				_game.SetState(GameStateTypes.LevelEnd);

				if (level.IsGoaled())
				{
					_persistence.GoalLevel(index);

					_levelIndex = Math.Min(index + 1, _levels.Count - 1);
					StartLevel(_levelIndex).Away();
				}
			}
			catch (OperationCanceledException e)
			{
				Log($"Cancelled level ({index}): {e}");
				level.Cancel();
			}
		}

		// Observe level-selecting buttons and stream out the next index
		IObservable<int> LevelSelectionAsObservable()
		{
			return Observable.Merge(
				_retryLevelButton.OnClickAsObservable().Select(_ => _levelIndex),
				_nextLevelButton.OnClickAsObservable().Select(_ => _levelIndex + 1),
				_lastLevelButton.OnClickAsObservable().Select(_ => _levelIndex - 1));
		}
	}
}