using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Apps
{
	public class GameListItem : BaseBehaviour
	{
		public struct Param
		{
			public Param(string id)
			{
				ID = id;
			}

			public string ID { get; set; }
		}
		
		public class Pool : MonoMemoryPool<Param, GameListItem>
		{
		    protected override void OnCreated(GameListItem item)
		    {
		        base.OnCreated(item);
		        item.OnCreated();
		    }
		
		    protected override void Reinitialize(Param param, GameListItem item)
		    {
		        base.Reinitialize(param, item);
		        item.OnSpawned(param);
		    }
		
		    protected override void OnDespawned(GameListItem item)
		    {
		        base.OnDespawned(item);
		        item.OnDespawned();
		    }
		}
		
		[SerializeField]
		Text _idText;

		[SerializeField]
		Button _selectButton;
		
		readonly CompositeDisposable _life = new CompositeDisposable();
		readonly Subject<string> _selection = new Subject<string>();

		public IObservable<string> OnSelected => _selection;
		
		void OnCreated()
		{
		    _life.AddTo(this);
		}
		
		void OnSpawned(Param param)
		{
			_idText.text = param.ID;

			_selectButton
				.OnClickAsObservable()
				.Subscribe(_ => _selection.OnNext(param.ID))
				.AddTo(_life);
		}
		
		void OnDespawned()
		{
		    _life.Clear();
		}
	}
}