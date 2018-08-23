using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.iOS;
using Utils;

namespace BooAR.ARs
{
	public class FloorController : BaseBehaviour, IFloorController
	{
		[SerializeField]
		Camera _camera;

		[SerializeField]
		Button _finishButton;

		[SerializeField]
		Vector3 _editorPosition;

		readonly ARHitTestResultType[] _hitTypes =
		{
			ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent,
			ARHitTestResultType.ARHitTestResultTypeFeaturePoint,
		};

		readonly ReactiveProperty<bool> _prompting = new ReactiveProperty<bool>();
		readonly ReactiveProperty<Floor?> _current = new ReactiveProperty<Floor?>();
		Vector3? _lastCameraPosition;

		public IReadOnlyReactiveProperty<bool> IsPrompting => _prompting;
		public IObservable<Floor?> Current => _current;

		public async UniTask<Floor> Prompt(CancellationToken canceller)
		{
			Log("Prompt()");

			using (CompositeDisposable bag = new CompositeDisposable())
			{
				// Set `IsPrompting` to True on start and False at the end
				UniRxUtils.Toggle(o => _prompting.Value = o).AddTo(bag);

				// Let the floor plan follow the camera position on the AR floor
				this.LateUpdateAsObservable()
				    .Subscribe(_ => UpdateFloor())
				    .AddTo(bag);

				_current.Select(c => c.HasValue)
				        .SubscribeToInteractable(_finishButton)
				        .AddTo(bag);

				// Wait until the user picks the position
				await _finishButton.ObserveNext(canceller);
			}

			canceller.ThrowIfCancellationRequested();

			Log("Prompt() finished");

			// ReSharper disable once PossibleInvalidOperationException
			return _current.Value.Value;
		}

		void UpdateFloor()
		{
			//Logger.Debug($"Raycast()");

			float angle = _camera.transform.eulerAngles.y;

			if (Application.isEditor)
			{
				_current.Value = new Floor(_editorPosition, angle);
			}
			else if (HitTest(_camera, _hitTypes, out Floor floor))
			{
				_current.Value = floor;
				_lastCameraPosition = _camera.transform.position;
			}
			else if (_current.Value.HasValue && _lastCameraPosition.HasValue)
			{
				Vector3 lastFloorPosition = _current.Value.Value.Position;
				Vector3 lastCameraPosition = _lastCameraPosition.Value;
				Vector3 currentCameraPosition = _camera.transform.position;
				Vector3 currentCameraRotation = _camera.transform.eulerAngles;

				// keep the same height
				// keep the same horizontal distance
				float distance = MovementUtils.HorizontalDistance(lastFloorPosition, lastCameraPosition);
				Vector3 floorPosition = currentCameraPosition + currentCameraRotation.normalized * distance;
				floorPosition.y = lastFloorPosition.y;

				_current.Value = new Floor(floorPosition, angle);
			}
		}

		bool HitTest(Camera arCamera, IEnumerable<ARHitTestResultType> priority, out Floor result)
		{
			Vector2 viewportPoint = arCamera.ScreenToViewportPoint(Vector2.one / 2f);
			ARPoint arPoint = new ARPoint {x = viewportPoint.x, y = viewportPoint.y};

			foreach (ARHitTestResultType type in priority)
			{
				IEnumerable<ARHitTestResult> results =
					UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(arPoint, type);

				foreach (ARHitTestResult hit in results)
				{
					Vector3 position = UnityARMatrixOps.GetPosition(hit.worldTransform);
					result = new Floor(position, _camera.transform.eulerAngles.y);
					return true;
				}
			}

			result = default(Floor);
			return false;
		}
	}
}