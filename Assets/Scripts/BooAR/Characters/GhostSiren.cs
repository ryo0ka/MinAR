using System;
using BooAR.Cameras;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Characters
{
	public class GhostSiren : CharacterBase
	{
		public struct Params
		{
			public Grid Grid { get; set; }
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

		[Inject]
		JackTogglePool _jackTogglePool;

		[SerializeField]
		TextureCamera _sirenCamera;

		[SerializeField]
		GameObject _body;

		[SerializeField]
		float _reach;

		Toggle _jackToggle;

		readonly Subject<Unit> _onReached = new Subject<Unit>();
		public IObservable<Unit> OnReachedPlayerAsObservable => _onReached;

		readonly Subject<Unit> _onJacked = new Subject<Unit>();
		public IObservable<Unit> OnJackedAsObservable => _onJacked;

		public RenderTexture ViewTexture => _sirenCamera.Texture;

		protected override void OnCreated()
		{
			base.OnCreated();

			_onReached.AddTo(this);
			_onJacked.AddTo(this);
		}

		void OnSpawned(Params options)
		{
			base.OnSpawned();

			transform.ProximityAsObservable(PlayerTransform)
			         .First(t => t < _reach)
			         .Subscribe(_ => _onReached.OnNext())
			         .AddTo(_lifeBag);

			Observable.EveryUpdate()
			          .Subscribe(_ => UpdateTransform())
			          .AddTo(_lifeBag);

			// Delay creation of toggle UI until this moment
			_jackToggle = _jackTogglePool.SpawnTo(_lifeBag);
			_jackToggle.isOn = false;
			_jackToggle.OnValueChangedAsObservable()
			           .Subscribe(SetJackActive)
			           .AddTo(_lifeBag);
		}

		void SetJackActive(bool render)
		{
			_sirenCamera.enabled = render;
			_onJacked.OnNext();
		}

		void UpdateTransform()
		{
			_sirenCamera.transform.LookAt(PlayerTransform);
		}

		[Button]
		void ToggleVisible()
		{
			_body.SetActive(!_body.activeSelf);
		}
	}
}