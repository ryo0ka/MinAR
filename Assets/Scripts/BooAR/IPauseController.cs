using System;
using UniRx;
using UnityEngine;
using Utils;

namespace BooAR
{
	public interface IPauseController
	{
		bool IsPaused { get; }
		Vector3 RecoveryPosition { get; }
		Vector3 RecoveryRotation { get; }

		// Invoked when pausing is intended. Observers should pause.
		IObservable<Unit> OnPaused { get; }

		// Invoked when resuming is intended. Observers should NOT resume yet.
		IObservable<Unit> OnResumeStarted { get; }

		// Invoekd when resuming actually takes place. Observers can resume now.
		IObservable<Unit> OnResumeCompleted { get; }

		IDisposable SubscribeWall(Collider wall, Vector3 normal);

		// Registers a pauser object with recovery transform.
		void Pause(Vector3 position, Vector3 rotation);
	}

	public static class PauseControllerUtils
	{
		public static IObservable<bool> OnPauseChanged(this IPauseController c)
		{
			return UniRxUtils
			       .MergeBool(c.OnPaused, c.OnResumeCompleted)
			       .StartWith(c.IsPaused)
			       .DistinctUntilChanged()
			       .Do(a => Debug.Log(a));
		}
	}
}