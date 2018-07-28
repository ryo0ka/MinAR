using UnityEngine;

namespace Utils
{
	public static class HierarchyUtils
	{
		public static void SetParent(this Transform t, Transform parent, Vector3 localPosition)
		{
			t.SetParent(parent);
			t.localPosition = localPosition;
		}
	}
}