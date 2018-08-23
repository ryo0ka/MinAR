using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Characters
{
	public abstract class CharacterBase : BaseBehaviour
	{
		[Inject]
		protected IGameState _game;

		[Inject]
		protected IPlayer _player;

		[Inject]
		protected IGridWorld _world;

		// Any events must be added to this bag
		protected readonly CompositeDisposable _lifeBag = new CompositeDisposable();

		bool _initialized; // for not pooling

		protected Transform PlayerTransform => _player.Transform;
		protected float PlayerHandReach => _player.HandReach;
		protected Collider PlayerBody => _player.BodyReach;
		protected Collider PlayerCore => _player.Core;

		// For pooling
		protected virtual void OnCreated()
		{
			_lifeBag.AddTo(this);
		}

		// For pooling
		protected virtual void OnSpawned()
		{
			_game.On(GameEventTypes.Unloaded)
			     .Subscribe(_ => _lifeBag.Clear())
			     .AddTo(_lifeBag);

			_game.On(GameEventTypes.Failed)
			     .Subscribe(_ => OnGameFailed())
			     .AddTo(_lifeBag);

			_game.On(GameEventTypes.Goaled)
			     .Subscribe(_ => OnGameGoaled())
			     .AddTo(_lifeBag);

			_game.On(GameEventTypes.PlayerKilled)
			     .Subscribe(_ => OnPlayerKilled())
			     .AddTo(_lifeBag);
		}

		// For pooling
		protected virtual void OnDespawned()
		{
			_lifeBag.Clear();
		}

		// For general initialization (not pooling)
		protected void Initialize()
		{
			Assert(!_initialized);

			OnCreated();
			OnSpawned();
		}

		// For general finalization (not pooling)
		protected virtual void OnDestroy()
		{
			OnDespawned();
		}

		protected virtual void OnGameFailed()
		{
		}

		protected virtual void OnGameGoaled()
		{
		}

		protected virtual void OnPlayerKilled()
		{
		}
	}
}