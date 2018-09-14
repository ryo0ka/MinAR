using UnityEngine;

namespace BooAR.Voxel
{
	public struct Quad
	{
		public Vector3 A { get; }
		public Vector3 B { get; }
		public Vector3 C { get; }
		public Vector3 D { get; }
		public int Width { get; }
		public int Height { get; }

		public Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int width, int height)
		{
			A = a;
			B = b;
			C = c;
			D = d;
			Width = width;
			Height = height;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = A.GetHashCode();
				hashCode = (hashCode * 397) ^ B.GetHashCode();
				hashCode = (hashCode * 397) ^ C.GetHashCode();
				hashCode = (hashCode * 397) ^ D.GetHashCode();
				hashCode = (hashCode * 397) ^ Width.GetHashCode();
				hashCode = (hashCode * 397) ^ Height.GetHashCode();
				return hashCode;
			}
		}

		bool Equals(Quad o) =>
			A.Equals(o.A) &&
			B.Equals(o.B) &&
			C.Equals(o.C) &&
			D.Equals(o.D) &&
			Width.Equals(o.Width) &&
			Height.Equals(o.Height);

		public override bool Equals(object o) => !ReferenceEquals(null, o) && (o is Quad q) && Equals(q);
		public override string ToString() => $"({A}, {B}, {C}, {D}, {Width}, {Height})";

		public static Quad operator +(Quad q, Vector3 v) =>
			new Quad(q.A + v, q.B + v, q.C + v, q.D + v, q.Width, q.Height);
	}
}