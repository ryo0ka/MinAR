using UnityEngine;

namespace Utils
{
	public static class UnityUtils
	{
		public static void SetEnabled(this Behaviour c, bool enabled)
		{
			c.enabled = enabled;
		}
	}
}