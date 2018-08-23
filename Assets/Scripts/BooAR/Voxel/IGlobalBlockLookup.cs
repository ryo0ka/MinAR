namespace BooAR.Voxel
{
	public interface IGlobalBlockLookup
	{
		Lookup? Lookup(Vector3i blockPosition);
	}
}