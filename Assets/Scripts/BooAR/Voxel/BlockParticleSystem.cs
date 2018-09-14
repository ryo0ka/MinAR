using System;
using UniRx;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Voxel
{
	public class BlockParticleSystem : BaseBehaviour
	{
		public struct Params
		{
			public int Index { get; set; }
		}

		public class Pool : MonoMemoryPool<Params, BlockParticleSystem>
		{
			protected override void OnCreated(BlockParticleSystem item)
			{
				base.OnCreated(item);
				item.OnCreated();
			}

			protected override void Reinitialize(Params param, BlockParticleSystem item)
			{
				base.Reinitialize(param, item);
				item.OnSpawned(param);
			}

			protected override void OnDespawned(BlockParticleSystem item)
			{
				base.OnDespawned(item);
				item.OnDespawned();
			}
		}

		[SerializeField]
		ParticleSystem _particles;

		[SerializeField]
		ParticleSystemRenderer _renderer;

		readonly CompositeDisposable _life = new CompositeDisposable();

		void OnCreated()
		{
			_life.AddTo(this);
			_renderer.material = Instantiate(_renderer.material);
		}

		void OnSpawned(Params p)
		{
			_renderer.material.SetFloat(VoxelConsts.BlockIndexKey, p.Index);
		}

		void OnDespawned()
		{
			_life.Clear();
		}

		public void Emit(bool half, Action onComplete)
		{
			Emit((half) ? 5 : 10);

			// Wait until the particle animation is finished, then invoke `onComplete`.
			Observable
				.Timer(_particles.main.duration.Seconds())
				.Subscribe(_ => onComplete?.Invoke())
				.AddTo(_life);
		}

		void Emit(int maxParticles)
		{
			ParticleSystem.MainModule main = _particles.main;
			main.maxParticles = maxParticles;
			_particles.Emit(0);
		}
	}
}