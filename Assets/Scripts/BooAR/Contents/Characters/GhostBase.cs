using BooAR.Cameras;
using BooAR.Levels;
using BooAR.Players;
using UniRx;
using Utils;
using Zenject;

namespace BooAR.Contents.Characters
{
	public abstract class GhostBase : BaseBehaviour
	{
		[Inject]
		protected ILevelState _level;

		[Inject]
		protected IPlayer _player;

		[Inject]
		protected ICameraState _playerCamera;

		protected readonly CompositeDisposable _life = new CompositeDisposable();
		protected readonly CompositeDisposable _levelLife = new CompositeDisposable();

		protected virtual void OnCreated()
		{
			_life.AddTo(this);
			_levelLife.AddTo(this);
		}

		protected virtual void OnSpawned()
		{
			_level.OnFailed()
			      .SubscribeUnit(OnLevelFailed)
			      .AddTo(_life);

			_level.OnFailed()
			      .SubscribeUnit(OnLevelGoaled)
			      .AddTo(_life);

			_level.OnEnded()
			      .Subscribe(_ => _levelLife.Clear())
			      .AddTo(_life);
		}

		protected virtual void OnDespawned()
		{
			_life.Clear();
		}

		protected virtual void OnLevelFailed()
		{
		}

		protected virtual void OnLevelGoaled()
		{
		}
	}
}