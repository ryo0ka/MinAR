using System;
using UnityEngine;

namespace BooAR.Voxel
{
	[Serializable]
	public class Array3<T>
	{
		[SerializeField]
		readonly T[] _self;

		public Vector3i Length { get; }

		public T this[int index]
		{
			get => _self[index];
			set => _self[index] = value;
		}

		public T this[Vector3i index]
		{
			get => _self[Index(index)];
			set => _self[Index(index)] = value;
		}

		public Array3(Vector3i length)
		{
			Length = length;
			_self = new T[length.x * length.y * length.z];
		}

		public Array3(int length) : this((length, length, length))
		{
		}

		int Index(Vector3i index)
		{
			return VoxelUtils.Index(Length, index);
		}

		public bool ContainsIndex(Vector3i index)
		{
			return VoxelUtils.ContainsIndex(Length, index);
		}
	}
}