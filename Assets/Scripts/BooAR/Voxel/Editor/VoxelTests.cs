using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace BooAR.Voxel.Editor
{
	class VoxelTestsbase
	{
		[Test]
		public void Vector3i_Equals()
		{
			Assert.AreEqual(
				new Vector3i(0, 1, -2).GetHashCode(),
				new Vector3i(0, 1, -2).GetHashCode());

			Assert.AreEqual(
				new Vector3i(0, 1, -2),
				new Vector3i(0, 1, -2));
		}

		[Test]
		public void VoxelUtils_WorldToLocalPosition_PositiveMin()
		{
			Assert.AreEqual(
				VoxelUtils.WorldToLocal(16, (0, 0, 0)),
				(new Vector3i(0, 0, 0), new Vector3i(0, 0, 0)));
		}

		[Test]
		public void VoxelUtils_WorldToLocalPosition_PositiveMax()
		{
			Assert.AreEqual(
				VoxelUtils.WorldToLocal(16, (15, 0, 0)),
				(new Vector3i(0, 0, 0), new Vector3i(15, 0, 0)));
		}

		[Test]
		public void VoxelUtils_WorldToLocalPosition_PositiveMultiple()
		{
			Assert.AreEqual(
				VoxelUtils.WorldToLocal(16, (20, 0, 0)),
				(new Vector3i(1, 0, 0), new Vector3i(4, 0, 0)));
		}

		[Test]
		public void VoxelUtils_WorldToLocalPosition_NegativeMin()
		{
			Assert.AreEqual(
				VoxelUtils.WorldToLocal(16, (-16, 0, 0)),
				(new Vector3i(-1, 0, 0), new Vector3i(0, 0, 0)));
		}

		[Test]
		public void VoxelUtils_WorldToLocalPosition_NegativeMax()
		{
			Assert.AreEqual(
				VoxelUtils.WorldToLocal(16, (-1, 0, 0)),
				(new Vector3i(-1, 0, 0), new Vector3i(15, 0, 0)));
		}

		[Test]
		public void VoxelUtils_WorldToLocalPosition_NegativeMultiple()
		{
			Assert.AreEqual(
				VoxelUtils.WorldToLocal(16, (-20, 0, 0)),
				(new Vector3i(-2, 0, 0), new Vector3i(12, 0, 0)));
		}
	}
}