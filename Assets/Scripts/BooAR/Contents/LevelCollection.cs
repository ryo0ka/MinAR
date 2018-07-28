using System;
using System.Collections.Generic;
using System.Linq;
using BooAR.Levels;
using Sirenix.OdinInspector;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace BooAR.Contents
{
	public class LevelCollection : BaseBehaviour, ILevelCollection
	{
		[Serializable]
		class SceneReference
		{
#pragma warning disable 649
			[SerializeField, TableColumnWidth(50)]
			int _index;

			[SerializeField]
			string _sceneName;
#pragma warning restore 649

			public int Index => _index;
			public string SceneName => _sceneName;
		}

		[SerializeField, TableList]
		List<SceneReference> _scenes;

		[SerializeField, ReadOnly]
		int _current;

		bool _processing;
		bool _loaded;

		public int Count => _scenes.Count;
		public int? Current => _loaded ? _current : (int?) null;

		void Start()
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				string sceneName = SceneManager.GetSceneAt(i).name;
				if (sceneName != "Main")
				{
					SceneManager.UnloadSceneAsync(sceneName)
					            .ToUniTask()
					            .Away();
				}
			}
		}

		public async UniTask<ILevelState> Initialize(int index)
		{
			Log($"Initialize({index})");

			await UniTask.WaitUntil(() => !_processing);

			using (UniRxUtils.Toggle(o => _processing = o))
			using (UniRxUtils.Finally(() => _loaded = true))
			{
				_current = index;
				string sceneName = GetSceneName(index);

				// Load the scene...
				await UniTask.Yield(PlayerLoopTiming.PostLateUpdate);
				await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

				return FindObjectOfType<LevelScene>().LevelState;
			}
		}

		public async UniTask UnloadAll()
		{
			Log("Unload()");

			if (!_loaded)
			{
				Log("No level is loaded");
				return;
			}

			await UniTask.WaitUntil(() => !_processing);

			using (UniRxUtils.Toggle(o => _processing = o))
			using (UniRxUtils.Finally(() => _loaded = false))
			{
				string sceneName = GetSceneName(_current);
				await SceneManager.UnloadSceneAsync(sceneName);
			}
		}

		string GetSceneName(int index)
		{
			return _scenes.First(l => l.Index == index).SceneName;
		}
	}
}