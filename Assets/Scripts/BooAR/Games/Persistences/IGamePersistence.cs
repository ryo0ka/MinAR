namespace BooAR.Games.Persistences
{
	public interface IGamePersistence
	{
		int LatestLevel { get; set; }
		bool HasGoaled(int index);
		void GoalLevel(int index);
		void Save();
	}
}