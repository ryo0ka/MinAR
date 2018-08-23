using System;
using UniRx;
using Utils;

namespace BooAR
{
	public enum GameEventTypes
	{
		PlayerKilled,
		Failed,
		Goaled,
		Unloaded,
	}

	// for controlling/observing the game from inside
	public interface IGameState
	{
		IObservable<GameEventTypes> Events { get; }
	}

	public static class GameStateUtils
	{
		public static IObservable<Unit> On(this IGameState s, GameEventTypes type)
		{
			return s.Events.Where(e => e == type).AsUnitObservable();
		}

		public static IObservable<bool> OnEnded(this IGameState s)
		{
			return UniRxUtils.MergeBool(
				                 s.On(GameEventTypes.Goaled),
				                 s.On(GameEventTypes.Failed))
			                 .DistinctUntilChanged();
		}
	}
}