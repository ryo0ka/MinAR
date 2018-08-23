using UniRx;
using UnityEngine;
using Zenject;

namespace BooAR.Characters
{
	public class Door : CharacterBase
	{
		[SerializeField]
		GameObject _body;

		[SerializeField]
		Collider _bodyCollider;

		[Inject]
		IPauseController _pauseController;

		bool _created;
		bool _inPlayerGrid;

		public Grid Grid { get; private set; }
		public Directions Direction { get; private set; }
		public bool IsOpen { get; private set; }

		public void Initialize(int x, int y, Directions d)
		{
			Initialize();
			Log($"Initialize({x}, {y}, {d})");
			
			name = $"Door ({x}, {y}, {d})";	
			Grid = (x, y);
			Direction = d;

			// Observe when player enters/exits the door's grid
			_world.ObservePlayerEnterOrExit(Grid)
			      .Subscribe(sameGrid => OnPlayerGridChanged(sameGrid))
			      .AddTo(_lifeBag);

			// Pause the game when player collides with (goes into) the door
			_pauseController
				.SubscribeWall(_bodyCollider, Direction.ToReverseVector())
				.AddTo(_lifeBag);

			// Open/close the door when this door's grid is connected to the player
			_world.ObserveGridConnection(Grid, Direction)
			      .Subscribe(open => SetOpen(open))
			      .AddTo(_lifeBag);
		}

		void OnPlayerGridChanged(bool sameGrid)
		{
			Log($"OnPlayerGridChanged({sameGrid})");

			_inPlayerGrid = sameGrid;
			_bodyCollider.enabled = _inPlayerGrid && !IsOpen;
		}

		public void SetOpen(bool open)
		{
			Log($"SetOpen({open})");

			IsOpen = open;
			_body.SetActive(!IsOpen);
			_bodyCollider.enabled = !open && _inPlayerGrid;
			_world.UpdateGridConnection(Grid, Direction, IsOpen);
		}
	}
}