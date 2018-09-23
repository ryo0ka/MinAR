using System;
using UniRx;
using Utils;

namespace BooAR.Voxel
{
	public class UpdateQueue : IDisposable
	{
		// Stream to do expensive task in a background thread
		readonly Subject<Unit> _worker = new Subject<Unit>();

		// Stream to do UI task in the main thread
		readonly Subject<Unit> _main = new Subject<Unit>();

		// Frequency of the background thread
		readonly TimeSpan _workerFrequency = .075f.Seconds();

		bool _isUpdatingMain;
		bool _shouldUpdateWorker;
		bool _shouldUpdateMain;

		public IObservable<Unit> Worker => _worker;
		public IObservable<Unit> Main => _main;

		public UpdateQueue()
		{
			Observable
				.Interval(_workerFrequency, Scheduler.ThreadPool)
				.Where(_ => _shouldUpdateWorker)
				.Subscribe(_ =>
				{
					_shouldUpdateWorker = false;

					// Wait until UI tasks are done (if any)
					while (_isUpdatingMain)
					{
					}

					_worker.OnNext();
					_shouldUpdateMain = true;
				});

			Observable
				.EveryUpdate()
				.Where(_ => _shouldUpdateMain)
				.Subscribe(_ =>
				{
					_shouldUpdateMain = false;

					_isUpdatingMain = true;
					_main.OnNext(); // do the main task
					_isUpdatingMain = false;
				});
		}

		// Let worker thread do the task in the next interval.
		public void QueueUpdate()
		{
			_shouldUpdateWorker = true;
		}

		public void Dispose()
		{
			_worker.Dispose();
			_main.Dispose();
		}
	}
}