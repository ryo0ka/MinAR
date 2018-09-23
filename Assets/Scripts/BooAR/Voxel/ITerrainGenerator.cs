namespace BooAR.Voxel
{
	public interface ITerrainGenerator
	{
		byte GenerateBlock(Vector3i position);
	}
}