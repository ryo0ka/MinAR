using System.Collections.Generic;
using UnityEngine;

namespace BooAR.Games.Persistences
{
	public class EditorGamePersistence : MonoBehaviour, IGamePersistence
	{
		[SerializeField]
		bool _unlockEveryLevel;
		
		[SerializeField]
		int _latestLevel;

		[SerializeField]
		List<int> goaledLevels;

		public int LatestLevel
		{
			get => _latestLevel;
			set => _latestLevel = value;
		}

		public bool HasGoaled(int index)
		{
			return _unlockEveryLevel || goaledLevels.Contains(index);
		}

		public void GoalLevel(int index)
		{
			if (!goaledLevels.Contains(index))
				goaledLevels.Add(index);
		}

		public void Save()
		{
			// pass
		}
	}
}