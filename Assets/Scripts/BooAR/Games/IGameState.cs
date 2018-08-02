using System;
using BooAR.Levels;
using UniRx;
using UniRx.Async;

namespace BooAR.Games
{
	public enum GameStateTypes
	{
		GameInitializeStart,
		GameInitialize,
		LevelLoadStart,
		LevelStart,
		LevelEnd,
	}

	public interface IGameState
	{
		ILevelState Level { get; set; }
		int Index { get; set; }
		IReactiveProperty<GameStateTypes> State { get; }
		IReactiveProperty<bool> Pause { get; }
	}

	public static class GameStateUtils
	{
		static IObservable<Unit> OnStateChangedTo(this IGameState s, GameStateTypes stateType)
		{
			return s.State.Where(c => c == stateType).AsUnitObservable();
		}

		public static void SetState(this IGameState s, GameStateTypes stateType)
		{
			s.State.Value = stateType;
		}

		public static IObservable<UniTask<Unit>> OnInitializing(this IGameState s)
		{
			return s.OnStateChangedTo(GameStateTypes.GameInitializeStart)
			        .Select(_ => s.OnStateChangedTo(GameStateTypes.GameInitialize).ToUniTask(useFirstValue: true));
		}

		public static IObservable<Unit> OnInitialized(this IGameState s)
		{
			return s.OnStateChangedTo(GameStateTypes.GameInitialize);
		}

		public static IObservable<Tuple<int, UniTask<Unit>>> OnLevelLoading(this IGameState s)
		{
			return s.OnStateChangedTo(GameStateTypes.LevelLoadStart)
			        .Select(_ => s.OnStateChangedTo(GameStateTypes.LevelStart).ToUniTask(useFirstValue: true))
			        .Select(loader => Tuple.Create(s.Index, loader));
		}

		public static IObservable<ILevelState> OnLevelStarted(this IGameState s)
		{
			return s.OnStateChangedTo(GameStateTypes.LevelStart)
			        .Select(_ => s.Level);
		}

		public static IObservable<bool> OnLevelEnded(this IGameState s)
		{
			return s.OnStateChangedTo(GameStateTypes.LevelEnd)
			        .Select(_ => s.Level.IsGoaled());
		}

		public static IObservable<int> OnIndexChanged(this IGameState s)
		{
			return s.State.Select(_ => s.Index);
		}

		public static void SetPause(this IGameState s, bool pause)
		{
			s.Pause.Value = pause;
		}
	}
}