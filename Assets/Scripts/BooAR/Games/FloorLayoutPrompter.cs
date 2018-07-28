using System.Collections.Generic;
using BooAR.Levels;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using Utils;

namespace BooAR.Games
{
	public class FloorLayoutPrompter : BaseBehaviour
	{
		[SerializeField]
		Camera _camera;

		[SerializeField]
		GameObject _screenRoot;

		[SerializeField]
		Button _finishButton;

		[SerializeField]
		GameObject _floorNotFoundIndicator;

		[SerializeField]
		Transform _floorPlan;

		[SerializeField]
		Vector3 _editorPosition;

		readonly ARHitTestResultType[] _hitTypes =
		{
			ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent,
			ARHitTestResultType.ARHitTestResultTypeFeaturePoint,
		};

		void Awake()
		{
			_floorPlan.gameObject.SetActive(false);
			_screenRoot.SetActive(false);
		}

		public async UniTask<FloorOptions> PromptLayout()
		{
			Log("Prompt()");

			FloorOptions options = new FloorOptions();

			using (UniRxUtils.Toggle(_floorPlan.gameObject.SetActive))
			using (UniRxUtils.Toggle(_screenRoot.SetActive))
			{
				// Let the floor plan follow the camera position on the AR floor
				this.LateUpdateAsObservable()
				    .TakeUntil(_finishButton.OnClickAsObservable())
				    .Subscribe(_ =>
				    {
					    Vector3? arFloorPosition = TryFindArFloor();

					    // Accept input only when AR floor is found
					    _finishButton.interactable = arFloorPosition.HasValue;
					    _floorPlan.gameObject.SetActive(arFloorPosition.HasValue);
					    _floorNotFoundIndicator.SetActive(!arFloorPosition.HasValue);

					    if (!arFloorPosition.HasValue) return;

					    // Floor plan should follow the camera position on the AR floor
					    _floorPlan.SetPosition(
						    x: _camera.transform.position.x,
						    y: arFloorPosition.Value.y,
						    z: _camera.transform.position.z);

					    // Floor plan should rotate to the camera's direction
					    _floorPlan.SetEulerAngles(
						    x: 0f,
						    y: _camera.transform.eulerAngles.y,
						    z: 0f);
				    });

				// Wait until the user picks the position
				await _finishButton.OnClickAsObservable().First();
			}

			_floorPlan.gameObject.SetActive(false);

			options.Position = _floorPlan.position;
			options.Rotation = _floorPlan.eulerAngles;

			Log("Prompt() finished");

			return options;
		}

		Vector3? TryFindArFloor()
		{
			//Logger.Debug($"Raycast()");

			if (Application.isEditor)
			{
				return _editorPosition;
			}
			else
			{
				Vector2 viewportPoint = _camera.ScreenToViewportPoint(Vector2.one * .5f);
				ARPoint arPoint = new ARPoint {x = viewportPoint.x, y = viewportPoint.y};

				foreach (ARHitTestResultType hitType in _hitTypes)
				{
					IEnumerable<ARHitTestResult> results =
						UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(arPoint, hitType);

					foreach (ARHitTestResult hit in results)
					{
						return UnityARMatrixOps.GetPosition(hit.worldTransform);
					}
				}

				return null;
			}
		}
	}
}