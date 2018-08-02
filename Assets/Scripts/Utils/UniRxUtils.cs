using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Utils
{
	public static class UniRxUtils
	{
		public static void SetValue<T>(this IReactiveProperty<T> r, T value)
		{
			r.Value = value;
		}

		public static IDisposable SubscribeUnit(this IObservable<Unit> o, Action f)
		{
			return o.Subscribe(_ => f());
		}

		public static void OnNext(this IObserver<Unit> o)
		{
			o.OnNext(Unit.Default);
		}

		public static IObservable<T> PublishWithRefCount<T>(this IObservable<T> o)
		{
			return o.Publish().RefCount();
		}

		public static IObservable<bool> Mask(this IObservable<bool> o1, IObservable<bool> o2)
		{
			return o1.CombineLatest(o2, (b1, b2) => b1 && b2);
		}

		public static IDisposable Toggle(Action<bool> f)
		{
			f(true);
			return Disposable.Create(() => f(false));
		}

		public static IDisposable ToggleActive(GameObject gameObject)
		{
			return Toggle(gameObject.SetActive);
		}

		public static IDisposable ToggleReverse(Action<bool> f)
		{
			f(false);
			return Disposable.Create(() => f(true));
		}

		public static IDisposable Finally(Action f)
		{
			return Disposable.Create(f);
		}

		public static IObservable<T> NonNull<T>(this IObservable<T?> o) where T : struct
		{
			// ReSharper disable once PossibleInvalidOperationException
			return o.Where(t => t.HasValue).Select(t => t.Value);
		}

		public static IObservable<T> TakeFirst<T>(this IObservable<T> o, Func<T, bool> f = null)
		{
			return o.Where(f ?? (_ => true)).Take(1);
		}

		public static void Away(this UniTask t)
		{
			t.Forget(Debug.LogException);
		}

		public static CancellationToken CancellationToken(this ICollection<IDisposable> d)
		{
			CancellationTokenSource s = new CancellationTokenSource();
			d.Add(Disposable.Create(s.Cancel));

			return s.Token;
		}

		public static CancellationToken CancellationToken<T>(this IObservable<T> o)
		{
			CancellationTokenSource s = new CancellationTokenSource();
			o.Subscribe(_ => s.Cancel());
			return s.Token;
		}

		public static async UniTask<Unit> Then(this UniTask<Unit> t, Action f)
		{
			await t;
			f();
			return Unit.Default;
		}

		public static IObservable<T> First<T>(params IObservable<T>[] os)
		{
			return Observable.Merge(os).First();
		}

		public static IObservable<T> DoOnEnd<T>(this IObservable<T> o, Action f)
		{
			return o.DoOnCompleted(f)
			        .DoOnCancel(f)
			        .DoOnError(_ => f());
		}

		public static IObservable<Unit> WhereTrue(this IObservable<bool> b)
		{
			return b.Where(t => t).AsUnitObservable();
		}

		public static void Add(this ICollection<IDisposable> d, Action f)
		{
			d.Add(Disposable.Create(f));
		}

		public static IDisposable SubscribeAway<T>(this IObservable<T> o, Func<T, UniTask> f)
		{
			return o.Subscribe(t => f(t).Away());
		}

		public static UniTask<T> ToUniTask<T>(this IObservable<T> o, IObservable<T> canceller)
		{
			return o.ToUniTask(canceller.CancellationToken(), true);
		}

		public static UniTask Delay(this TimeSpan t)
		{
			return UniTask.Delay(t);
		}
	}
}