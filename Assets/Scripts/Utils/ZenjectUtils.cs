using System;
using System.Collections.Generic;
using Zenject;

namespace Utils
{
	public static class ZenjectUtils
	{
		public static T SpawnTo<T>(this IMemoryPool<T> p, ICollection<IDisposable> disposer)
		{
			T instance = p.Spawn();
			disposer.Add(() => p.Despawn(instance));
			return instance;
		}
		
		public static T Spawn<T, U>(this IMemoryPool<U, T> p, U param, ICollection<IDisposable> disposer)
		{
			T instance = p.Spawn(param);
			disposer.Add(() => p.Despawn(instance));
			return instance;
		}
	}
}