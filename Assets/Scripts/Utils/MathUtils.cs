using System;
using UnityEngine;

namespace Utils
{
	public static class MathUtils
	{
		public static Vector2 Horizontal(Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static int Clamp(int v, int min, int max)
		{
			return Math.Min(Math.Max(v, min), max);
		}

		public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
		{
			return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
		}
	}
}