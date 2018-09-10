namespace BooAR.Voxel
{
	public interface IBlockAttributeTable
	{
		Visibilities GetVisibility(Blocks block);
		int GetDurability(Blocks block);
	}
}