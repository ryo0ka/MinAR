using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Voxel
{
	public class BlockActionHandler : BaseBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		float _durableTime = .5f;

		[Inject]
		IBlockDamagePresenter _surfaces;

		[Inject]
		IBlockParticlePresenter _particles;
#pragma warning restore 649

		// Current block state
		Vector3i? _position;
		float _health;
		float? _lastDamageTime;

		void Update()
		{
			if (_lastDamageTime.HasValue)
			{
				float pastTime = Time.time - _lastDamageTime.Value;
				if (pastTime > _durableTime)
				{
					Initialize();
				}
			}
		}

		public void Initialize()
		{
			_position = null;
			_health = 1f;
			_lastDamageTime = null;

			_surfaces.ResetDamage();
		}

		public bool Damage(Vector3i position, Vector3i face, byte block, float damage)
		{
			Debug.Assert(damage >= 0f);

			if (position == _position)
			{
				_health -= damage;
			}
			else
			{
				_position = position;
				_health = 1f - damage;
			}

			_health = Mathf.Clamp01(_health);

			_particles.EmitDamage(position, face, block, _health);

			if (_health == 0f)
			{
				Initialize();
				return true;
			}
			else
			{
				_lastDamageTime = Time.time;
				_surfaces.UpdateHealth(position, _health);
				return false;
			}
		}

		public void Place(Vector3i position)
		{
			Initialize();
			_particles.EmitPlacement(position);
		}
	}
}