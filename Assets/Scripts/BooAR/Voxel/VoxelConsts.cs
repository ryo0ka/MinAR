namespace BooAR.Voxel
{
	public static class VoxelConsts
	{
		public const int ChunkLength = 16;
		public const float BlockSize = 0.5f;
		public const int DamageLength = 5;

		public const string BlockIndexKey = "_BlockIndex";
		public const string DamageIndexKey = "_DamageIndex";

		public static readonly int BlockCount = Games.BlocksUtils.All.Length;
		public const byte EmptyBlock = (byte) Games.Blocks.Empty;
	}
}