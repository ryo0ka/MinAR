using BooAR.Haptics;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;
using UnityEngine.UI;
using Zenject;

namespace BooAR.Voxel
{
	public class VoxelGameController : BaseBehaviour
	{
#pragma warning disable 649
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

		[Inject]
		IHapticFeedbackGenerator _haptics;
#pragma warning restore 649

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
			       //.Merge(_screen.OnDragAsObservable().AsUnitObservable())
			       .Subscribe(r => OnRaycast(GetRay()));

			// Connect with block inventory
			_inventory.OnBlockSelected.Subscribe(OnBlockButtonSelected);
			_inventory.OnPickaxeSelected.Subscribe(_ => OnPickaxeButtonSelected());
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
					if (block != Blocks.Empty)
					{
						OnBlockSelected(position, block, face);
						return true; // End tracing
					}

					return false; // Continue tracing
				});
			}
		}

		void OnBlockButtonSelected(Blocks block)
		{
			_placedBlock = block;
			
			_haptics.Trigger(HapticFeedbackTypes.Selection);
		}

		void OnPickaxeButtonSelected()
		{
			_placedBlock = null;
			
			_haptics.Trigger(HapticFeedbackTypes.Selection);
		}

		void OnBlockSelected(Vector3i position, Blocks block, Vector3i face)
		{
			if (_placedBlock == null)
			{
				DamageBlock(position, block);
			}
			else
			{
				PlaceBlock(position + face, _placedBlock.Value);
			}
		}

		void DamageBlock(Vector3i position, Blocks block)
		{
			if (_voxels.DamageBlock(position, 1)) // if block destroyed
			{
				_inventory.Add(block);
				_haptics.Trigger(HapticFeedbackTypes.ImpactHeavy);
			}
			else // if block damaged (but not destroyed)
			{
				_haptics.Trigger(HapticFeedbackTypes.ImpactLight);
			}
		}

		void PlaceBlock(Vector3i position, Blocks block)
		{
			if (_inventory.HasBlock(block))
			{
				_voxels.SetBlock(position, block);
				_inventory.Substract(block);

				_haptics.Trigger(HapticFeedbackTypes.ImpactMedium);
			}
			else
			{
				_haptics.Trigger(HapticFeedbackTypes.Failure);
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