using System;
using UniRx;
using Utils;

namespace BooAR.Voxel
{
	public class UpdateQueue
	{
		readonly Subject<Unit> _mainQueue = new Subject<Unit>();
		readonly TimeSpan _workerFrequency;
		
		bool _isUpdatingMain;
		bool _shouldUpdateWorker;

		public UpdateQueue(TimeSpan workerFrequency)
		{
			_workerFrequency = workerFrequency;
		}

		// Let worker thread do the task in the next interval.
		public void QueueUpdate()
		{
			_shouldUpdateWorker = true;

			_mainQueue.OnNext(); // Queue main task
		}

		public IObservable<Unit> StartWorker()
		{
			return Observable.Create<Unit>(observer =>
			{
				return Observable
				       .Interval(_workerFrequency, Scheduler.ThreadPool)
				       .Where(_ => _shouldUpdateWorker)
				       .Subscribe(_ =>
				       {
					       _shouldUpdateWorker = false;

					       while (_isUpdatingMain)
					       {
						       // wait
					       }

					       observer.OnNext(); // do the worker task
				       });
			});
		}

		public IObservable<Unit> StartMain()
		{
			return Observable.Create<Unit>(observer =>
			{
				return _mainQueue
				       .ObserveOnMainThread()
				       .Subscribe(_ =>
				       {
					       _isUpdatingMain = true;
					       observer.OnNext(); // do the main task
					       _isUpdatingMain = false;
				       });
			});
		}
	}
}