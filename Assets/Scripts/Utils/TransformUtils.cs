using UnityEngine;

namespace Utils
{
	public static class TransformUtils
	{
		public static void SetLocalEulerAngles(this Transform t, float? x = null, float? y = null, float? z = null)
		{
			Vector3 a = t.localEulerAngles;
			a.x = x ?? a.x;
			a.y = y ?? a.y;
			a.z = z ?? a.z;
			t.localEulerAngles = a;
		}

		public static void SetEulerAngles(this Transform t, float? x = null, float? y = null, float? z = null)
		{
			Vector3 a = t.eulerAngles;
			a.x = x ?? a.x;
			a.y = y ?? a.y;
			a.z = z ?? a.z;
			t.eulerAngles = a;
		}

		public static void SetPosition(this Transform t, float? x = null, float? y = null, float? z = null)
		{
			Vector3 a = t.position;
			a.x = x ?? a.x;
			a.y = y ?? a.y;
			a.z = z ?? a.z;
			t.position = a;
		}
	}
}