using System;
using System.Threading;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Utils;

namespace BooAR.ARs
{
	public struct Floor
	{
		public Vector3 Position { get; }
		public float Rotation { get; }

		public Floor(Vector3 position, float rotation)
		{
			Position = position;
			Rotation = rotation;
		}
	}

	public interface IFloorController
	{
		IReadOnlyReactiveProperty<bool> IsPrompting { get; }
		IObservable<Floor?> Current { get; }

		UniTask<Floor> Prompt(CancellationToken canceller);
	}

	public static class FloorControllerUtils
	{
		public static IObservable<UniTask<Unit>> OnPromptStarted(this IFloorController p)
		{
			IObservable<Unit> onStart = p.IsPrompting.WhereTrue();
			IObservable<Unit> onEnd = p.IsPrompting.WhereFalse();
			return onStart.Select(_ => onEnd.ToUniTask(useFirstValue: true));
		}
	}
}