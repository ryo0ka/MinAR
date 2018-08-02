using UnityEngine;

namespace BooAR.Cameras
{
	public interface ICameraState
	{
		void SetToMainCamera();
		void SetToSubCamera(RenderTexture subCamera);
	}

	public static class CameraStates
	{
		public static void SetSubCameraActive(this ICameraState s, RenderTexture subCamera, bool active)
		{
			if (active) s.SetToSubCamera(subCamera);
			else s.SetToMainCamera();
		}
	}
}