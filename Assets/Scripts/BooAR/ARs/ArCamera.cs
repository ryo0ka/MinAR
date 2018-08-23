using UnityEngine;

namespace BooAR.ARs
{
	public class ArCamera : MonoBehaviour, IArCamera
	{
		[SerializeField]		
		Camera _camera;

		public Camera Camera => _camera;
	}
}