using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Utils
{
	public static class UnityUtils
	{
		public static void SetEnabled(this Behaviour c, bool enabled)
		{
			c.enabled = enabled;
		}

		public static void DestroyGameObject(Component component)
		{
			Object.Destroy(component.gameObject);
		}

		public static void SetActive(this IEnumerable<GameObject> os, bool active)
		{
			foreach (GameObject gameObject in os)
			{
				gameObject.SetActive(active);
			}
		}

		class EndSampleDisposable : IDisposable
		{
			public static readonly EndSampleDisposable Instance = new EndSampleDisposable();
			public void Dispose() => Profiler.EndSample();
		}

		public static IDisposable Sample(string message)
		{
			Profiler.BeginSample(message);
			return EndSampleDisposable.Instance;
		}
	}
}