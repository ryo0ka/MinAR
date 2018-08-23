using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Sirenix.OdinInspector;
using UniRx;
using Zenject;

namespace BooAR.Voxel
{
	public class InventoryButton : BaseBehaviour
	{
		public struct Param
		{
			public Blocks Block { get; set; }
		}

		public class Pool : MonoMemoryPool<Param, InventoryButton>
		{
			protected override void OnCreated(InventoryButton item)
			{
				base.OnCreated(item);
				item.OnCreated();
			}

			protected override void Reinitialize(Param param, InventoryButton item)
			{
				base.Reinitialize(param, item);
				item.OnSpawned(param);
			}

			protected override void OnDespawned(InventoryButton item)
			{
				base.OnDespawned(item);
				item.OnDespawned();
			}
		}

		readonly CompositeDisposable _life = new CompositeDisposable();

		[SerializeField, ReadOnly]
		Blocks _block;

		[SerializeField]
		Button _button;

		[SerializeField]
		Graphic _graphic;

		[SerializeField]
		Text _countText;

		[Inject]
		VoxelMeshSource _source;

		public Button Button => _button;

		void OnCreated()
		{
			_life.AddTo(this);
		}

		void OnSpawned(Param param)
		{
			_block = param.Block;
			_graphic.color = _source.BlockMaterials[(int) _block].GetColor("_Color");
			name = $"InventoryButton({_block})";
			SetCount(0);
		}

		void OnDespawned()
		{
			_life.Clear();
		}

		public void SetCount(int count)
		{
			_countText.text = $"{count}";
		}
	}
}