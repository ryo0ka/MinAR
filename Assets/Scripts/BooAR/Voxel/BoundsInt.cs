using System;
using Sirenix.OdinInspector;
using UnityEngine;

// https://gist.github.com/dogfuntom/cc881c8fc86ad43d55d8
namespace BooAR.Voxel
{
	[Serializable, InlineProperty]
	public struct BoundsInt
	{
		[SerializeField, HorizontalGroup, HideLabel]
		Vector3i _min;

		[SerializeField, HorizontalGroup, HideLabel]
		Vector3i _max;

		public Vector3 Center => (Max - Min) / 2f;
		public Vector3 Extents => Max - Center;

		public Vector3i Max
		{
			get => _max;
			set => _max = value;
		}

		public Vector3i Min
		{
			get => _min;
			set => _min = value;
		}

		public Vector3i Size
		{
			get => Max - Min;
			set => Max = Min + value;
		}

		public BoundsInt(Vector3i min, Vector3i max)
		{
			_min = min;
			_max = max;
		}

		public bool Intersects(BoundsInt b)
		{
			return (Min.x <= b.Max.x && Max.x >= b.Min.x && Min.y <= b.Max.y &&
			        Max.y >= b.Min.y && Min.z <= b.Max.z && Max.z >= b.Min.z);
		}

		bool Equals(BoundsInt other)
		{
			return _min.Equals(other._min) && _max.Equals(other._max);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is BoundsInt i && Equals(i);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (_min.GetHashCode() * 397) ^ _max.GetHashCode();
			}
		}

		public static bool operator ==(BoundsInt i, BoundsInt j) => i.Min == j.Min && i.Max == j.Max;
		public static bool operator !=(BoundsInt i, BoundsInt j) => !(i == j);
	}
}