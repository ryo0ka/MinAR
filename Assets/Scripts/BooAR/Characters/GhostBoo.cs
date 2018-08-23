using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Characters
{
	[SelectionBase]
	public class GhostBoo : CharacterBase
	{
		public struct Params
		{
			public Grid Grid { get; set; }
		}

		// Use this pool to manage ghosts' lifecycle
		public class Pool : MemoryPool<Params, GhostBoo>
		{
			protected override void OnCreated(GhostBoo item)
			{
				base.OnCreated(item);
				item.OnCreated();
			}

			protected override void Reinitialize(Params p1, GhostBoo item)
			{
				base.Reinitialize(p1, item);
				item.OnSpawned(p1);
			}

			protected override void OnDespawned(GhostBoo item)
			{
				base.OnDespawned(item);
				item.OnDespawned();
			}
		}

		[SerializeField]
		float _reachDistance;

		[SerializeField]
		float _thresholdAngle;

		[SerializeField]
		float _minVelocity;

		[SerializeField]
		float _playerSpeedCoeff;

		readonly CompositeDisposable _sneak = new CompositeDisposable();
		readonly Subject<Unit> _onReached = new Subject<Unit>();

		public IObservable<Unit> OnReachedAsObservable => _onReached;

		protected override void OnCreated()
		{
			base.OnCreated();

			_sneak.AddTo(_lifeBag);
			_onReached.AddTo(this);
		}

		void OnSpawned(Params options)
		{
			base.OnSpawned();

			// Start or stop moving to the player
			// when the player rotates
			this.UpdateAsObservable()
			    .Select(_ => !IsLookedAt())
			    .DistinctUntilChanged()
			    .Subscribe(OnMovableChanged)
			    .AddTo(_lifeBag);

			// Kill player when this ghost reaches it
			transform.ProximityAsObservable(PlayerTransform)
			         .First(p => p < _reachDistance)
			         .Subscribe(_ => _onReached.OnNext())
			         .AddTo(_lifeBag);
		}

		protected override void OnGameFailed()
		{
			base.OnGameFailed();
			_sneak.Clear();
		}

		protected override void OnGameGoaled()
		{
			base.OnGameGoaled();
			_sneak.Clear();
		}

		protected override void OnDespawned()
		{
			base.OnDespawned();

			_sneak.Clear();
		}

		void OnMovableChanged(bool movable)
		{
			Debug.Log($"OnMovableChanged({movable})");

			_sneak.Clear();

			if (movable)
			{
				// Update boo's position
				Observable.EveryFixedUpdate()
				          .Select(_ => PlayerTransform.position)
				          .ToVelocity()
				          .Subscribe(UpdateSneakingTransform)
				          .AddTo(_sneak)
				          .AddTo(_lifeBag);
			}
		}

		bool IsLookedAt()
		{
			return MovementUtils.HorizontalLookedAt(
				PlayerTransform.forward,
				PlayerTransform.position,
				transform.position,
				_thresholdAngle);
		}

		void UpdateSneakingTransform(float playerVelocity)
		{
			// Boo's speed should depend on player's speed
			// so that "outrunning" is not an option for player,
			// but should have a minimum speed
			// so that boo will move even if player is standing still.
			playerVelocity = Mathf.Max(_minVelocity, playerVelocity);

			transform.position = Vector3.MoveTowards(
				transform.position,
				PlayerTransform.position,
				playerVelocity * _playerSpeedCoeff);

			transform.LookAt(PlayerTransform);
		}
	}
}