using System.IO;
using UniRx;
using UnityEngine;
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

#pragma warning disable 649
		[Inject]
		IBlockAttributeTable _table;

		[SerializeField]
		ChunkPresenter _presenter;
#pragma warning restore 649

		readonly CompositeDisposable _life = new CompositeDisposable();

		Vector3i _chunkPosition;
		Array3<byte> _blocks;
		ChunkSerializer _serializer;

		void OnCreated()
		{
			_life.AddTo(this);
			_blocks = new Array3<byte>(VoxelConsts.ChunkLength);
			_serializer = new ChunkSerializer(VoxelConsts.ChunkLength);
		}

		void OnSpawned(Params param)
		{
			using (UnityUtils.Sample("Chunk.OnSpawned()"))
			{
				_chunkPosition = param.Position;
				_presenter.Initiate(param.Position);
				name = $"Chunk({_chunkPosition})";
			}
		}

		void OnDespawned()
		{
			_life.Clear();
			_presenter.Clear();

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

		public bool SetBlock(Vector3i position, byte block)
		{
			if (!_blocks.ContainsIndex(position)) return false; // skip out bounds
			if (_blocks[position] == block) return false; // skip already-punched

			_blocks[position] = block;

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
			_presenter.QueueUpdate();
		}
	}
}