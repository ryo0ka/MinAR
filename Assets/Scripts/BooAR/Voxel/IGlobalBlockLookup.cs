namespace BooAR.Voxel
{
	public interface IGlobalBlockLookup
	{
		BlockLookup? Lookup(Vector3i blockPosition);
	}
}