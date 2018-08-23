using System;
using System.Threading;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utils
{
	public static class UiUtils
	{
		public static void Toggle(this Toggle t)
		{
			t.isOn = !t.isOn;
		}

		public static Toggle StartWith(this Toggle t, bool isOn)
		{
			t.isOn = isOn;
			return t;
		}

		public static UniTask<bool> ObserveNext(this Toggle toggle, CancellationToken canceller)
		{
			return toggle.OnValueChangedAsObservable().ToUniTask(canceller, true);
		}

		public static UniTask ObserveNext(this Button toggle, CancellationToken canceller)
		{
			return toggle.OnClickAsObservable().ToUniTask(canceller, true);
		}

		public static IDisposable BindTo(this IReactiveCommand<bool> command, Toggle toggle)
		{
			var d1 = command.CanExecute.SubscribeToInteractable(toggle);
			var d2 = toggle.OnValueChangedAsObservable().SubscribeWithState(command, (x, c) => c.Execute(x));
			return StableCompositeDisposable.Create(d1, d2);
		}

		public static IDisposable SubscribeToAlpha(this IObservable<float> o, CanvasGroup canvas)
		{
			return o.Subscribe(a => canvas.alpha = a);
		}

		public static IObservable<float> DoToAlpha(this IObservable<float> o, CanvasGroup canvas)
		{
			return o.Do(a => canvas.alpha = a);
		}

		public static IObservable<BaseEventData> OnEventTriggerdAsObservable(this EventTrigger t,
			EventTriggerType type)
		{
			return Observable.Create<BaseEventData>(observer =>
			{
				EventTrigger.TriggerEvent callback = new EventTrigger.TriggerEvent();
				callback.AddListener(observer.OnNext);

				EventTrigger.Entry entry = new EventTrigger.Entry
				{
					eventID = type,
					callback = callback,
				};
				
				t.triggers.Add(entry);

				return Disposable.Create(() => t.triggers.Remove(entry));
			});
		}
	}
}