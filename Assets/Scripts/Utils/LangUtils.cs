using System;

namespace Utils
{
	public static class LangUtils
	{
		public static T[] EnumValues<T>()
		{
			return (T[]) Enum.GetValues(typeof(T));
		}

		public static TimeSpan Seconds(this float duration)
		{
			return TimeSpan.FromSeconds(duration);
		}

		public static void ForSelf<T>(this T t, Action<T> f)
		{
			f(t);
		}

		public static U MapSelf<T, U>(this T t, Func<T, U> f)
		{
			return f(t);
		}
	}
}