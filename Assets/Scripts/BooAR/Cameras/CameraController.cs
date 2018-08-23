using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace BooAR.Cameras
{
	public class CameraController : BaseBehaviour, ICameraController
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