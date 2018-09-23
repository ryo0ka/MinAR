using System;
using System.Collections.Generic;
using System.Linq;
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
		[SerializeField]
		Button _cancelButton;

		[Inject]
		GameListItem.Pool _itemPool;

		[Inject]
		IGameList _list;

		public async UniTask<string> Open()
		{
			using (UniRxUtils.Toggle(SetVisible)) // open the list, and close when finished
			{
				List<GameListItem> items = new List<GameListItem>();
				
				// Spawn list items
				foreach (string id in _list.GetGameIDs())
				{
					items.Add(_itemPool.Spawn(new GameListItem.Param(id)));
				}

				// Merge all the buttons into a stream of selected ID
				IObservable<string> inputs = items.Select(i => i.OnSelected).Merge();

				// Merge the "cancel" stream
				IObservable<Unit> cancelInput = _cancelButton.OnClickAsObservable();
				inputs = inputs.Merge(cancelInput.Select(_ => (string) null));

				string input = await inputs.First().ToUniTask();

				// Clear the list
				items.ForEach(_itemPool.Despawn);
				items.Clear();

				return input;
			}
		}

		void SetVisible(bool visible)
		{
			gameObject.SetActive(visible);
		}
	}
}