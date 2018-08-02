using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zenject;

namespace BooAR.Games.Persistences
{
	public class GamePersistence : IGamePersistence
	{
		[Serializable]
		class Data
		{
#pragma warning disable 649
			[SerializeField]
			int _latestLevel;

			[SerializeField]
			List<int> _clearedLevels;
#pragma warning restore 649

			public int LatestLevel
			{
				get => _latestLevel;
				set => _latestLevel = value;
			}

			public List<int> ClearedLevels =>
				_clearedLevels ?? (_clearedLevels = new List<int>());
		}

		const string FileName = "save.json";
		string _path;
		Data _data;

		public int LatestLevel
		{
			get => _data.LatestLevel;
			set => _data.LatestLevel = value;
		}

		[Inject]
		void OnInjected()
		{
			Debug.Log("GamePersistence.OnInjected() started");
			
			_path = Path.Combine(Application.persistentDataPath, FileName);
			//Debug.Log(_path);

			if (!File.Exists(_path))
			{
				//Debug.Log("GamePersistence.OnInjected() file created");
				File.Create(_path);
				_data = new Data();
			}
			else
			{
				_data = JsonUtility.FromJson<Data>(File.ReadAllText(_path));
			}

			Debug.Log("GamePersistence.OnInjected() finished");
		}

		public bool HasGoaled(int index)
		{
			Debug.Log(_data != null);
			return _data.ClearedLevels.Contains(index);
		}

		public void GoalLevel(int index)
		{
			if (!_data.ClearedLevels.Contains(index))
				_data.ClearedLevels.Add(index);
		}

		public void Save()
		{
			File.WriteAllText(_path, JsonUtility.ToJson(_data));
		}
	}
}