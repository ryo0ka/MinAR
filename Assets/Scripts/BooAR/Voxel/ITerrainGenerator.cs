namespace BooAR.Voxel
{
	public interface ITerrainGenerator
	{
		Blocks GenerateBlock(Vector3i position);
	}
}