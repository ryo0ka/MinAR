using System;
using BooAR.Cameras;
using BooAR.Levels;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Contents.Characters
{
	public class GhostSiren : GhostBase
	{
		public struct Params
		{
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
		TextureCamera _camera;

		[SerializeField]
		GameObject _body;

		[SerializeField]
		float _reach;

		Toggle _jackToggle;

		readonly Subject<Unit> _onReached = new Subject<Unit>();

		public IObservable<Unit> OnReachedPlayerAsObservable => _onReached;

		public IObservable<Unit> OnJackedAsObservable =>
			_jackToggle.OnValueChangedAsObservable()
			           .WhereTrue()
			           .TakeUntil(_level.OnEnded());

		protected override void OnCreated()
		{
			base.OnCreated();

			_onReached.AddTo(this);
		}

		void OnSpawned(Params param)
		{
			base.OnSpawned();

			transform.ObserveReached(_player.Transform, _reach)
			         .SubscribeUnit(_onReached.OnNext)
			         .AddTo(_life)
			         .AddTo(_levelLife);

			Observable.EveryUpdate()
			          .Subscribe(_ => UpdateTransform())
			          .AddTo(_life)
			          .AddTo(_levelLife);

			// Delay creation of toggle UI until this moment
			_jackToggle = _jackTogglePool.Spawn(_life);
			_jackToggle.isOn = false;
			_jackToggle.OnValueChangedAsObservable()
			           .Subscribe(SetJackActive)
			           .AddTo(_life)
			           .AddTo(_levelLife);
		}

		void SetJackActive(bool render)
		{
			_camera.enabled = render;
			_playerCamera.SetSubCameraActive(_camera.Texture, render);
		}

		void UpdateTransform()
		{
			_camera.transform.LookAt(_player.Transform);
		}

		[Button]
		void ToggleVisible()
		{
			_body.SetActive(!_body.activeSelf);
		}
	}
}