using UnityEngine;

namespace BooAR.Cameras
{
	public interface ICameraState
	{
		void SetToMainCamera();
		void SetToSubCamera(RenderTexture subCamera);
	}
}