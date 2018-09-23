using System;
using System.IO;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;
using Zenject;

namespace BooAR.Voxel
{
	public class Chunk : BaseBehaviour
	{
		public struct Params
		{
			public Vector3i Position { get; set; } // for name and stuff
		}

		public class Pool : MonoMemoryPool<Params, Chunk>
		{
			protected override void OnCreated(Chunk item)
			{
				base.OnCreated(item);
				item.OnCreated();
			}

			protected override void Reinitialize(Params param, Chunk item)
			{
				base.Reinitialize(param, item);
				item.OnSpawned(param);
			}

			protected override void OnDespawned(Chunk item)
			{
				base.OnDespawned(item);
				item.OnDespawned();
			}
		}

		struct BlockState
		{
			public int Durability { get; set; }
		}

#pragma warning disable 649
		[SerializeField]
		MeshFilter _filter;

		[SerializeField]
		MeshRenderer _renderer;

		[SerializeField]
		BlockDamageMeshGenerator _damageMesh;

		[Inject]
		VoxelSource _source;

		[Inject]
		IBlockAttributeTable _table;

		[Inject]
		IGlobalBlockLookup _globalLookup;
#pragma warning restore 649

		readonly CompositeDisposable _life = new CompositeDisposable();
		readonly ReusableCanceller _blockUpdateCanceller = new ReusableCanceller();
		readonly UpdateQueue _blockUpdateQueue = new UpdateQueue(.075f.Seconds());
		readonly UpdateQueue _damageUpdateQueue = new UpdateQueue(.075f.Seconds());

		Vector3i _chunkPosition;
		Array3<byte> _blocks;
		Array3<BlockState> _healths;
		VoxelQuadBuilder _quadBuilder;
		VoxelMeshBuilder _meshBuilder;
		ChunkSerializer _serializer;

		void OnCreated()
		{
			_life.AddTo(this);
			_blocks = new Array3<byte>(VoxelConsts.ChunkLength);
			_healths = new Array3<BlockState>(VoxelConsts.ChunkLength);
			_serializer = new ChunkSerializer(VoxelConsts.ChunkLength);
			_meshBuilder = new VoxelMeshBuilder(1024, VoxelConsts.BlockCount);
			_renderer.sharedMaterials = _source.BlockMaterials;
			_quadBuilder = new VoxelQuadBuilder(1024, VoxelConsts.ChunkLength, blockPosition =>
			{
				Vector3i p = VoxelUtils.LocalToWorld(VoxelConsts.ChunkLength, _chunkPosition, blockPosition);
				return _globalLookup.Lookup(p);
			});
		}

		void OnSpawned(Params param)
		{
			using (UnityUtils.Sample("Chunk.OnSpawned()"))
			{
				_chunkPosition = param.Position;
				name = $"Chunk({_chunkPosition})";
				_filter.sharedMesh = new Mesh {indexFormat = IndexFormat.UInt32};

				InitiateUpdaters();
				QueueUpdate();
			}
		}

		void OnDespawned()
		{
			_life.Clear();
			_blockUpdateCanceller.Cancel();
			_meshBuilder.Clear();
			_damageMesh.ResetHealthAll();
			_damageUpdateQueue.QueueUpdate();

			Destroy(_filter.sharedMesh);
			_filter.sharedMesh = null;

			name = $"Chunk(null)";
		}

		public BlockLookup LookUp(Vector3i position)
		{
			byte block = GetBlock(position);
			Visibilities visibility = _table.GetVisibility(block);
			return new BlockLookup(block, visibility);
		}

		public byte GetBlock(Vector3i position)
		{
			return _blocks[position];
		}

		float GetDurability(Vector3i position)
		{
			byte block = _blocks[position];
			int initDurability = _table.GetDurability(block);
			int durability = _healths[position].Durability;
			return (float) durability / initDurability;
		}

		// Returns durability (remainig health in 0~1f) of the block
		public float DamageBlock(Vector3i position, int damage)
		{
			byte block = _blocks[position];

			// Can't break the thin air
			Debug.Assert(block != VoxelConsts.EmptyBlock);

			BlockState state = _healths[position];
			int maxHealth = _table.GetDurability(block);
			if (state.Durability <= 0) // if first time hit:
			{
				// Initialize durability
				state.Durability = maxHealth;
			}

			// Deal damage
			state.Durability -= damage;
			_healths[position] = state;

			float durability = Mathf.Max(0f, (float) state.Durability / maxHealth);
			_damageMesh.UpdateHealth(position, durability);
			_damageUpdateQueue.QueueUpdate();

			if (durability <= 0)
			{
				// Break block if durability exceeded
				SetBlock(position, VoxelConsts.EmptyBlock);
			}

			return durability;
		}

		public bool SetBlock(Vector3i position, byte block)
		{
			if (!_blocks.ContainsIndex(position)) return false; // skip out bounds
			if (_blocks[position] == block) return false; // skip already-punched

			_blocks[position] = block;

			// initialize damage state
			_healths[position] = new BlockState();
			_damageMesh.ResetHealth(position);
			_damageUpdateQueue.QueueUpdate();

			QueueUpdate();
			return true;
		}

		public void Load(Stream serializedBlocks)
		{
			_serializer.Deserialize(_blocks, serializedBlocks);
			QueueUpdate();
		}

		public void Save(Stream serializedBlocks)
		{
			_serializer.Serialize(_blocks, serializedBlocks);
		}

		public void QueueUpdate()
		{
			_blockUpdateQueue.QueueUpdate();
		}

		void InitiateUpdaters()
		{
			_blockUpdateQueue
				.StartWorker()
				.Subscribe(_ => UpdateBlockQuads())
				.AddTo(_life);

			_blockUpdateQueue
				.StartMain()
				.Subscribe(_ => UpdateBlockMesh())
				.AddTo(_life);

			_damageUpdateQueue
				.StartWorker()
				.Subscribe(_ => _damageMesh.UpdateQuads())
				.AddTo(_life);

			_damageUpdateQueue
				.StartMain()
				.Subscribe(_ => _damageMesh.UpdateMesh())
				.AddTo(_life);
		}

		void UpdateBlockQuads() // Executed in a worker thread
		{
			try
			{
				// cancel the ongoing update
				_blockUpdateCanceller.Cancel();

				CancellationToken token = _blockUpdateCanceller.Token;

				// Clear previous mesh data
				_meshBuilder.Clear();

				// Update mesh data with current quads
				foreach ((Quad quad, byte block) in _quadBuilder.Build(token))
				{
					_meshBuilder.Add(quad, block);

					token.ThrowIfCancellationRequested();
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		void UpdateBlockMesh()
		{
			_meshBuilder.Apply(_filter.sharedMesh);
		}
	}
}