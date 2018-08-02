using System;
using UniRx.Async;

namespace BooAR.Levels
{
	public interface ILevelCollection
	{
		int Count { get; }
		int? Current { get; }
		
		UniTask<ILevelState> Initialize(int index);
		UniTask UnloadAll();
	}
}