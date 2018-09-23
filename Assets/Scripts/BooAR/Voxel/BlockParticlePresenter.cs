using UnityEngine;
using Zenject;

namespace BooAR.Voxel
{
	public class BlockParticlePresenter : IBlockParticlePresenter
	{
		[Inject]
		BlockParticleSystem.Pool _pool;

		public void EmitDamage(Vector3i position, Vector3i face, byte block, float durability)
		{
			BlockParticleSystem particles = _pool.Spawn(new BlockParticleSystem.Params
			{
				Index = block,
			});

			// Set position of the particle system, ASSUMING it's parented to the voxel world.
			Vector3 localPosition = position + Vector3.one / 2f + face / 2f;
			particles.transform.localPosition = localPosition;
			particles.Emit(durability > 0f, () => _pool.Despawn(particles));
		}

		public void EmitPlacement(Vector3i position)
		{
			BlockParticleSystem particles = _pool.Spawn(new BlockParticleSystem.Params
			{
				Index = 16 * 16 - 1 - 5,
			});
			
			Vector3 localPosition = position + Vector3.one / 2f;
			particles.transform.localPosition = localPosition;
			particles.Emit(true, () => _pool.Despawn(particles));
		}
	}
}