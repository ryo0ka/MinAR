using UnityEngine.UI;

namespace Utils
{
	public static class UiUtils 
	{
		public static void Toggle(this Toggle t)
		{
			t.isOn = !t.isOn;
		}

		public static Toggle StartWith(this Toggle t, bool isOn)
		{
			t.isOn = isOn;
			return t;
		}
	}
}