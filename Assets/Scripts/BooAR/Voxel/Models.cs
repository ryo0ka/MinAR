using System;

namespace BooAR.Voxel
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

	public enum Visibilities
	{
		Clear,
		Opaque,
		Transparent,
	}
	
	public struct Lookup
	{
		public Blocks Block { get; }
		public Visibilities Visibility { get; }
		public bool IsOpaque => Visibility == Visibilities.Opaque;

		public Lookup(Blocks block, Visibilities visibility)
		{
			Block = block;
			Visibility = visibility;
		}
	}
}