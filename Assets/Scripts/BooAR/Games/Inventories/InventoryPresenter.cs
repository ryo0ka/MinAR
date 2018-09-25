using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Games.Inventories
{
	public class InventoryPresenter : BaseBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		Button _pickaxeButton;

		[SerializeField, ReadOnly]
		InventoryButton[] _blockButtons;

		[Inject]
		InventoryButton.Pool _blockButtonPool;

		[Inject]
		IInventory _inventory;

		[Inject]
		IInventoryToolController _toolController;
#pragma warning restore 649

		readonly CompositeDisposable _initButtons = new CompositeDisposable();

		void Start()
		{
			_blockButtons = new InventoryButton[BlocksUtils.All.Length];

			_pickaxeButton
				.OnClickAsObservable()
				.Subscribe(_ => _toolController.SelectPickaxe());

			_inventory
				.OnCountChanged
				.Subscribe(b => OnInventoryBlockCountChanged(b));

			_inventory
				.OnReplaced
				.Subscribe(_ => InitializeButtons());
		}

		void InitializeButtons()
		{
			// Dispose button events
			_initButtons.Clear();

			// Dispawn button objects in scene
			foreach (InventoryButton button in _blockButtons)
			{
				button?.ForSelf(_blockButtonPool.Despawn);
			}

			// Discard dispawned button objects
			_blockButtons.Clear();

			// Repopulate buttons
			foreach (Blocks block in BlocksUtils.All)
			{
				if (_inventory.Has(block))
				{
					InitializeBlockButton(block);
					UpdateButtonViews(block);
				}
			}
		}

		void OnInventoryBlockCountChanged(Blocks block)
		{
			// Populate a button for the block if not yet
			if (_blockButtons[(int) block] == null)
			{
				InitializeBlockButton(block);
			}

			UpdateButtonViews(block);
		}

		void InitializeBlockButton(Blocks block)
		{
			//Debug.Log("InventoryPresenter.InitializeBlockButton()");

			// Don't make a button for the "empty" block type
			if (block == Blocks.Empty) return;

			// Spawn (intiialize) a button for this block type
			InventoryButton button =
				_blockButtonPool.Spawn(new InventoryButton.Param
				{
					Block = block,
				});

			// Set events to the button
			button.OnClickAsObservable()
			      .Subscribe(_ => _toolController.SelectBlock(block))
			      .AddTo(_initButtons);

			// Set to the button list
			_blockButtons[(int) block] = button;
		}

		// Update the count text view on the button of the block
		void UpdateButtonViews(Blocks block)
		{
			// Button should be interactable only if the block exists in inventory
			_blockButtons[(int) block].Interactable = _inventory.Has(block);

			// Button's count text should refect the block's current amount in inventory
			_blockButtons[(int) block].SetCount(_inventory.GetCount(block));
		}
	}
}