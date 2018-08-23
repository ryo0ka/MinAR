using System;
using System.Collections.Generic;
using UnityEngine;

namespace BooAR.Voxel
{
	public enum Directions
	{
		Forward,
		Right,
		Left,
		Back,
		Up,
		Down,
	}

	public static class VoxelUtils
	{
		public static Vector3 Normal(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			Vector3 u = v1 - v0;
			Vector3 v = v2 - v0;
			return new Vector3(
				x: u.y * v.z - u.z * v.y,
				y: u.z * v.x - u.x * v.z,
				z: u.x * v.y - u.y * v.x);
		}

		public static Directions Direction(Vector3i normal)
		{
			if (normal.x < 0) return Directions.Left;
			if (normal.x > 0) return Directions.Right;
			if (normal.y < 0) return Directions.Up;
			if (normal.y > 0) return Directions.Down;
			if (normal.z < 0) return Directions.Back;
			if (normal.z > 0) return Directions.Forward;
			throw new ArgumentOutOfRangeException(nameof(normal), normal, null);
		}

		static void Quad(Directions d, out Vector3 v0, out Vector3 v1, out Vector3 v2, out Vector3 v3)
		{
			switch (d)
			{
				case Directions.Forward:
					v0 = new Vector3(1, 0, 1);
					v1 = new Vector3(1, 1, 1);
					v2 = new Vector3(0, 1, 1);
					v3 = new Vector3(0, 0, 1);
					break;
				case Directions.Right:
					v0 = new Vector3(1, 0, 0);
					v1 = new Vector3(1, 1, 0);
					v2 = new Vector3(1, 1, 1);
					v3 = new Vector3(1, 0, 1);
					break;
				case Directions.Left:
					v0 = new Vector3(0, 0, 1);
					v1 = new Vector3(0, 1, 1);
					v2 = new Vector3(0, 1, 0);
					v3 = new Vector3(0, 0, 0);
					break;
				case Directions.Back:
					v0 = new Vector3(0, 0, 0);
					v1 = new Vector3(0, 1, 0);
					v2 = new Vector3(1, 1, 0);
					v3 = new Vector3(1, 0, 0);
					break;
				case Directions.Up:
					v0 = new Vector3(0, 1, 0);
					v1 = new Vector3(0, 1, 1);
					v2 = new Vector3(1, 1, 1);
					v3 = new Vector3(1, 1, 0);
					break;
				case Directions.Down:
					v0 = new Vector3(0, 0, 0);
					v1 = new Vector3(1, 0, 0);
					v2 = new Vector3(1, 0, 1);
					v3 = new Vector3(0, 0, 1);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(d), d, null);
			}
		}

		public static int Index(Vector3i length, Vector3i index)
		{
			if (!ContainsIndex(length, index))
			{
				throw new ArgumentOutOfRangeException($"length: {length}, index: {index}");
			}

			return index.x + (index.y * length.x) + (index.z * length.x * length.y);
		}

		public static bool ContainsIndex(Vector3i length, Vector3i index)
		{
			return ContainsIndex(0, length, index)
			       && ContainsIndex(1, length, index)
			       && ContainsIndex(2, length, index);
		}

		public static bool ContainsIndex(int axis, Vector3i length, Vector3i index)
		{
			return 0 <= index[axis] && index[axis] < length[axis];
		}

		public static Vector3i RoundToInt3(this Vector3 v3)
		{
			return new Vector3i(
				Mathf.RoundToInt(v3.x),
				Mathf.RoundToInt(v3.y),
				Mathf.RoundToInt(v3.z));
		}

		public static Vector3i LocalToWorld(int containerLength, Vector3i containerPosition, Vector3i position)
		{
			return position + (containerPosition * containerLength);
		}

		public static (Vector3i, Vector3i) WorldToLocal(int containerLength, Vector3i position)
		{
			Vector3i containerPosition = new Vector3i();
			Vector3i localPosition = new Vector3i();

			for (int i = 0; i < 3; i++)
			{
				int p = position[i];
				if (p >= 0)
				{
					containerPosition[i] = p / containerLength;
					localPosition[i] = p % containerLength;
				}
				else // negative
				{
					int chunkp = Mathf.FloorToInt((float) p / containerLength);
					int blockp = chunkp * containerLength * -1 + p;

					containerPosition[i] = chunkp;
					localPosition[i] = blockp;
				}
			}

			return (containerPosition, localPosition);
		}

		public static void FindNeighbors(int containerLength, Vector3i position, ICollection<Vector3i> deltas)
		{
			for (int i = 0; i < 3; i++)
			{
				int p = position[i];

				if (p == 0)
				{
					Vector3i d = new Vector3i();
					d[i] = -1;
					deltas.Add(d);
				}

				if (p == containerLength - 1)
				{
					Vector3i d = new Vector3i();
					d[i] = 1;
					deltas.Add(d);
				}
			}
		}
	}
}