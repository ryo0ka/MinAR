using System;

namespace BooAR.Voxel
{
	public enum Visibilities
	{
		Clear,
		Opaque,
		Transparent,
	}
	
	public struct BlockLookup
	{
		public byte Block { get; }
		public Visibilities Visibility { get; }
		public bool IsOpaque => Visibility == Visibilities.Opaque;

		public BlockLookup(byte block, Visibilities visibility)
		{
			Block = block;
			Visibility = visibility;
		}
	}
}