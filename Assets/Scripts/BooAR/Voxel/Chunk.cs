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

		[SerializeField]
		MeshFilter _filter;

		[SerializeField]
		MeshRenderer _renderer;

		[Inject(Id = VoxelInstaller.Ids.ChunkLength)]
		int _length;

		[Inject]
		VoxelMeshSource _source;

		[Inject]
		IGlobalBlockLookup _globalLookup;

		readonly CompositeDisposable _life = new CompositeDisposable();
		Subject<Unit> _updateMesh;
		Vector3i _chunkPosition;
		ReusableCanceller _quadUpdates;
		Array3<Blocks> _blocks;
		VoxelQuadBuilder _quadBuilder;
		VoxelMeshBuilder _meshBuilder;
		BlocksSerializer _serializer;
		bool _mustUpdateQuads;
		bool _isUpdatingMesh;

		void OnCreated()
		{
			_life.AddTo(this);
			_quadUpdates = new ReusableCanceller();
			_blocks = new Array3<Blocks>(_length);
			_serializer = new BlocksSerializer(_length);
			_meshBuilder = new VoxelMeshBuilder(1024);
			_renderer.sharedMaterials = _source.BlockMaterials;
			_quadBuilder = new VoxelQuadBuilder(1024, _length, blockPosition =>
			{
				Vector3i p = VoxelUtils.LocalToWorld(_length, _chunkPosition, blockPosition);
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

				TimeSpan updateFrequency = 0.075f.Seconds();
				_updateMesh = new Subject<Unit>().AddTo(_life);

				// Check if a QUADS update is queued every once in a while.
				// If queued, update the quads on a background thread.
				Observable.Interval(updateFrequency, Scheduler.ThreadPool)
				          .Where(_ => _mustUpdateQuads)
				          .Subscribe(_ => UpdateQuads())
				          .AddTo(_life);

				// When a MESH update is queued, update the mesh on the main thread.
				_updateMesh.ObserveOnMainThread()
				           .ThrottleFrame(1) // up to once every frame
				           .Subscribe(_ => UpdateMesh())
				           .AddTo(_life);

				QueueUpdate();
			}
		}

		void OnDespawned()
		{
			_life.Clear();
			_quadUpdates.Cancel();
			_meshBuilder.Clear();

			Destroy(_filter.sharedMesh);
			_filter.sharedMesh = null;

			name = $"Chunk(null)";
		}

		public Blocks GetBlock(Vector3i position)
		{
			return _blocks[position];
		}

		public bool SetBlock(Vector3i position, Blocks block)
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
			_mustUpdateQuads = true;
		}

		void UpdateQuads()
		{
			if (!_mustUpdateQuads) return;
			_mustUpdateQuads = false;

			_quadUpdates.Cancel(); // cancel the ongoing update

			lock (_quadUpdates) // wait until the last update is done or cancelled
			{
				try
				{
					CancellationToken token = _quadUpdates.Token;
					
					while (_isUpdatingMesh) // wait until mesh update is done
					{
						token.ThrowIfCancellationRequested();
					}

					var quads = _quadBuilder.Build(token);
					_meshBuilder.Update(quads, token);
					_updateMesh.OnNext(); // update mesh in the next frame
				}
				catch (OperationCanceledException)
				{
				}
			}
		}

		void UpdateMesh()
		{
			_isUpdatingMesh = true;
			_meshBuilder.Apply(_filter.sharedMesh);
			_isUpdatingMesh = false;
		}
	}
}