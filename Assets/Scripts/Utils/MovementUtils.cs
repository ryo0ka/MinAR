using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Utils
{
	public static class MovementUtils
	{
		public static float HorizontalDistance(Vector3 p1, Vector3 p2)
		{
			return Vector2.Distance(
				MathUtils.Horizontal(p1),
				MathUtils.Horizontal(p2));
		}

		public static bool HorizontalReached(Vector3 p1, Vector3 p2, float threshold)
		{
			return threshold > HorizontalDistance(p1, p2);
		}

		public static bool HorizontalLookedAt(Vector3 p1forward, Vector3 p1, Vector3 p2, float threshold)
		{
			return threshold > Vector2.Angle(
				       MathUtils.Horizontal(p1forward),
				       MathUtils.Horizontal(p2 - p1));
		}

		public static IObservable<float> ProximityAsObservable(this Transform t, Transform target)
		{
			return t.UpdateAsObservable()
			        .Select(_ => HorizontalDistance(t.position, target.position))
			        .DistinctUntilChanged();
		}

		public static IObservable<float> ToVelocity(this IObservable<Vector3> positions)
		{
			return positions.Pairwise()
			                .Select(p => Vector3.Distance(p.Previous, p.Current))
			                .DistinctUntilChanged();
		}
	}
}