namespace BooAR.Voxel
{
	public interface IBlockDamagePresenter
	{
		void ResetDamage();
		void UpdateHealth(Vector3i position, float health);
	}
}