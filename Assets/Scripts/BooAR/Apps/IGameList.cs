using System.Collections.Generic;

namespace BooAR.Apps
{
	public interface IGameList
	{
		IEnumerable<string> GetGameIDs();
	}
}