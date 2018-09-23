namespace BooAR.Voxel
{
	public interface IVoxelPersistence
	{
		void Save(string dirPath);
		void Load(string dirPath);
	}
}