using UnityEngine;
using Utils;

#if UNITY_EDITOR
namespace BooAR
{
	public class EditorPlayerController : BaseBehaviour
	{
		[SerializeField]
		Transform _player;

		[SerializeField]
		float _speed;

		[SerializeField]
		float _angle;

		Vector2 _lastPos;

		void Update()
		{
			UpdatePosition();
			UpdateRotation();
		}

		void UpdatePosition()
		{
			Vector3 w = Input.GetKey(KeyCode.W) ? Vector3.forward : Vector3.zero;
			Vector3 a = Input.GetKey(KeyCode.A) ? Vector3.left : Vector3.zero;
			Vector3 s = Input.GetKey(KeyCode.S) ? Vector3.back : Vector3.zero;
			Vector3 d = Input.GetKey(KeyCode.D) ? Vector3.right : Vector3.zero;

			Vector3 delta = (w + a + s + d) * _speed * Time.unscaledDeltaTime;
			delta = _player.TransformVector(delta); // from local to world
			delta.y = 0f;
			_player.Translate(delta, Space.World);
		}

		void UpdateRotation()
		{
			Vector2 screen = new Vector2(Screen.width, Screen.height);
			Vector2 mouse = (Vector2) Input.mousePosition - screen / 2f;
			Vector2 pos = new Vector2(mouse.x / screen.x, mouse.y / screen.x);

			if (Input.GetKey(KeyCode.LeftShift))
			{
				Vector2 delta = (pos - _lastPos) * _angle;
				_player.localEulerAngles += new Vector3(delta.y * -1f, delta.x, 0);
			}

			_lastPos = pos;
		}
	}
}
#endif