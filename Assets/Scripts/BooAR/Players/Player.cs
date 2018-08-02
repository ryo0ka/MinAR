using System;
using UniRx;
using UnityEngine;
using Utils;

namespace BooAR.Players
{
	[SelectionBase]
	public class Player : BaseBehaviour, IPlayer
	{
		[SerializeField]
		Transform _player;

		[SerializeField]
		bool _invincible;

		readonly Subject<Unit> _onKilled = new Subject<Unit>();

		public Transform Transform => _player;
		public IObservable<Unit> OnKilled => _onKilled;

		public void Kill()
		{
			if (!_invincible && Debug.isDebugBuild)
				_onKilled.OnNext();
		}
	}
}