using UnityEngine;
using UnityEngine.UI;

namespace BooAR.Cameras
{
	public class CameraState : BaseBehaviour, ICameraState
	{
		[SerializeField]
		TextureCamera _mainCamera;

		[SerializeField]
		RawImage _targetImage;

		public void SetToMainCamera()
		{
			Log("SetToMainCamera()");

			_targetImage.texture = _mainCamera.Texture;
			_mainCamera.enabled = true;
		}

		public void SetToSubCamera(RenderTexture subCamera)
		{
			Log("SetToSubCamera()");

			_mainCamera.enabled = false;
			_targetImage.texture = subCamera;
		}
	}
}