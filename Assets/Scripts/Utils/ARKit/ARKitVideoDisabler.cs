using UnityEngine;
using UnityEngine.XR.iOS;

namespace Utils.ARKit
{
	[RequireComponent(typeof(UnityARVideo), typeof(Camera))]
	public class ARKitVideoDisabler : MonoBehaviour
	{
		void Awake()
		{
#if UNITY_EDITOR
			GetComponent<UnityARVideo>().enabled = false;
			Camera cam = GetComponent<Camera>();
			cam.clearFlags = CameraClearFlags.Skybox;
#endif
		}
	}
}