using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace BooAR.Characters
{
	public class Room : CharacterBase
	{
		public struct Param
		{
			public Grid Grid { get; set; }
		}

		public class Pool : MonoMemoryPool<Param, Room>
		{
			protected override void OnCreated(Room item)
			{
				base.OnCreated(item);
				item.OnCreated();
			}

			protected override void Reinitialize(Param p1, Room item)
			{
				base.Reinitialize(p1, item);
				item.OnSpawned(p1);
			}

			protected override void OnDespawned(Room item)
			{
				base.OnDespawned(item);
				item.OnDespawned();
			}
		}

		[Serializable]
		class DirectionalDoor
		{
#pragma warning disable 649
			[SerializeField]
			public Directions Direction;

			[SerializeField]
			public Door Door;
#pragma warning restore 649
		}

		[Serializable]
		class DirectionalWall
		{
#pragma warning disable 649
			[SerializeField]
			public Directions Direction;

			[SerializeField]
			public Collider Wall;
#pragma warning restore 649
		}

		[SerializeField, TableList]
		List<DirectionalDoor> _doors;

		[SerializeField, TableList(NumberOfItemsPerPage = 32)]
		List<DirectionalWall> _walls;

		[Inject]
		IPauseController _pauseController;

		Grid _grid;

		void OnSpawned(Param options)
		{
			name = $"Room ({options.Grid.X}, {options.Grid.Z})";

			OnSpawned();

			_grid = options.Grid;

			SetCollisionActive(false);

			// Initialize doors
			foreach (DirectionalDoor p in _doors)
			{
				p.Door.Initialize(_grid.X, _grid.Z, p.Direction);
			}

			// Pause the game when user exits off the wall
			foreach (DirectionalWall wall in _walls)
			{
				_pauseController
					.SubscribeWall(wall.Wall, wall.Direction.ToReverseVector())
					.AddTo(_lifeBag);
			}

			// Toggle rendering/collision when user enters/exits the room
			_world.ObservePlayerGridConnection(_grid)
			      .Subscribe(active => SetAllActive(active))
			      .AddTo(_lifeBag);

			// Toggle collision when user enters/exits the room
			_world.ObservePlayerEnterOrExit(_grid)
			      .Subscribe(sameGrid => SetCollisionActive(sameGrid))
			      .AddTo(_lifeBag);
		}

		void SetAllActive(bool active)
		{
			Log($"SetAllActive({active})");

			gameObject.SetActive(active);
		}

		void SetCollisionActive(bool active)
		{
			Log($"SetCollisionActive({active})");

			// enable/disable wall colliders when palyer moves between grids
			foreach (DirectionalWall wall in _walls)
			{
				wall.Wall.enabled = active;
			}
		}
	}
}