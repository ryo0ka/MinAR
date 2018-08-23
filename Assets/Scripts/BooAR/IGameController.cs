using UniRx.Async;

namespace BooAR
{
	// for controlling the game from outside
	public interface IGameController
	{
		UniTask Initialize();
		UniTask Unload();
	}
}