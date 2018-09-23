using UniRx.Async;

namespace BooAR.Apps
{
	public interface IAppController
	{
		UniTask LoadGame(string id);
		UniTask SaveGame(string id);
		void StartNewGame();
		void SetPause(bool pause);
	}
}