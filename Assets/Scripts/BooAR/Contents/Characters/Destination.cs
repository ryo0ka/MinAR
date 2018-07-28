using System;
using BooAR.Levels;
using UniRx;
using UnityEngine;
using Zenject;

namespace BooAR.Contents.Characters
{
	public class Destination : BaseBehaviour
	{
		[SerializeField]
		MeshRenderer _body;

		[SerializeField]
		float _reachDistance;

		[Inject]
		ILevelState _level;

		void Awake()
		{
			SetVisible(false);
		}

		public IObservable<Unit> WaitUntilReached(Transform player)
		{
			SetVisible(true);

			return transform.ObserveReached(player, _reachDistance)
			                .TakeUntil(_level.OnEnded())
			                .DoOnCompleted(() => SetVisible(false))
			                .DoOnError(_ => SetVisible(false))
			                .DoOnCancel(() => SetVisible(false));
		}

		void SetVisible(bool visible)
		{
			_body.enabled = visible;
		}
	}
}