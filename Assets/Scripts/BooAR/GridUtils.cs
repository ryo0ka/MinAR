using System;
using System.Collections.Generic;
using UnityEngine;

namespace BooAR
{
	public enum Directions
	{
		North,
		East,
		West,
		South,
	}

	public struct Grid
	{
		public int X { get; }
		public int Z { get; }

		#region Utils

		Grid(int x, int z)
		{
			X = x;
			Z = z;
		}

		bool Equals(Grid other)
		{
			return X == other.X && Z == other.Z;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X * 397) ^ Z;
			}
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Grid grid && Equals(grid);
		}

		public override string ToString()
		{
			return $"({X}, {Z})";
		}

		public static bool operator ==(Grid g1, Grid g2)
		{
			return g1.Equals(g2);
		}

		public static bool operator !=(Grid g1, Grid g2)
		{
			return !g1.Equals(g2);
		}

		public static implicit operator (int, int)(Grid grid)
		{
			return (grid.X, grid.Z);
		}

		public static implicit operator Grid((int, int) g)
		{
			return new Grid(g.Item1, g.Item2);
		}

		#endregion
	}

	public static class GridUtils
	{
		public static readonly Directions[] AllDirections =
			(Directions[]) Enum.GetValues(typeof(Directions));

		public static Grid PositionToGrid(Vector2 gridSize, Vector3 position)
		{
			return (Mathf.RoundToInt(position.x / gridSize.x),
				Mathf.RoundToInt(position.z / gridSize.x));
		}

		public static Vector3 GridToPosition(Vector2 gridSize, Grid grid)
		{
			return new Vector3(gridSize.x * grid.X, 0f, gridSize.x * grid.Z);
		}

		public static Directions Reverse(this Directions direction)
		{
			switch (direction)
			{
				case Directions.North: return Directions.South;
				case Directions.East: return Directions.West;
				case Directions.West: return Directions.East;
				case Directions.South: return Directions.North;
				default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public static (Grid, Directions) Mirror(Grid g, Directions d)
		{
			(int x, int z) = (g.X, g.Z);
			switch (d)
			{
				case Directions.North: return ((x, z + 1), Directions.South);
				case Directions.East: return ((x + 1, z), Directions.West);
				case Directions.West: return ((x - 1, z), Directions.East);
				case Directions.South: return ((x, z - 1), Directions.North);
				default: throw new ArgumentOutOfRangeException(nameof(d), d, null);
			}
		}

		public static Vector3 ToVector(this Directions direction)
		{
			switch (direction)
			{
				case Directions.North: return Vector3.forward;
				case Directions.East: return Vector3.right;
				case Directions.West: return Vector3.left;
				case Directions.South: return Vector3.back;
				default: throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public static Vector3 ToReverseVector(this Directions direction)
		{
			return direction.Reverse().ToVector();
		}

		public static ISet<Grid> Connected(Grid initialGrid, Func<Grid, Directions, bool> predicate)
		{
			HashSet<Grid> set = new HashSet<Grid>();
			FindAllConnected(initialGrid, predicate, set);
			return set;
		}

		static void FindAllConnected(
			Grid initialGrid,
			Func<Grid, Directions, bool> predicate,
			ISet<Grid> connected)
		{
			Debug.Assert(!connected.Contains(initialGrid));

			connected.Add(initialGrid);

			foreach (Directions direction in AllDirections)
			{
				(Grid nextGrid, Directions nextDirection) = Mirror(initialGrid, direction);
				if (!connected.Contains(nextGrid) && predicate(nextGrid, nextDirection))
				{
					FindAllConnected(nextGrid, predicate, connected);
				}
			}
		}
	}
}