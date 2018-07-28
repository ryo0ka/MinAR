using UnityEngine.UI;

namespace Utils
{
	public static class UiUtils 
	{
		public static void Toggle(this Toggle t)
		{
			t.isOn = !t.isOn;
		}
	}
}