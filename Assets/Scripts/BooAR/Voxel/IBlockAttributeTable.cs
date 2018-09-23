namespace BooAR.Voxel
{
	public interface IBlockAttributeTable
	{
		Visibilities GetVisibility(byte block);
		int GetDurability(byte block);
	}
}