using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Apps
{
	public class GameList : BaseBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		Button _cancelButton;

		[Inject]
		GameListItem.Pool _itemPool;

		[Inject]
		IGameList _list;
#pragma warning restore 649

		readonly CompositeDisposable _open = new CompositeDisposable();
		readonly List<GameListItem> _items = new List<GameListItem>();
		readonly Subject<string> _selection = new Subject<string>();
		readonly Subject<Unit> _cancellation = new Subject<Unit>();

		void Prepare()
		{
			Initialize();

			// Spawn list items
			foreach (string id in _list.GetGameIDs())
			{
				GameListItem item = _itemPool.Spawn(new GameListItem.Param(id));

				// Keep track of it
				_items.Add(item);

				// Route selection event
				item.OnSelected
				    .Subscribe(i => _selection.OnNext(i))
				    .AddTo(_open);
			}

			// Route cancellation event
			_cancelButton
				.OnClickAsObservable()
				.Subscribe(_ => _cancellation.OnNext())
				.AddTo(_open);

			// Refresh list when list changed
			_list.OnChanged
			     .Subscribe(_ => Prepare())
			     .AddTo(_open);
		}

		void Initialize()
		{
			_open.Clear();
			_items.ForEach(_itemPool.Despawn);
			_items.Clear();
		}

		public async UniTask<string> Open()
		{
			using (UniRxUtils.Toggle(SetVisible)) // open the list, and close when finished
			{
				Prepare();

				// Return null when cancelled
				IObservable<string> cancel = _cancellation.Select(_ => (string) null);

				string input = await _selection.Merge(cancel).ToUniTask(useFirstValue: true);

				Initialize();

				return input;
			}
		}

		void SetVisible(bool visible)
		{
			gameObject.SetActive(visible);
		}
	}
}