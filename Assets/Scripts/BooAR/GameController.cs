using System;
using System.Threading;
using BooAR.ARs;
using BooAR.Characters;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR
{
	// facade of everything
	public class GameController : BaseBehaviour, IGameController, IGameState
	{
		[SerializeField]
		Transform _root;

		// x: horizontal, y: vertical
		[SerializeField]
		Vector2 _gridSize;

		[Inject]
		Room.Pool _roomPool;

		[Inject]
		IFloorController _floorPrompt;

		[Inject]
		IPauseController _pauseController;

		[Inject]
		IPlayer _player;

		[Inject]
		IGridWorld _world;

		readonly Subject<GameEventTypes> _events = new Subject<GameEventTypes>();
		readonly CompositeDisposable _bag = new CompositeDisposable();

		public IObservable<GameEventTypes> Events => _events;

		public async UniTask Initialize()
		{
			_bag.Clear();
			_world.Initialize();

			// Spawn a new room for a new grid
			_world.ObserveGridAdded
			      .Subscribe(grid => OnGridAdded(grid))
			      .AddTo(_bag);

			// Update player grid in physics cycle (supporting pausing)
			Observable
				.EveryFixedUpdate()
				.Subscribe(_ => UpdatePlayerGrid())
				.AddTo(_bag);

			_pauseController
				.OnPauseChanged()
				.Subscribe(paused => SetActive(!paused))
				.AddTo(_bag);

			// Set up floor plan
			Floor floor = await _floorPrompt.Prompt(CancellationToken.None);
			_root.position = floor.Position;
			_root.SetEulerAngles(0f, floor.Rotation, 0f);

			_world.AddGrid((0, 0));
		}

		public async UniTask Unload()
		{
			_bag.Clear();
			_world.Initialize();

			await UniTask.CompletedTask;
		}

		void OnGridAdded(Grid grid)
		{
			Log($"OnGridAdded({grid})");

			// Fit the room in the grid
			Room newRoom = _roomPool.Spawn(new Room.Param {Grid = grid});
			newRoom.transform.SetParent(_root);
			newRoom.transform.localPosition = GridUtils.GridToPosition(_gridSize, grid);
		}

		void UpdatePlayerGrid()
		{
			Vector3 playerPos = _player.Transform.position;
			Grid playerGrid = GridUtils.PositionToGrid(_gridSize, playerPos);
			_world.UpdatePlayerGrid(playerGrid);
		}

		void SetActive(bool active)
		{
			Log($"SetActive({active})");
			_root.gameObject.SetActive(active);
		}
	}
}