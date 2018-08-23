using System;

namespace BooAR
{
	public interface IGameConsts
	{
		float ArIdDistance { get; }
		float ArIdAngle { get; }
		TimeSpan ResumeTime { get; }
		float WallNormalLength { get; }
	}
}