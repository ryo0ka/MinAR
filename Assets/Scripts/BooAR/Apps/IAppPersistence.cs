using UniRx.Async;

namespace BooAR.Apps
{
	public interface IAppPersistence
	{
		UniTask SaveAll(string id);
		UniTask LoadAll(string id);
	}
}