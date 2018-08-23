using System;

namespace BooAR
{
	public interface IGridWorld
	{
		IObservable<Grid> ObserveGridAdded { get; }
		IObservable<bool> ObservePlayerEnterOrExit(Grid grid);
		IObservable<bool> ObservePlayerGridConnection(Grid grid);
		IObservable<bool> ObserveGridConnection(Grid grid, Directions direction);
		
		void Initialize();
		void AddGrid(Grid grid);
		void UpdatePlayerGrid(Grid grid);
		void UpdateGridConnection(Grid fromGrid, Directions toDirection, bool connected);
	}
}