using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Utils;

namespace BooAR.Contents
{
	public static class ContentsUtils
	{
		public static bool HorizontalReached(Vector3 p1, Vector3 p2, float threshold)
		{
			return threshold > Vector2.Distance(
				       MathUtils.Horizontal(p1),
				       MathUtils.Horizontal(p2));
		}

		public static bool HorizontalLookedAt(Vector3 p1forward, Vector3 p1, Vector3 p2, float threshold)
		{
			return threshold > Vector2.Angle(
				       MathUtils.Horizontal(p1forward),
				       MathUtils.Horizontal(p2 - p1));
		}

		public static IObservable<Unit> ObserveReached(this Transform t, Transform target, float reach)
		{
			return t.UpdateAsObservable()
			        .FirstOrDefault(_ => HorizontalReached(t.position, target.position, reach))
			        .AsUnitObservable();
		}

		public static IObservable<float> ToVelocity(this IObservable<Vector3> positions)
		{
			return positions.Pairwise().Select(p => Vector3.Distance(p.Previous, p.Current));
		}
	}
}