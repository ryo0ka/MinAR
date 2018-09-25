using UniRx.Async;

namespace BooAR.Apps
{
	public interface IAppPersistence
	{
		UniTask Save(string id);
		UniTask Load(string id);
	}
}