using System;
using BooAR.Voxel;
using UniRx;
using UnityEngine;

namespace BooAR.Games
{
	public class PauseHandler
	{
		public enum State
		{
			Normal,
			Recovering,
			Paused,
		}

		readonly ReactiveProperty<State> _state = new ReactiveProperty<State>();
		Vector3i _recoveryPosition;

		public IObservable<State> OnStateChanged => _state;
		public bool IsPaused => ToPaused(_state.Value);

		public void SetPause(bool pause, Vector3i position)
		{
			if (pause)
				Pause(position);
			else
				TryResume(position);
		}

		public void Pause(Vector3i position)
		{
			_state.Value = State.Paused;
			_recoveryPosition = position;
		}

		public void TryResume(Vector3i position)
		{
			if (position == _recoveryPosition)
			{
				_state.Value = State.Normal;
			}
			else
			{
				_state.Value = State.Recovering;
			}
		}

		bool ToPaused(State state)
		{
			switch (state)
			{
				case State.Normal: return false;
				case State.Paused: return true;
				case State.Recovering: return true;
				default: throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}
	}
}