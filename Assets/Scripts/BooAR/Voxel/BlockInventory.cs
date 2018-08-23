using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;
using Sirenix.OdinInspector;

namespace BooAR.Voxel
{
	public class BlockInventory : BaseBehaviour
	{
		[SerializeField]
		Button _pickaxeButton;

		[Inject]
		InventoryButton.Pool _blockButtonPool;

		[SerializeField, ReadOnly]
		int[] _inventory;

		[SerializeField, ReadOnly]
		InventoryButton[] _buttons;

		Subject<Blocks> _blockSelection;
		Subject<Unit> _pickaxeSelection;
		Subject<Blocks> _blockRunout;

		public IObservable<Blocks> OnBlockSelected => _blockSelection;
		public IObservable<Unit> OnPickaxeSelected => _pickaxeSelection;
		public IObservable<Blocks> OnBlockRunOut => _blockRunout;

		void Awake()
		{
			_inventory = new int[BlocksUtils.All.Length];
			_blockSelection = new Subject<Blocks>();
			_pickaxeSelection = new Subject<Unit>();
			_blockRunout = new Subject<Blocks>();
			_buttons = new InventoryButton[BlocksUtils.All.Length];
		}

		void Start()
		{
			_pickaxeButton.OnClickAsObservable().Subscribe(_ =>
			{
				_pickaxeSelection.OnNext();
			});

			for (int i = 0; i < BlocksUtils.All.Length; i++)
			{
				Blocks block = (Blocks) i;

				if (block == Blocks.Empty) continue;

				_buttons[i] = _blockButtonPool.Spawn(new InventoryButton.Param
				{
					Block = block,
				});

				_buttons[i].Button.OnClickAsObservable().Subscribe(_ =>
				{
					_blockSelection.OnNext(block);
				});
			}
		}

		public int GetCount(Blocks block)
		{
			return _inventory[(int) block];
		}

		public void Add(Blocks block)
		{
			_inventory[(int) block] += 1;

			if (_inventory[(int) block] == 1)
			{
				_buttons[(int) block].Button.interactable = true;
			}

			UpdateButtonCount(block);
		}

		public void Substract(Blocks block)
		{
			_inventory[(int) block] -= 1;

			if (_inventory[(int) block] <= 0)
			{
				_buttons[(int) block].Button.interactable = false;
				_blockRunout.OnNext(block);
			}

			UpdateButtonCount(block);
		}

		void UpdateButtonCount(Blocks block)
		{
			_buttons[(int) block].SetCount(_inventory[(int) block]);
		}
	}
}