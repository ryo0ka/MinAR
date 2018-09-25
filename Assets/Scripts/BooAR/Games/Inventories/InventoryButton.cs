using System;
using BooAR.Voxel;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Games.Inventories
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

#pragma warning disable 649
		[SerializeField, ReadOnly]
		Blocks _block;

		[SerializeField]
		Button _button;

		[SerializeField]
		Graphic _graphic;

		[SerializeField]
		Text _countText;
#pragma warning restore 649

		public IObservable<Unit> OnClickAsObservable() => _button.OnClickAsObservable();

		public bool Interactable
		{
			get => _button.interactable;
			set => _button.interactable = value;
		}

		void OnCreated()
		{
			_life.AddTo(this);
			_graphic.material = Instantiate(_graphic.material);
		}

		void OnSpawned(Param param)
		{
			_block = param.Block;
			_graphic.material.SetFloat(VoxelConsts.BlockIndexKey, (int) _block);
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