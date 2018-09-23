using System;
using System.IO;
using UniRx;
using UnityEngine;
using Newtonsoft.Json;
using Utils;

namespace BooAR.Games
{
	public class Inventory : IInventory
	{
		readonly int[] _inventory = new int[BlocksUtils.All.Length];
		readonly Subject<Blocks> _onCountChanged = new Subject<Blocks>();
		readonly Subject<Unit> _onReplaced = new Subject<Unit>();

		public IObservable<Blocks> OnCountChanged => _onCountChanged;
		public IObservable<Unit> OnReplaced => _onReplaced;

		public bool Has(Blocks block)
		{
			return _inventory[(int) block] > 0;
		}

		public int GetCount(Blocks block)
		{
			return _inventory[(int) block];
		}

		public void Initialize()
		{
			_inventory.Clear();
			_onReplaced.OnNext();
		}

		public void Add(Blocks block)
		{
			_inventory[(int) block] += 1;
			_onCountChanged.OnNext(block);
		}

		public void Substract(Blocks block)
		{
			Debug.Assert(_inventory[(int) block] > 0);

			_inventory[(int) block] -= 1;
			_onCountChanged.OnNext(block);
		}

		public void Save(string dirPath)
		{
			Debug.Assert(Directory.Exists(dirPath));

			string json = JsonConvert.SerializeObject(_inventory);
			File.WriteAllText(GetPath(dirPath), json);
		}

		public void Load(string dirPath)
		{
			Debug.Assert(Directory.Exists(dirPath));

			string json = File.ReadAllText(GetPath(dirPath));
			int[] inventory = JsonConvert.DeserializeObject<int[]>(json);
			Array.Copy(inventory, _inventory, _inventory.Length);

			_onReplaced.OnNext();
		}

		string GetPath(string dirPath)
		{
			return Path.Combine(dirPath, "inventory.json");
		}
	}
}