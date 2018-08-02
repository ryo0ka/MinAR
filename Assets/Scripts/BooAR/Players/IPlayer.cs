using System;
using UniRx;
using UnityEngine;

namespace BooAR.Players
{
	public interface IPlayer
	{
		Transform Transform { get; }
		IObservable<Unit> OnKilled { get; }

		void Kill();
	}
}