namespace BooAR.Voxel
{
	public interface IBlockParticlePresenter
	{
		void EmitDamage(Vector3i position, Vector3i face, byte block, float durability);
		void EmitPlacement(Vector3i position);
	}
}