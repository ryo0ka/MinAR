using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Voxel
{
	[Serializable]
	public class VoxelWorld : BaseBehaviour, IGlobalBlockLookup
	{
#pragma warning disable 649
		[SerializeField]
		ChunksSerializer _serializer;

		[SerializeField]
		Vector3i _initialExtent;

		[Inject(Id = VoxelInstaller.Ids.ChunkLength)]
		int _chunkLength;

		[Inject(Id = VoxelInstaller.Ids.BlockSize)]
		float _blockSize;

		[Inject]
		ITerrainGenerator _terrain;

		[Inject]
		IBlockAttributeTable _table;

		[Inject]
		Chunk.Pool _chunkPool;
#pragma warning restore 649

		Dictionary<Vector3i, Chunk> _chunks;
		ISet<Vector3i> _neighbors;
		Vector3 _rootPos;

		void Awake()
		{
			_chunks = new Dictionary<Vector3i, Chunk>();
			_neighbors = new HashSet<Vector3i>();

			float pos = (_chunkLength * _blockSize) / 2;
			_rootPos = new Vector3(pos, pos, pos);

			transform.position = -_rootPos;
			transform.localScale = new Vector3(_blockSize, _blockSize, _blockSize);
		}

		public void PopulateInitialBlocks()
		{
			ClearBlocks();

			GetOrAddChunk((0, 0, 0));

			for (int x = -_initialExtent.x + 1; x < _initialExtent.x; x++)
			for (int y = -_initialExtent.y + 1; y < _initialExtent.y; y++)
			for (int z = -_initialExtent.z + 1; z < _initialExtent.z; z++)
			{
				GetOrAddChunk((x, y, z));
			}
		}

		void ClearBlocks()
		{
			_chunks.Values.ForEach(_chunkPool.Despawn);
			_chunks.Clear();
		}

		public void Save(string id)
		{
			_serializer.Serialize(id, _chunks);
		}

		public void Load(string id)
		{
			_serializer.Deserialize(id, _chunks, c => GetOrAddChunk(c));
		}

		public Vector3 WorldToVoxel(Vector3 worldPosition)
		{
			return (worldPosition + _rootPos) / _blockSize;
		}

		public Vector3i WorldToVoxel(Vector3i worldPosition)
		{
			return WorldToVoxel(worldPosition * 1f).RoundToInt3();
		}

		public Blocks? GetBlock(Vector3i position)
		{
			(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(position);
			if (_chunks.TryGetValue(chunkPosition, out Chunk chunk))
			{
				return chunk.GetBlock(blockPosition);
			}

			return null;
		}

		public Blocks GetBlockOrInit(Vector3i position)
		{
			(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(position);
			Chunk chunk = GetOrAddChunk(chunkPosition);
			return chunk.GetBlock(blockPosition);
		}

		public bool DamageBlock(Vector3i position, int damage)
		{
			using (UnityUtils.Sample("VoxelWorld.DamageBlock()"))
			{
				(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(position);
				Chunk chunk = GetOrAddChunk(chunkPosition);

				if (chunk.DamageBlock(blockPosition, damage) == 0) // block destroyed
				{
					UpdateNeighborChunk(chunkPosition, blockPosition);
					return true;
				}

				return false;
			}
		}

		public void SetBlock(Vector3i position, Blocks block)
		{
			using (UnityUtils.Sample("VoxelWorld.SetBlock()"))
			{
				(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(position);
				Chunk chunk = GetOrAddChunk(chunkPosition);

				if (chunk.SetBlock(blockPosition, block)) // block placed
				{
					UpdateNeighborChunk(chunkPosition, blockPosition);
				}
			}
		}

		void UpdateNeighborChunk(Vector3i chunkPosition, Vector3i blockPosition)
		{
			// set neighboring chunks dirty
			_neighbors.Clear();
			VoxelUtils.FindNeighbors(_chunkLength, blockPosition, _neighbors);
			foreach (Vector3i delta in _neighbors)
			{
				Vector3i neighborChunkPosition = chunkPosition + delta;

				// Do spawn the neighbor chunk if it doesn't exist
				// because their blocks must be visible if the set block is Empty.
				Chunk neighborChunk = GetOrAddChunk(neighborChunkPosition);

				neighborChunk.QueueUpdate();
			}
		}

		Chunk GetOrAddChunk(Vector3i position)
		{
			using (UnityUtils.Sample("VoxelWorld.GetOrAddChunk()"))
			{
				if (_chunks.TryGetValue(position, out Chunk chunk)) return chunk;
				return _chunks[position] = SpawnChunk(position);
			}
		}

		Chunk SpawnChunk(Vector3i chunkPosition)
		{
			Chunk chunk = _chunkPool.Spawn(new Chunk.Params
			{
				Position = chunkPosition,
			});

			InitializeBlocks(chunk, chunkPosition);
			chunk.transform.localPosition = chunkPosition * _chunkLength;

			return chunk;
		}

		void InitializeBlocks(Chunk chunk, Vector3i chunkPosition)
		{
			using (UnityUtils.Sample("VoxelWorld.InitializeBlocks()"))
			{
				// Initialize blocks in the chunk
				for (int x = 0; x < _chunkLength; x++)
				for (int y = 0; y < _chunkLength; y++)
				for (int z = 0; z < _chunkLength; z++)
				{
					Vector3i localPosition = new Vector3i(x, y, z);
					Vector3i globalPosition = LocalToGlobal(chunkPosition, new Vector3i(x, y, z));
					chunk.SetBlock(localPosition, _terrain.GenerateBlock(globalPosition));
				}
			}
		}

		public Lookup? Lookup(Vector3i globalBlockPosition)
		{
			(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(globalBlockPosition);

			if (_chunks.TryGetValue(chunkPosition, out Chunk chunk))
			{
				return chunk.LookUp(blockPosition);
			}

			return null;
		}

		Vector3i LocalToGlobal(Vector3i chunkPosition, Vector3i blockPosition)
		{
			return VoxelUtils.LocalToWorld(_chunkLength, chunkPosition, blockPosition);
		}

		(Vector3i, Vector3i) GlobalToLocal(Vector3i position)
		{
			return VoxelUtils.WorldToLocal(_chunkLength, position);
		}
	}
}