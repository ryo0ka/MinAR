using UnityEngine;

namespace BooAR.Cameras
{
	public interface ICameraController
	{
		void SetToMainCamera();
		void SetToSubCamera(RenderTexture subCamera);
	}

	public static class CameraControllerUtils
	{
		public static void SetSubCameraActive(this ICameraController s, RenderTexture subCamera, bool active)
		{
			if (active) s.SetToSubCamera(subCamera);
			else s.SetToMainCamera();
		}
	}
}