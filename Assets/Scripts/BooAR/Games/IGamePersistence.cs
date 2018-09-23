namespace BooAR.Games
{
	/* Save/load game state in the disk.
	 * Game state does not include AR positioning or voxel world data.
	 * In other words: game state ~= player data; =/= world data.
	 * Serialized data shouldn't (have to) be shared with other players thru network.
	 */
	public interface IGamePersistence
	{
		void Save(string dirPath);
		void Load(string dirPath);
	}
}