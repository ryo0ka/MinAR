using System;
using UniRx;
using UnityEngine;
using Utils;
using Sirenix.OdinInspector;

namespace BooAR.Levels
{
	public class LevelState : BaseBehaviour, ILevelState
	{
		readonly Subject<Unit> _onFailed = new Subject<Unit>();
		readonly Subject<FloorOptions> _onStarted = new Subject<FloorOptions>();
		readonly Subject<Unit> _onGoaled = new Subject<Unit>();

		[SerializeField, ReadOnly]
		bool _goaled;

		[SerializeField, ReadOnly]
		bool _failed;

		public bool Failed => _failed;
		public bool Goaled => _goaled;

		public IObservable<Unit> OnGoaled => _onGoaled;
		public IObservable<Unit> OnFailed => _onFailed;
		public FloorOptions Options { get; private set; }

		void Awake()
		{
			_onFailed.AddTo(this);
			_onStarted.AddTo(this);
			_onGoaled.AddTo(this);
		}

		public void Begin(FloorOptions options)
		{
			Log($"Initialize({options})");

			Options = options;
			_goaled = false;
			_failed = false;
			_onStarted.OnNext(options);
		}

		public void Goal()
		{
			Log("Clear()");
			_goaled = true;
			_onGoaled.OnNext();
		}

		public void Fail()
		{
			Log("KillPlayer()");
			_failed = true;
			_onFailed.OnNext();
		}
	}
}