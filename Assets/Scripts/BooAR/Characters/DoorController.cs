using System;
using BooAR.Haptics;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Characters
{
	[RequireComponent(typeof(Door))]
	public class DoorController : CharacterBase
	{
		[SerializeField]
		Collider _knobCollider;

		[SerializeField]
		EventTrigger _knob;

		[Inject]
		DoorButtonPool _buttonPool;

		[Inject]
		IHapticFeedbackGenerator _haptic;

		readonly CompositeDisposable _buttonBag = new CompositeDisposable();
		Door _door;
		Button _button;
		bool _knobInteractable;

		void Start()
		{
			Initialize();
			_door = GetComponent<Door>();
			_buttonBag.AddTo(_lifeBag);

			IObservable<bool> observePlayerEnterOrExit =
				_world.ObservePlayerEnterOrExit(_door.Grid)
				      .PublishWithRefCount();

			observePlayerEnterOrExit
				.Subscribe(sameGrid => OnPlayerGridChanged(sameGrid))
				.AddTo(_lifeBag);

			// room connection update
			_knob.OnEventTriggerdAsObservable(EventTriggerType.PointerClick)
			     .Do(_ => Log("Knob clicked"))
			     .Where(_ => _knobInteractable)
			     .Subscribe(_ => _door.SetOpen(!_door.IsOpen))
			     .AddTo(_lifeBag);

			IObservable<bool> knobInteractable =
				this.UpdateAsObservable()
				    .Select(_ => _player.VisibleAndReachable(_knobCollider.transform.position))
				    .Mask(observePlayerEnterOrExit)
				    .StartWith(false) // for CombineLatest
				    .DistinctUntilChanged()
				    .PublishWithRefCount();

			// knob interactable
			knobInteractable
				.Subscribe(SetKnobInteractable)
				.AddTo(_lifeBag);

			IObservable<bool> buttonInteractable =
				PlayerBody
					.OnTriggerEnterOrExitAsObservable(_knobCollider)
					.StartWith(false) // for CombineLatest
					.CombineLatest(knobInteractable, (b, k) => b && !k) // shouldn't coexist
					.Mask(observePlayerEnterOrExit)
					.DistinctUntilChanged()
					.PublishWithRefCount();

			// button interactable
			buttonInteractable
				.Subscribe(SetButtonInteractable)
				.AddTo(_lifeBag);
		}

		void OnPlayerGridChanged(bool sameGrid)
		{
			Log($"OnPlayerGridChanged({sameGrid})");

			_knobCollider.enabled = sameGrid;

			if (!sameGrid)
			{
				SetButtonInteractable(false);
			}
		}

		void SetKnobInteractable(bool interactable)
		{
			Log($"SetKnobInteractable({interactable})");

			_knobInteractable = interactable;

			_haptic.TriggerSelected(interactable);
		}

		void SetButtonInteractable(bool interactable)
		{
			Log($"SetButtonInteractable({interactable})");

			_buttonBag.Clear();

			if (interactable)
			{
				_button = _buttonPool.SpawnTo(_buttonBag);
				_button.OnClickAsObservable()
				       .Subscribe(_ => _door.SetOpen(!_door.IsOpen))
				       .AddTo(_buttonBag);
			}

			_haptic.TriggerSelected(interactable);
		}
	}
}