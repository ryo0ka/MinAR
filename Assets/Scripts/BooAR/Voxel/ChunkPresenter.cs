using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;
using Zenject;
using UniRx;

namespace BooAR.Voxel
{
	public class ChunkPresenter : BaseBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		MeshFilter _filter;

		[SerializeField]
		MeshRenderer _renderer;

		[Inject]
		VoxelSource _source;

		[Inject]
		IGlobalBlockLookup _globalLookup;
#pragma warning restore 649

		readonly CompositeDisposable _life = new CompositeDisposable();
		readonly ReusableCanceller _blockUpdateCanceller = new ReusableCanceller();
		
		UpdateQueue _blockUpdateQueue;
		VoxelQuadBuilder _quadBuilder;
		VoxelMeshBuilder _meshBuilder;

		public void Initiate(Vector3i chunkPosition)
		{
			Clear();
			
			_renderer.sharedMaterials = _source.BlockMaterials;
			_filter.sharedMesh = new Mesh {indexFormat = IndexFormat.UInt32};

			_meshBuilder = new VoxelMeshBuilder(1024, VoxelConsts.BlockCount);
			_quadBuilder = new VoxelQuadBuilder(1024, VoxelConsts.ChunkLength, blockPosition =>
			{
				Vector3i p = VoxelUtils.LocalToWorld(VoxelConsts.ChunkLength, chunkPosition, blockPosition);
				return _globalLookup.Lookup(p);
			});

			InitiateUpdaters();
		}

		public void Clear()
		{
			_life.Clear();
			_blockUpdateCanceller.Cancel();
			_blockUpdateQueue?.Dispose();
			_meshBuilder?.Clear();

			_filter.sharedMesh?.ForSelf(Destroy);
			_filter.sharedMesh = null;
		}

		void InitiateUpdaters()
		{
			_blockUpdateQueue = new UpdateQueue();
			_blockUpdateQueue.Worker.Subscribe(_ => UpdateBlockQuads()).AddTo(_life);
			_blockUpdateQueue.Main.Subscribe(_ => UpdateBlockMesh()).AddTo(_life);
			
			QueueUpdate();
		}

		public void QueueUpdate()
		{
			_blockUpdateQueue.QueueUpdate();
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