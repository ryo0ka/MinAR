using System;
using UniRx;
using UnityEngine;
using Utils;

namespace BooAR.Levels
{
	public struct FloorOptions
	{
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
		public Vector2 Size { get; }
	}

	public interface ILevelState
	{
		IReactiveProperty<FloorOptions?> Begun { get; }
		IReactiveProperty<bool> Goaled { get; }
		IReactiveProperty<bool> Failed { get; }
		IReactiveProperty<bool> Cancelled { get; }
	}

	public static class LevelStates
	{
		// ReSharper disable once PossibleInvalidOperationException
		public static FloorOptions Options(this ILevelState s) => s.Begun.Value.Value;
		public static bool IsGoaled(this ILevelState s) => s.Goaled.Value;
		public static bool IsFailed(this ILevelState s) => s.Failed.Value;

		public static void Begin(this ILevelState s, FloorOptions options)
		{
			s.Begun.Value = options;
			s.Goaled.Value = false;
			s.Failed.Value = false;
			s.Cancelled.Value = false;
		}

		public static void Goal(this ILevelState s) => s.Goaled.Value = true;
		public static void Fail(this ILevelState s) => s.Failed.Value = true;
		public static void Cancel(this ILevelState s) => s.Cancelled.Value = true;

		public static IObservable<Unit> OnFailed(this ILevelState s)
		{
			return s.Failed.WhereTrue();
		}

		public static IObservable<Unit> OnGoaled(this ILevelState s)
		{
			return s.Goaled.WhereTrue();
		}

		public static IObservable<Unit> OnCancelled(this ILevelState s)
		{
			return s.Cancelled.WhereTrue();
		}

		// Invoked when the level cannot continue
		public static IObservable<Unit> OnEnded(this ILevelState s)
		{
			return Observable.Merge(s.OnFailed(), s.OnGoaled(), s.OnCancelled()).First();
		}
	}
}