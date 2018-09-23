using System;

namespace BooAR.Games
{
	public enum Blocks : byte
	{
		Empty,
		Stone,
		Coal,
		Iron,
	}

	public static class BlocksUtils
	{
		public static readonly Blocks[] All = (Blocks[]) Enum.GetValues(typeof(Blocks));
	}
}