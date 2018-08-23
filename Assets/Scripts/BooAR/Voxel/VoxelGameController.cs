using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using UnityEngine.UI;

namespace BooAR.Voxel
{
	public class VoxelGameController : BaseBehaviour
	{
		[SerializeField]
		Camera _camera;

		[SerializeField]
		Transform _player;

		[SerializeField]
		Button _screen;

		[SerializeField]
		VoxelWorld _voxels;

		[SerializeField]
		string _worldId;

		[SerializeField]
		BlockInventory _inventory;

		[SerializeField]
		int _bodyExtent;

		Blocks? _placedBlock;

		void Start()
		{
			_voxels.PopulateInitialBlocks();

			// Player position update
			_player.UpdateAsObservable()
			       .Select(_ => _player.position.RoundToInt3())
			       .DistinctUntilChanged()
			       .Subscribe(p => OnPlayerPositionChanged(p));

			// Raycast update
			_screen.OnClickAsObservable()
			       .Merge(_screen.OnDragAsObservable().AsUnitObservable())
			       .Subscribe(r => OnRaycast(GetRay()));

			// Connect with block inventory
			_inventory.OnBlockSelected.Subscribe(block => _placedBlock = block);
			_inventory.OnPickaxeSelected.Subscribe(_ => _placedBlock = null);
			_inventory.OnBlockRunOut.Subscribe(_ => _placedBlock = null);
		}

		void OnPlayerPositionChanged(Vector3i position)
		{
			using (UnityUtils.Sample("VoxelWorld.OnPlayerPositionChanged()"))
			{
				Vector3i p = _voxels.WorldToVoxel(position);
				Debug.Log($"{position} -> {p}");

				_voxels.SetBlock(p, Blocks.Empty);

				for (int x = p.x - _bodyExtent; x < p.x + _bodyExtent; x++)
				for (int y = p.y - _bodyExtent; y < p.y + _bodyExtent; y++)
				for (int z = p.z - _bodyExtent; z < p.z + _bodyExtent; z++)
				{
					_voxels.SetBlock((x, y, z), Blocks.Empty);
				}
			}
		}

		Ray GetRay()
		{
			Ray r = _camera.ScreenPointToRay(Input.mousePosition);
			r.origin = _voxels.WorldToVoxel(r.origin);
			return r;
		}

		void OnRaycast(Ray ray)
		{
			using (UnityUtils.Sample("VoxelWorldController.OnRaycast()"))
			{
				VoxelRaycast.Raycast(ray.origin, ray.direction, 10f, null, (position, face) =>
				{
					Blocks block = _voxels.GetBlockOrInit(position);
					if (block == Blocks.Empty)
					{
						return false;
					}

					OnBlockSelected(position, block, face);
					return true; // End the tracing
				});
			}
		}

		void OnBlockSelected(Vector3i position, Blocks block, Vector3i face)
		{
			if (_placedBlock == null) // picking a selected block
			{
				_voxels.SetBlock(position, Blocks.Empty);
				_inventory.Add(block);
			}
			else // placing a block from inventory
			{
				Blocks placedBlock = _placedBlock.Value;

				_voxels.SetBlock(position + face, placedBlock);
				_inventory.Substract(block);
			}
		}

		[Button]
		void Save()
		{
			_voxels.Save(_worldId);
		}

		[Button]
		void Load()
		{
			_voxels.Load(_worldId);
		}

		[Button]
		void InitializeBlocks()
		{
			_voxels.PopulateInitialBlocks();
		}
	}
}