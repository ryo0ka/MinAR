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
#pragma warning disable 649
		[SerializeField]
		Button _pickaxeButton;

		[Inject]
		InventoryButton.Pool _blockButtonPool;

		[SerializeField, ReadOnly]
		int[] _inventory;

		[SerializeField, ReadOnly]
		InventoryButton[] _buttons;
#pragma warning restore 649

		Subject<Blocks> _blockSelection;
		Subject<Unit> _pickaxeSelection;

		public IObservable<Blocks> OnBlockSelected => _blockSelection;
		public IObservable<Unit> OnPickaxeSelected => _pickaxeSelection;

		void Awake()
		{
			_inventory = new int[BlocksUtils.All.Length];
			_blockSelection = new Subject<Blocks>();
			_pickaxeSelection = new Subject<Unit>();
			_buttons = new InventoryButton[BlocksUtils.All.Length];
		}

		void Start()
		{
			_pickaxeButton.OnClickAsObservable()
			              .Subscribe(_ => OnPickaxeButtonPressed());
		}

		void OnPickaxeButtonPressed()
		{
			_pickaxeSelection.OnNext();
		}

		public bool HasBlock(Blocks block)
		{
			return _inventory[(int) block] > 0;
		}

		public void Add(Blocks block)
		{
			_inventory[(int) block] += 1;

			if (HasBlock(block) && _buttons[(int) block] == null)
			{
				InitializeBlockButton(block);
			}

			UpdateButtonView(block);
		}

		public void Substract(Blocks block)
		{
			_inventory[(int) block] -= 1;
			UpdateButtonView(block);
		}

		void InitializeBlockButton(Blocks block)
		{
			// Don't make a button for the "empty" block type
			if (block == Blocks.Empty) return;

			// Spawn (intiialize) a button for this block type
			InventoryButton button = _blockButtonPool.Spawn(new InventoryButton.Param
			{
				Block = block,
			});

			// Set events to the button
			button.OnClickAsObservable()
			      .Subscribe(_ => OnBlockButtonPressed(block));

			// Set to the button list
			_buttons[(int) block] = button;
		}

		void OnBlockButtonPressed(Blocks block)
		{
			_blockSelection.OnNext(block);
		}

		// Update the count text view on the button of the block
		void UpdateButtonView(Blocks block)
		{
			// Button should be interactable only if the block exists in inventory
			_buttons[(int) block].Interactable = HasBlock(block);

			// Button's count text should refect the block's current amount in inventory
			_buttons[(int) block].SetCount(_inventory[(int) block]);
		}
	}
}