using System;
using System.Collections.Generic;

namespace Utils
{
	public class Pool<T>
	{
		readonly Stack<T> _self;
		readonly Func<T> _create;
		readonly Action<T> _init;
		
		public Pool(int capacity, Func<T> create, Action<T> init)
		{
			_self = new Stack<T>(capacity);
			_create = create;
			_init = init;
		}

		public void Enpool(T element)
		{
			_self.Push(element);
			_init(element);
		}

		public T Unpool()
		{
			return (_self.Count == 0) ? _create() : _self.Pop();
		}
	}
}