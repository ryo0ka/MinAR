using System;
using UniRx;
using UniRx.Async;

namespace BooAR
{
	public enum AppStateTypes
	{
		AppInitializeStart,
		AppInitialize,
		GameInitializeStart,
		GameInitialize,
		GameEnd,
	}

	public interface IAppState
	{
		IReadOnlyReactiveProperty<AppStateTypes> State { get; }
	}

	public static class AppStateUtils
	{
		static IObservable<Unit> OnStateChangedTo(this IAppState s, AppStateTypes state, bool exclusive = false)
		{
			IObservable<Unit> o = s.State.Where(c => c == state).AsUnitObservable();
			if (exclusive) o = o.TakeUntil(s.State.Where(c => c != state));
			return o;
		}

		static IObservable<UniTask<Unit>> FromTo(this IAppState s, AppStateTypes from, AppStateTypes to)
		{
			IObservable<Unit> init = s.OnStateChangedTo(from);
			IObservable<Unit> last = s.OnStateChangedTo(to, true);
			return init.Select(_ => last.ToUniTask(useFirstValue: true));
		}

		public static IObservable<UniTask<Unit>> OnAppInitializing(this IAppState s)
		{
			return s.FromTo(AppStateTypes.AppInitializeStart, AppStateTypes.AppInitialize);
		}

		public static IObservable<UniTask<Unit>> OnGameLoading(this IAppState s)
		{
			return s.FromTo(AppStateTypes.GameInitializeStart, AppStateTypes.GameInitialize);
		}
	}
}