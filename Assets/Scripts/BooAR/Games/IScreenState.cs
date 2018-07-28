using System;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BooAR.Games
{
	public interface IScreenState
	{
		void AddComponent(Transform component);
	}

	public static class ScreenStates
	{
		public static IDisposable Instantiate<T>(
			this IScreenState s,
			T prefab,
			out T component) where T : Behaviour
		{
			component = Object.Instantiate(prefab);
			s.AddComponent(component.transform);
			GameObject componentObject = component.gameObject;
			return Disposable.Create(() => Object.Destroy(componentObject));
		}
	}
}