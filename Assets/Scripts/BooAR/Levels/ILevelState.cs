using System;
using UniRx;
using UnityEngine;

namespace BooAR.Levels
{
	public struct FloorOptions
	{
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
	}

	public interface ILevelState
	{
		FloorOptions Options { get; }
		bool Goaled { get; }
		bool Failed { get; }

		IObservable<Unit> OnGoaled { get; }
		IObservable<Unit> OnFailed { get; }

		void Begin(FloorOptions options);
		void Goal();
		void Fail();
	}

	public static class Levels
	{
		// Invoked when the level cannot continue
		public static IObservable<Unit> OnEnded(this ILevelState s)
		{
			return Observable.Merge(s.OnFailed, s.OnGoaled).First();
		}
	}
}