using System;
using System.Collections.Generic;

namespace Utils
{
	public static class LinqUtils
	{
		public static void ForEach<T>(this IEnumerable<T> ens, Action<T> f)
		{
			foreach (T e in ens)
			{
				f(e);
			}
		}

		public static void ForEachWithIndex<T>(this IEnumerable<T> ens, Action<T, int> f)
		{
			int index = 0;
			foreach (T e in ens)
			{
				f(e, index);
				index += 1;
			}
		}

		public static string ToStringPretty<T>(this IEnumerable<T> ts)
		{
			return "[" + string.Join(", ", ts) + "]";
		}
	}
}