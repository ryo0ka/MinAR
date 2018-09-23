using UniRx.Async;

namespace BooAR.ARs
{
	public interface IArWorldPersistence
	{
		UniTask Load(string dirPath);
		UniTask Save(string dirPath);
	}
}