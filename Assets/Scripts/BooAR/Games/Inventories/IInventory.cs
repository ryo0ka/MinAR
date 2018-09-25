using System;
using UniRx;

namespace BooAR.Games.Inventories
{
	public interface IInventory
	{
		IObservable<Blocks> OnCountChanged { get; }
		IObservable<Unit> OnReplaced { get; }

		bool Has(Blocks block);
		int GetCount(Blocks block);

		void Initialize();
		void Add(Blocks block);
		void Substract(Blocks block);

		void Save(string dirPath);
		void Load(string dirPath);
	}
}