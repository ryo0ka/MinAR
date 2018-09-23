using UnityEngine;
using Utils;

namespace BooAR.Games
{
	public class EditorCameraController : BaseBehaviour
	{
#if UNITY_EDITOR
		[SerializeField]
		float _lookSpeed;

		[SerializeField]
		float _moveSpeed;
		
		Vector2? _lastScreenPosition;

		void Update()
		{
			UpdateRotation();
			UpdatePosition();
		}

		void UpdateRotation()
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				Vector2 position = Input.mousePosition;
				Vector2 lastPosition = _lastScreenPosition ?? position;
				Vector2 delta = (position - lastPosition) * Time.smoothDeltaTime * _lookSpeed;
				
				Vector3 angles = transform.eulerAngles;
				angles.y += delta.x;
				angles.x += delta.y * -1;
				transform.eulerAngles = angles;

				_lastScreenPosition = position;
				
				//Log($"{lastPosition} -> {position} -- {delta}");
			}
			else
			{
				_lastScreenPosition = null;
			}
		}

		void UpdatePosition()
		{
			Vector3 delta = Vector3.zero;

			if (Input.GetKey(KeyCode.W)) delta.z += 1;
			if (Input.GetKey(KeyCode.A)) delta.x -= 1;
			if (Input.GetKey(KeyCode.S)) delta.z -= 1;
			if (Input.GetKey(KeyCode.D)) delta.x += 1;

			delta *= Time.deltaTime * _moveSpeed;

			Vector3 worldDelta = transform.TransformVector(delta);
			transform.position += worldDelta;
		}
#endif
	}
}