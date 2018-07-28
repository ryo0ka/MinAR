using UnityEngine;

namespace Utils
{
	public static class MathUtils
	{
		public static Vector2 Horizontal(Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}
	}
}