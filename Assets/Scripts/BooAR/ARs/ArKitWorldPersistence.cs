using System.IO;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.XR.iOS;
using Utils;
using Zenject;

namespace BooAR.ARs
{
	public class ArKitWorldPersistence : IArWorldPersistence
	{
#pragma warning disable 649
		[Inject]
		UnityARCameraManager _camera;
#pragma warning restore 649

		public UniTask Save(string dirPath)
		{
			Subject<Unit> saver = new Subject<Unit>();

			UnityARSessionNativeInterface
				.GetARSessionNativeInterface()
				.GetCurrentWorldMapAsync(worldMap =>
				{
					if (worldMap != null)
					{
						string path = GetPath(dirPath);
						worldMap.Save(path);
						Debug.Log($"ARWorldMap saved to {path}");
					}
					else
					{
						Debug.LogWarning($"ARWorldMap save failed to {dirPath}");
					}

					saver.OnNext();
				});

			return saver.First().ToUniTask();
		}

		public UniTask Load(string dirPath)
		{
			string path = GetPath(dirPath);
			ARWorldMap worldMap = ARWorldMap.Load(path);
			Debug.Log($"ARWorldMap loaded from {path}");

			ARKitWorldTrackingSessionConfiguration config = _camera.sessionConfiguration;
			config.worldMap = worldMap;
			
			const UnityARSessionRunOption runOption =
				UnityARSessionRunOption.ARSessionRunOptionRemoveExistingAnchors |
				UnityARSessionRunOption.ARSessionRunOptionResetTracking;
			
			UnityARSessionNativeInterface
				.ARSessionShouldAttemptRelocalization = true;
			
			UnityARSessionNativeInterface
				.GetARSessionNativeInterface()
				.RunWithConfigAndOptions(config, runOption);
			
			Debug.Log("Restarted session with worldMap");

			return UniTask.CompletedTask;
		}

		string GetPath(string dirPath)
		{
			return Path.Combine(dirPath, "foo.worldmap");
		}
	}
}