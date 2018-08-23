using System;
using System.Collections.Generic;
using UniRx;
using Utils;

namespace BooAR
{
	public class GridWorld : IGridWorld
	{
		class V
		{
			public bool PlayerConnected { get; set; }
		}

		readonly Dictionary<Grid, V> _grids = new Dictionary<Grid, V>();
		readonly Dictionary<(Grid, Directions), bool> _gridConnections = new Dictionary<(Grid, Directions), bool>();
		readonly ReactiveProperty<Grid> _player = new ReactiveProperty<Grid>();
		readonly Subject<Unit> _gridConnectionUpdates = new Subject<Unit>();
		readonly Subject<Unit> _playerConnectionUpdates = new Subject<Unit>();
		readonly Subject<Grid> _addedGrids = new Subject<Grid>();

		public IObservable<Grid> ObserveGridAdded => _addedGrids;

		bool IsConnected(Grid grid, Directions direction)
		{
			return _gridConnections.TryGetValue((grid, direction), out bool connected) && connected;
		}

		bool IsConnectedToPlayer(Grid grid)
		{
			return _grids[grid].PlayerConnected;
		}

		public IObservable<bool> ObservePlayerEnterOrExit(Grid grid)
		{
			return _player
			       .Select(g => g.Equals(grid))
			       .DistinctUntilChanged();
		}

		public IObservable<bool> ObservePlayerGridConnection(Grid grid)
		{
			return _playerConnectionUpdates
			       .Select(_ => IsConnectedToPlayer(grid))
			       .StartWith(IsConnectedToPlayer(grid))
			       .DistinctUntilChanged();
		}

		public IObservable<bool> ObserveGridConnection(Grid grid, Directions direction)
		{
			return _gridConnectionUpdates
			       .Select(_ => IsConnected(grid, direction))
			       .StartWith(IsConnected(grid, direction))
			       .DistinctUntilChanged();
		}

		public void Initialize()
		{
			_grids.Clear();
			_gridConnections.Clear();
		}

		public void AddGrid(Grid grid)
		{
			if (_grids.ContainsKey(grid)) return;

			_grids[grid] = new V();
			_addedGrids.OnNext(grid);
		}

		public void UpdatePlayerGrid(Grid grid)
		{
			_player.Value = grid;
			UpdatePlayerConnection();
		}

		public void UpdateGridConnection(Grid toGrid, Directions fromDirection, bool connected)
		{
			(Grid fromGrid, Directions toDirection) = GridUtils.Mirror(toGrid, fromDirection);

			_gridConnections[(toGrid, fromDirection)] = connected;
			_gridConnections[(fromGrid, toDirection)] = connected;

			if (connected)
			{
				AddGrid(toGrid);
				AddGrid(fromGrid);
			}

			_gridConnectionUpdates.OnNext();
			UpdatePlayerConnection();
		}

		void UpdatePlayerConnection()
		{
			// List all the rooms that are connected to the player's room via open doors
			ISet<Grid> activeGrids = GridUtils.Connected(_player.Value, IsConnected);

			foreach ((Grid grid, V v) in _grids.ToTuples())
			{
				v.PlayerConnected = activeGrids.Contains(grid);
			}

			_playerConnectionUpdates.OnNext();
		}
	}
}