using System;
using UnityEngine;
using Utils;

namespace BooAR
{
	public class GameConsts : BaseBehaviour, IGameConsts
	{
		[SerializeField]
		float _arIdDistance;

		[SerializeField]
		float _arIdAngle;

		[SerializeField]
		float _pauseResumeTime;

		[SerializeField]
		float _wallNormalLength;

		public float ArIdDistance => _arIdDistance;
		public float ArIdAngle => _arIdAngle;
		public TimeSpan ResumeTime => _pauseResumeTime.Seconds();
		public float WallNormalLength => _wallNormalLength;
	}
}