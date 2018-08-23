namespace BooAR.Voxel
{
	public interface ITerrainGenerator
	{
		Blocks GetBlock(Vector3i chunkPosition, Vector3i blockPosition);
	}
}