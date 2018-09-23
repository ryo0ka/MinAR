using Zenject;

namespace BooAR.Games
{
	public class GamePersistence : IGamePersistence
	{
#pragma warning disable 649
		[Inject]
		IInventory _inventory;
#pragma warning restore 649

		public void Save(string dirPath)
		{
			_inventory.Save(dirPath);
		}

		public void Load(string dirPath)
		{
			_inventory.Load(dirPath);
		}
	}
}