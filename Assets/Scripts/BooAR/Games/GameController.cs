using BooAR.Games.Inventories;
using BooAR.Haptics;
using BooAR.Voxel;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Games
{
	/* Game controller with some public lifecycle operation 
	 * 
	 */
	public class GameController : BaseBehaviour, IInventoryToolController
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
		PausePresenter _pausePresenter;

		[SerializeField]
		int _bodyExtent;

		[Inject]
		IInventory _inventory;

		[Inject]
		IHapticFeedbackGenerator _haptics;
#pragma warning restore 649

		readonly CompositeDisposable _life = new CompositeDisposable();
		readonly PauseHandler _metaPause = new PauseHandler();
		readonly PauseHandler _stuckPause = new PauseHandler();

		Blocks? _placedBlock;
		Vector3i _recoverablePosition;
		bool IsPaused => _metaPause.IsPaused || _stuckPause.IsPaused;

		// Start the game
		// NOTE use `TendSpawn()` too
		public void Initialize()
		{
			// Clear ongoing game (if any)
			End();

			// Observe player position
			this.UpdateAsObservable()
			    .Select(_ => GetPlayerVoxelPosition())
			    .DistinctUntilChanged()
			    .Subscribe(p => OnPlayerPositionUpdated(p))
			    .AddTo(_life);

			// Observe screen tap input (for placing/breaking blocks)
			_screen.OnClickAsObservable()
			       //.Merge(_screen.OnDragAsObservable().AsUnitObservable())
			       //.Where(_ => !IsPaused) // don't raycast while paused
			       .Subscribe(r => OnRaycast(GetRay()))
			       .AddTo(_life);

			_metaPause
				.OnStateChanged
				.CombineLatest(_stuckPause.OnStateChanged, (s, t) => (s, t))
				.Subscribe(s => OnPauseStateChanged(s.Item1, s.Item2))
				.AddTo(_life);
		}

		// Decorate the spawn position
		public void TendSpawn()
		{
			TendAround(GetPlayerVoxelPosition());
		}

		public void End()
		{
			// Discard observers
			_life.Clear();

			// Initialize game state
			_voxels.Clear();
			_inventory.Initialize();
		}

		public void SetPause(bool pause)
		{
			_metaPause.SetPause(pause, GetPlayerVoxelPosition());
		}

		public void SelectBlock(Blocks block)
		{
			_placedBlock = block;
			_haptics.Trigger(HapticFeedbackTypes.Selection);
		}

		public void SelectPickaxe()
		{
			_placedBlock = null;
			_haptics.Trigger(HapticFeedbackTypes.Selection);
		}

		void OnPauseStateChanged(PauseHandler.State meta, PauseHandler.State stuck)
		{
			//Debug.Log($"OnPauseStateChanged({meta}, {stuck})");
		}

		Vector3i GetPlayerVoxelPosition()
		{
			return _voxels.WorldToVoxel(_player.position.RoundToInt3());
		}

		void OnPlayerPositionUpdated(Vector3i position)
		{
			_metaPause.TryResume(position);

			bool stuck = (Blocks) _voxels.GetBlock(position) != Blocks.Empty;
			if (stuck)
			{
				_stuckPause.Pause(_recoverablePosition);
			}
			else
			{
				_stuckPause.TryResume(position);
				_recoverablePosition = position;
			}
		}

		void TendAround(Vector3i p)
		{
			for (int x = p.x - _bodyExtent; x < p.x + _bodyExtent; x++)
			for (int y = p.y - _bodyExtent; y < p.y + _bodyExtent; y++)
			for (int z = p.z - _bodyExtent; z < p.z + _bodyExtent; z++)
			{
				_voxels.SetBlock((x, y, z), (byte) Blocks.Empty);
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
					Blocks block = (Blocks) _voxels.GetBlock(position);
					if (block != Blocks.Empty)
					{
						OnBlockSelected(position, block, face);
						return true; // End tracing
					}

					return false; // Continue tracing
				});
			}
		}

		void OnBlockSelected(Vector3i position, Blocks block, Vector3i face)
		{
			if (_placedBlock == null)
			{
				DamageBlock(position, face, block);
			}
			else
			{
				PlaceBlock(position + face, _placedBlock.Value);
			}
		}

		void DamageBlock(Vector3i position, Vector3i face, Blocks block)
		{
			if (_voxels.DamageBlock(position, face, 1)) // if block destroyed
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
			if (_inventory.Has(block))
			{
				_voxels.SetBlock(position, (byte) block, animate: true);
				_inventory.Substract(block);

				_haptics.Trigger(HapticFeedbackTypes.ImpactMedium);
			}
			else
			{
				_haptics.Trigger(HapticFeedbackTypes.Failure);
			}
		}
	}
}