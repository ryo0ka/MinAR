using System;
using System.Threading;

namespace BooAR.Voxel
{
	[Serializable]
	public class ReusableCanceller
	{
		CancellationTokenSource _canceller;
		public CancellationToken Token => GetOrPopulateToken();

		CancellationToken GetOrPopulateToken()
		{
			if (_canceller == null)
			{
				_canceller = new CancellationTokenSource();
			}

			return _canceller.Token;
		}

		public void Cancel()
		{
			_canceller?.Cancel();
			_canceller?.Dispose();
			_canceller = null;
		}
	}
}