using BooAR.Levels;
using UniRx;
using Utils;
using Zenject;

namespace BooAR.Contents.Characters
{
	public abstract class GhostBase : BaseBehaviour
	{
		[Inject]
		protected ILevelState _level;

		protected readonly CompositeDisposable _life = new CompositeDisposable();

		protected virtual void OnCreated()
		{
			_life.AddTo(this);
		}

		protected virtual void OnSpawned()
		{
			_level.OnFailed
			      .SubscribeUnit(OnLevelFailed)
			      .AddTo(this);

			_level.OnGoaled
			      .SubscribeUnit(OnLevelGoaled)
			      .AddTo(this);
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