using UnityEngine;

namespace Utils
{
	public class UnityIOBridge : MonoBehaviour
	{
		public static string persistentDataPath { get; private set; }
		
		void Awake()
		{
			persistentDataPath = Application.persistentDataPath;
		}
	}
}