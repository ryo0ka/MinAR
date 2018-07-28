using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Players
{
	public class PlayerController : BaseBehaviour
	{
		[Inject]
		IPlayer _player;

		[SerializeField]
		KeyCode _editorRotateKey;

		void Start()
		{
#if UNITY_EDITOR
			this.UpdateAsObservable()
			    .Where(_ => Input.GetKeyDown(_editorRotateKey))
			    .Subscribe(_ => RotatePlayer())
			    .AddTo(this);
#endif
		}

		void RotatePlayer()
		{
			_player.Transform.SetLocalEulerAngles(
				y: _player.Transform.localEulerAngles.y + 180f);
		}
	}
}