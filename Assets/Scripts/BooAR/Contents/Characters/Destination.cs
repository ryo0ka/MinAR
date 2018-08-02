using System;
using BooAR.Levels;
using BooAR.Players;
using UniRx;
using UnityEngine;
using Utils;
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

		[Inject]
		IPlayer _player;

		public Vector3 Position => transform.position;

		void Awake()
		{
			SetVisible(false);
		}

		public IObservable<Unit> OnPlayerReachedAsObservable()
		{
			SetVisible(true);

			return transform.ObserveReached(_player.Transform, _reachDistance)
			                .TakeUntil(_level.OnEnded())
			                .FirstOrDefault()
			                .DoOnEnd(() => SetVisible(false));
		}

		void SetVisible(bool visible)
		{
			_body.enabled = visible;
		}
	}
}