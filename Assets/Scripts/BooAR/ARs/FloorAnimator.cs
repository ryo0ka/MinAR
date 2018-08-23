using UniRx;
using UniRx.Async;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.ARs
{
	public class FloorAnimator : MonoBehaviour
	{
		[Inject]
		IFloorController _controller;

		[SerializeField]
		GameObject _floorNotFoundIndicator;

		[SerializeField]
		Transform _floorPlan;

		[SerializeField]
		GameObject _screenRoot;

		void Start()
		{
			SetVisible(false);

			_controller.OnPromptStarted()
			           .SubscribeAway(c => OnPromptStarted(c));
		}

		async UniTask OnPromptStarted(UniTask onPromptCompleted)
		{
			SetVisible(true);

			_controller.Current
			           .SampleFrame(1)
			           .TakeUntil(onPromptCompleted.ToObservable())
			           .Subscribe(OnFloorChanged);

			await onPromptCompleted;

			SetVisible(false);
		}

		void OnFloorChanged(Floor? floor)
		{
			_floorPlan.gameObject.SetActive(floor.HasValue);
			_floorNotFoundIndicator.SetActive(!floor.HasValue);

			if (floor.HasValue)
			{
				// Floor plan should follow the camera position on the AR floor
				_floorPlan.position = floor.Value.Position;

				// Floor plan should rotate to the camera's direction
				_floorPlan.eulerAngles = new Vector3(0f, floor.Value.Rotation, 0f);
			}
		}

		void SetVisible(bool visible)
		{
			_floorPlan.gameObject.SetActive(visible);
			_screenRoot.SetActive(visible);
		}
	}
}