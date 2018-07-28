using BooAR.Levels;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Contents.Characters
{
	[SelectionBase]
	public class GhostBoo : GhostBase
	{
		public struct Params
		{
			public Transform Player { get; set; }
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
		IReadOnlyReactiveProperty<float> _playerVelocity;

		Transform _player;

		protected override void OnCreated()
		{
			base.OnCreated();

			_sneak.AddTo(this);
		}

		void OnSpawned(Params options)
		{
			base.OnSpawned();

			_player = options.Player;

			_playerVelocity =
				_player.ObservePositionVelocity()
				       .TakeUntil(_level.OnEnded())
				       .ToReadOnlyReactiveProperty()
				       .AddTo(_life);

			// Start or stop moving to the player
			// when the player rotates
			this.UpdateAsObservable()
			    .TakeUntil(_level.OnEnded())
			    .Select(_ => IsLookedAway())
			    .DistinctUntilChanged()
			    .Subscribe(OnMovableChanged)
			    .AddTo(_life);

			// Kill player when this ghost reaches it
			transform.ObserveReached(_player, _reachDistance)
			         .TakeUntil(_level.OnEnded())
			         .SubscribeUnit(OnReached)
			         .AddTo(_life);

			_level.OnFailed.SubscribeUnit(OnLevelFailed).AddTo(this);
			_level.OnGoaled.SubscribeUnit(OnLevelGoaled).AddTo(this);
		}

		protected override void OnLevelFailed()
		{
			_sneak.Clear();
		}

		protected override void OnLevelGoaled()
		{
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
				// Update position
				this.UpdateAsObservable()
				    .Subscribe(_ => UpdateSneakingTransform())
				    .AddTo(_sneak)
				    .AddTo(_life);
			}
		}

		void OnReached()
		{
			_level.Fail();
		}

		bool IsLookedAway()
		{
			return !ContentsUtils.HorizontalLookedAt(
				_player.forward,
				_player.position,
				transform.position,
				_thresholdAngle);
		}

		void UpdateSneakingTransform()
		{
			// Boo's speed should depend on player's speed
			// so that "outrunning" is not an option for player,
			// but should have a minimum speed
			// so that boo will move even if player is standing still.
			float chaseVelocity = _playerVelocity.Value * _playerSpeedCoeff;
			float velocity = Mathf.Max(_minVelocity, chaseVelocity);

			transform.position = Vector3.MoveTowards(
				transform.position,
				_player.position,
				velocity * Time.deltaTime);

			transform.LookAt(_player);
		}
	}
}