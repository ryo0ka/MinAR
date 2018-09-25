using System;
using System.Collections.Generic;
using UniRx;

namespace BooAR.Apps
{
	public interface IGameList
	{
		IObservable<Unit> OnChanged { get; }
		
		IEnumerable<string> GetGameIDs();
		void DeleteGame(string id);
	}
}