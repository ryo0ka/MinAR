using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using AnimeRx;
using UniRx;
using UniRx.Async;
using UniRx.Triggers;
using UnityEngine;

namespace Utils
{
	public static class UniRxUtils
	{
		public static void SetValue<T>(this IReactiveProperty<T> r, T value)
		{
			r.Value = value;
		}

		public static IDisposable SubscribeUnit<T>(this IObservable<T> o, Action f)
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

		public static IDisposable Toggle(ISubject<bool> subject)
		{
			subject.OnNext(true);
			return Disposable.Create(() => subject.OnNext(false));
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
			IDisposable d = null;
			d = o.Subscribe(_ =>
			{
				s.Cancel();
				d.Dispose();
				d = null;
			});

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

		public static IObservable<bool> MergeBool<T, U>(IObservable<T> t, IObservable<U> f)
		{
			return Observable.Merge(t.Select(_ => true), f.Select(_ => false));
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

		public static IObservable<Unit> WhereFalse(this IObservable<bool> b)
		{
			return b.Where(t => !t).AsUnitObservable();
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

		public static IObservable<Unit> TimerUnscaled(TimeSpan timeSpan)
		{
			return Observable
			       .Timer(timeSpan, Scheduler.ThreadPool)
			       .ObserveOnMainThread()
			       .AsUnitObservable();
		}

		public static IObservable<long> IntervalUnscaled(TimeSpan timeSpan)
		{
			return Observable
			       .Interval(timeSpan, Scheduler.ThreadPool)
			       .ObserveOnMainThread();
		}

		public static IObservable<float> PlayUnscaled(float init, float last, IAnimator animator)
		{
			return Anime.Play(init, last, animator, new UnscaledTimeScheduler())
			            .ObserveOnMainThread();
		}

		public static IObservable<(T, UniTask<U>)> FromTo<T, U>(
			this IObservable<T> from,
			IObservable<U> to,
			CancellationToken canceller)
		{
			return from.Select(n => (n, to.ToUniTask(canceller, true)));
		}

		public static IObservable<Unit> StartWithUnit<T>(this IObservable<T> t)
		{
			return t.AsUnitObservable().StartWith(Unit.Default);
		}

		public static IObservable<UniTask<Unit>> FromTo(this IObservable<Unit> from, IObservable<Unit> to)
		{
			return from.FromTo(to, System.Threading.CancellationToken.None)
			           .Select(p => p.Item2);
		}

		public static IObservable<bool> Reverse(this IObservable<bool> o)
		{
			return o.Select(b => !b);
		}

		public static IObservable<U> SelectMany<T, U>(this IEnumerable<T> ts, Func<T, IObservable<U>> f)
		{
			return ts.Select(f).ToObservable().SelectMany(_ => _);
		}

		public static IObservable<T> WhereNonNull<T>(this IObservable<T> ts) where T : class
		{
			return ts.Where(t => t != null);
		}

		public static IObservable<T> FilterNonNull<T>(this IObservable<T?> ts) where T : struct
		{
			// ReSharper disable once PossibleInvalidOperationException
			return ts.Where(t => t.HasValue).Select(t => t.Value);
		}

		public static IObservable<Unit> KeyPressedAsObservable(this KeyCode k)
		{
			return Observable.EveryUpdate().Select(_ => Input.GetKey(k)).WhereTrue();
		}

		public static IObservable<U> SelectOrDefault<T, U>(this IObservable<T> o, Func<T, U> f, U def) where T : class
		{
			return o.Select(t => (t == null) ? def : f(t));
		}

		public static IObservable<Unit> OnTriggerEnterAsObservable(this Collider c, Collider target)
		{
			return c.OnTriggerEnterAsObservable()
			        //.Do(cc => Debug.Log($"{c}.OnTriggerEnterAsObservable({target}) -> {cc}"))
			        .Where(cc => cc == target)
			        .AsUnitObservable();
		}

		public static IObservable<Unit> OnTriggerExitAsObservable(this Collider c, Collider target)
		{
			return c.OnTriggerExitAsObservable()
			        .Where(cc => cc == target)
			        .AsUnitObservable();
		}

		public static IObservable<bool> OnTriggerEnterOrExitAsObservable(this Collider c, Collider target)
		{
			return MergeBool(c.OnTriggerEnterAsObservable(target), c.OnTriggerExitAsObservable(target));
		}

		public static IObservable<UniTask<Unit>> ObserveTriggerInOut(this Collider c, Collider target)
		{
			return c.OnTriggerEnterAsObservable(target)
			        .FromTo(c.OnTriggerExitAsObservable(target));
		}

		public static IObservable<bool> ObserveRaycast(this Transform eye, Collider target, float reach)
		{
			Collider Raycast() =>
				Physics.Raycast(eye.position, eye.forward, out RaycastHit hit, reach)
					? hit.collider
					: null;

			return Observable.EveryFixedUpdate()
			                 .Select(_ => Raycast())
			                 .Select(c => c == target);
		}

		public static IObservable<T> SkipLatestValueOnSubscribe<T>(this IReadOnlyReactiveProperty<T> r, bool skip)
		{
			return (skip)
				? r.SkipLatestValueOnSubscribe()
				: r;
		}
	}
}