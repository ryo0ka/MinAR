using BooAR.Cameras;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace BooAR.Contents.Characters
{
	public class GhostSiren : GhostBase
	{
		public struct Params
		{
			public Transform Player { get; set; }
		}

		public class Pool : MonoMemoryPool<Params, GhostSiren>
		{
			protected override void OnCreated(GhostSiren item)
			{
				base.OnCreated(item);
				item.OnCreated();
			}

			protected override void Reinitialize(Params param, GhostSiren item)
			{
				base.Reinitialize(param, item);
				item.OnSpawned(param);
			}

			protected override void OnDespawned(GhostSiren item)
			{
				base.OnDespawned(item);
				item.OnDespawned();
			}
		}

		[SerializeField]
		TextureCamera _camera;

		[SerializeField]
		GameObject _body;

		[SerializeField]
		float _reach;
		
		Transform _player;

		public RenderTexture View => _camera.Texture;

		void OnSpawned(Params param)
		{
			base.OnSpawned();
			
			_player = param.Player;

			transform.ObserveReached(_player, _reach)
			         .Subscribe(_ => OnReached())
			         .AddTo(_life);

			this.UpdateAsObservable()
			    .Subscribe(_ => UpdateTransform())
			    .AddTo(_life);
		}

		void OnReached()
		{
			_level.Fail();
		}

		public void SetActiveRendering(bool render)
		{
			_camera.enabled = render;
		}

		void UpdateTransform()
		{
			_camera.transform.LookAt(_player);
		}

		[Button]
		void ToggleVisible()
		{
			_body.SetActive(!_body.activeSelf);
		}
	}
}