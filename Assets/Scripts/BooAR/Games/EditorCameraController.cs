using UnityEngine;
using Utils;

namespace BooAR.Games
{
	public class EditorCameraController : BaseBehaviour
	{
		Vector2? _lastPosition;

#if UNITY_EDITOR
		void Update()
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				Vector2 position = Input.mousePosition;
				Vector2 lastPosition = _lastPosition ?? position;
				Vector2 delta = (position - lastPosition) * .1f;
				
				Vector3 angles = transform.eulerAngles;
				angles.y += delta.x;
				angles.x += delta.y * -1;
				transform.eulerAngles = angles;

				_lastPosition = position;
				
				//Log($"{lastPosition} -> {position} -- {delta}");
			}
			else
			{
				_lastPosition = null;
			}
		}
#endif
	}
}