using UnityEngine;

namespace BooAR.Cameras
{
	[RequireComponent(typeof(Camera))]
	public class TextureCamera : MonoBehaviour
	{
		[SerializeField]
		string _textureName;
		
		Camera _camera;

		public RenderTexture Texture { get; private set; }

		void Awake()
		{
			_camera = GetComponent<Camera>();

			Texture = new RenderTexture(
				_camera.pixelWidth,
				_camera.pixelHeight,
				16);

			Texture.name = _textureName;

			_camera.targetTexture = Texture;
		}

		void OnEnable()
		{
			_camera.enabled = true;
		}

		void OnDisable()
		{
			_camera.enabled = false;
		}

		void OnDestroy()
		{
			Texture.Release();
		}
	}
}