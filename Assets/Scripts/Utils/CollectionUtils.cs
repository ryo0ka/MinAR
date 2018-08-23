using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
	public static class CollectionUtils
	{
		public static IEnumerable<(K, V)> ToTuples<K, V>(this IDictionary<K, V> d)
		{
			return d.Select(p => (p.Key, p.Value));
		}

		public static V GetOrAddValue<K, V>(this IDictionary<K, V> d, K key, Func<K, V> def)
		{
			return (d.TryGetValue(key, out V v)) ? v : d[key] = def(key);
		}

		public static V GetOrAddValue<V>(this IList<V> c, int index, Func<V> def)
		{
			return (c.Count < index) ? c[index] : c[index] = def();
		}
	}
}