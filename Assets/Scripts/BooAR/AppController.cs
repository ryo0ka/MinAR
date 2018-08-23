using BooAR.Cameras;
using BooAR.Persistences;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR
{
	public class AppController : BaseBehaviour, IAppState
	{
		[Inject]
		IPersistenceController _persistence;

		[Inject]
		IGameController _gameController;

		[Inject]
		IGameState _gameState;

		[Inject]
		ICameraController _camera;

		[SerializeField]
		Button _exitButton;

		readonly CompositeDisposable _gameBag = new CompositeDisposable();
		readonly Subject<bool> _interactable = new Subject<bool>();
		readonly ReactiveProperty<AppStateTypes> _state = new ReactiveProperty<AppStateTypes>();

		public IReadOnlyReactiveProperty<AppStateTypes> State => _state;

		void Start()
		{
			_persistence.Load();

			Observable.OnceApplicationQuit().Subscribe(_ =>
			{
				// Save persistence on application quit
				_persistence.Save();
			});

			// Pressing the exit button should re-initialize the game anytime
			IReactiveCommand<Unit> exitCommand = _interactable.ToReactiveCommand();
			exitCommand.BindTo(_exitButton);
			exitCommand.SubscribeAway(_ => InitializeGame());

			InitializeGame().Away();
		}

		async UniTask InitializeGame()
		{
			_gameBag.Clear();

			_interactable.OnNext(false);
			
			_camera.SetToMainCamera();

			SetState(AppStateTypes.AppInitializeStart);

			await _gameController.Unload();

			SetState(AppStateTypes.AppInitialize);
			SetState(AppStateTypes.GameInitializeStart);

			// Load and start the level
			await _gameController.Initialize();

			SetState(AppStateTypes.GameInitialize);

			_interactable.OnNext(true);

			// Wait until the game ends
			await _gameState.OnEnded().ToUniTask(_gameBag.CancellationToken(), true);

			SetState(AppStateTypes.GameEnd);

			_gameBag.Clear();
		}

		void SetState(AppStateTypes state)
		{
			_state.Value = state;
		}
	}
}