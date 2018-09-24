using System.Collections.Generic;
using UnityEngine;
using Utils;
using Zenject;

namespace BooAR.Voxel
{
	public class VoxelWorld : BaseBehaviour, IGlobalBlockLookup, IVoxelPersistence
	{
#pragma warning disable 649
		[Inject]
		ITerrainGenerator _terrain;

		[Inject]
		Chunk.Pool _chunkPool;

		[Inject]
		IBlockAttributeTable _table;

		[SerializeField]
		BlockActionHandler _actions;
#pragma warning restore 649

		readonly VoxelWorldSerializer _serializer = new VoxelWorldSerializer();
		readonly Dictionary<Vector3i, Chunk> _chunks = new Dictionary<Vector3i, Chunk>();
		readonly ISet<Vector3i> _neighbors = new HashSet<Vector3i>();
		Vector3 _rootPos;

		void Awake()
		{
			const float pos = (VoxelConsts.ChunkLength * VoxelConsts.BlockSize) / 2;
			_rootPos = new Vector3(pos, pos, pos);

			transform.position = -_rootPos;
			transform.localScale = new Vector3(VoxelConsts.BlockSize, VoxelConsts.BlockSize, VoxelConsts.BlockSize);
		}

		public void Save(string dirPath)
		{
			_serializer.Serialize(dirPath, _chunks);
		}

		public void Load(string dirPath)
		{
			Clear();
			_serializer.Deserialize(dirPath, _chunks, GetOrAddChunk);
		}

		public void Clear()
		{
			_chunks.Values.ForEach(_chunkPool.Despawn);
			_chunks.Clear();
			_actions.Initialize();
		}

		public Vector3 WorldToVoxel(Vector3 worldPosition)
		{
			return (worldPosition + _rootPos) / VoxelConsts.BlockSize;
		}

		public Vector3i WorldToVoxel(Vector3i worldPosition)
		{
			return WorldToVoxel(worldPosition * 1f).RoundToInt3();
		}

		public byte GetBlock(Vector3i position)
		{
			(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(position);
			Chunk chunk = GetOrAddChunk(chunkPosition);
			return chunk.GetBlock(blockPosition);
		}

		public bool DamageBlock(Vector3i position, Vector3i face, int damage)
		{
			using (UnityUtils.Sample("VoxelWorld.DamageBlock()"))
			{
				(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(position);
				Chunk chunk = GetOrAddChunk(chunkPosition);
				byte block = chunk.GetBlock(blockPosition);
				float maxHealth = _table.GetDurability(block);

				if (_actions.Damage(position, face, block, damage / maxHealth))
				{
					chunk.SetBlock(blockPosition, VoxelConsts.EmptyBlock);
					UpdateNeighborChunk(chunkPosition, blockPosition);
					return true;
				}

				return false;
			}
		}

		public void SetBlock(Vector3i position, byte block, bool animate = false)
		{
			using (UnityUtils.Sample("VoxelWorld.SetBlock()"))
			{
				(Vector3i chunkPosition, Vector3i blockPosition) = GlobalToLocal(position);
				Chunk chunk = GetOrAddChunk(chunkPosition);

				if (chunk.SetBlock(blockPosition, block)) // block placed
				{
					UpdateNeighborChunk(chunkPosition, blockPosition);

					if (block != VoxelConsts.EmptyBlock && animate)
					{
						_actions.Place(position);
					}
				}
			}
		}

		void UpdateNeighborChunk(Vector3i chunkPosition, Vector3i blockPosition)
		{
			// set neighboring chunks dirty
			_neighbors.Clear();
			VoxelUtils.FindNeighbors(VoxelConsts.ChunkLength, blockPosition, _neighbors);
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
			if (_chunks.TryGetValue(position, out Chunk chunk)) return chunk;
			return _chunks[position] = SpawnChunk(position);
		}

		Chunk SpawnChunk(Vector3i chunkPosition)
		{
			Chunk chunk = _chunkPool.Spawn(new Chunk.Params
			{
				Position = chunkPosition,
			});

			InitializeBlocks(chunk, chunkPosition);
			chunk.transform.localPosition = chunkPosition * VoxelConsts.ChunkLength;

			return chunk;
		}

		void InitializeBlocks(Chunk chunk, Vector3i chunkPosition)
		{
			using (UnityUtils.Sample("VoxelWorld.InitializeBlocks()"))
			{
				// Initialize blocks in the chunk
				for (int x = 0; x < VoxelConsts.ChunkLength; x++)
				for (int y = 0; y < VoxelConsts.ChunkLength; y++)
				for (int z = 0; z < VoxelConsts.ChunkLength; z++)
				{
					Vector3i localPosition = new Vector3i(x, y, z);
					Vector3i globalPosition = LocalToGlobal(chunkPosition, new Vector3i(x, y, z));
					chunk.SetBlock(localPosition, _terrain.GenerateBlock(globalPosition));
				}
			}
		}

		public BlockLookup? Lookup(Vector3i globalBlockPosition)
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
			return VoxelUtils.LocalToWorld(VoxelConsts.ChunkLength, chunkPosition, blockPosition);
		}

		(Vector3i, Vector3i) GlobalToLocal(Vector3i position)
		{
			return VoxelUtils.WorldToLocal(VoxelConsts.ChunkLength, position);
		}
	}
}