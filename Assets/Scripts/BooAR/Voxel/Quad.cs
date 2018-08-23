using UnityEngine;

namespace BooAR.Voxel
{
	public struct Quad
	{
		public Vector3 A { get; }
		public Vector3 B { get; }
		public Vector3 C { get; }
		public Vector3 D { get; }
		public Blocks Block { get; }

		public Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Blocks block)
		{
			A = a;
			B = b;
			C = c;
			D = d;
			Block = block;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = A.GetHashCode();
				hashCode = (hashCode * 397) ^ B.GetHashCode();
				hashCode = (hashCode * 397) ^ C.GetHashCode();
				hashCode = (hashCode * 397) ^ D.GetHashCode();
				hashCode = (hashCode * 397) ^ Block.GetHashCode();
				return hashCode;
			}
		}

		bool Equals(Quad other) => A.Equals(other.A) && B.Equals(other.B) && C.Equals(other.C) && D.Equals(other.D);
		public override bool Equals(object o) => !ReferenceEquals(null, o) && (o is Quad q) && Equals(q);
		public override string ToString() => $"({A}, {B}, {C}, {D}, {Block})";
		public static Quad operator +(Quad q, Vector3 v) => new Quad(q.A + v, q.B + v, q.C + v, q.D + v, q.Block);
	}
}