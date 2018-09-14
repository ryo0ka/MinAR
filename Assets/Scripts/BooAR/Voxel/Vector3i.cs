using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BooAR.Voxel
{
	[Serializable, InlineProperty]
	public struct Vector3i
	{
		[SerializeField, HorizontalGroup, HideLabel]
		int _x;

		[SerializeField, HorizontalGroup, HideLabel]
		int _y;

		[SerializeField, HorizontalGroup, HideLabel]
		int _z;

		public int x
		{
			get => _x;
			set => _x = value;
		}

		public int y
		{
			get => _y;
			set => _y = value;
		}

		public int z
		{
			get => _z;
			set => _z = value;
		}

		public int this[int i]
		{
			get => Get(i);
			set => Set(i, value);
		}

		public Vector3i(int x = 0, int y = 0, int z = 0)
		{
			_x = x;
			_y = y;
			_z = z;
		}

		int Get(int i)
		{
			switch (i)
			{
				case 0: return x;
				case 1: return y;
				case 2: return z;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		void Set(int i, int v)
		{
			switch (i)
			{
				case 0: x = v; break;
				case 1: y = v; break;
				case 2: z = v; break;
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = x;
				hashCode = (hashCode * 397) ^ y;
				hashCode = (hashCode * 397) ^ z;
				return hashCode;
			}
		}

		bool Equals(Vector3i o) => x == o.x && y == o.y && z == o.z;
		public override bool Equals(object o) => !ReferenceEquals(null, o) && o is Vector3i i && Equals(i);
		public override string ToString() => $"({x}, {y}, {z})";

		public static Vector3i operator +(Vector3i i, Vector3i j) => new Vector3i(i.x + j.x, i.y + j.y, i.z + j.z);
		public static Vector3i operator -(Vector3i i, Vector3i j) => new Vector3i(i.x - j.x, i.y - j.y, i.z - j.z);
		public static Vector3i operator *(Vector3i i, int m) => new Vector3i(i.x * m, i.y * m, i.z * m);
		public static Vector3 operator *(Vector3i i, float m) => new Vector3(i.x * m, i.y * m, i.z * m);
		public static Vector3 operator /(Vector3i i, float m) => new Vector3(i.x / m, i.y / m, i.z / m);
		public static Vector3i operator %(Vector3i i, int m) => new Vector3i(i.x % m, i.y % m, i.z % m);
		public static bool operator ==(Vector3i i, Vector3i j) => i.Equals(j);
		public static bool operator !=(Vector3i i, Vector3i j) => !i.Equals(j);
		public static implicit operator Vector3i((int, int, int) t) => new Vector3i(t.Item1, t.Item2, t.Item3);
		public static implicit operator (int, int, int)(Vector3i i) => (i.x, i.y, i.z);
		public static implicit operator Vector3(Vector3i i) => new Vector3(i.x, i.y, i.z);

		public static Vector3i Sign(Vector3i i)
		{
			return new Vector3i(Math.Sign(i.x), Math.Sign(i.y), Math.Sign(i.z));
		}
	}
}