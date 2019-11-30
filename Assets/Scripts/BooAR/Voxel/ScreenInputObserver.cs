using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BooAR.Voxel
{
	public class ScreenInputObserver : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
	{
		readonly Subject<Unit> _onTap = new Subject<Unit>();
		readonly Subject<Unit> _onHold = new Subject<Unit>();
		bool _holding;

		public IObservable<Unit> OnSingleTapped => _onTap;
		public IObservable<Unit> OnHoldEveryFrame => _onHold;

		void Awake()
		{
			_onTap.AddTo(this);
			_onHold.AddTo(this);
		}

		void Update()
		{
			if (_holding)
			{
				_onHold.OnNext(Unit.Default);
			}
		}

		void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
		{
			_onTap.OnNext(Unit.Default);
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			_holding = true;
			Debug.Log("down");
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			_holding = false;
			Debug.Log("up");
		}
	}
}