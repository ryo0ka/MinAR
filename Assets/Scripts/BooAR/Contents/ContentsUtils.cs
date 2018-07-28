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

		public static IObservable<float> ObservePositionVelocity(this Transform t)
		{
			return t.UpdateAsObservable()
			        .Select(_ => t.position)
			        .Pairwise()
			        .StartWith(new Pair<Vector3>(t.position, t.position))
			        .Select(pp => Vector2.Distance(
				        MathUtils.Horizontal(pp.Current),
				        MathUtils.Horizontal(pp.Previous)))
			        .Select(d => d / Time.deltaTime);
		}

		public static IObservable<Unit> ObserveReached(this Transform t, Transform target, float reach)
		{
			return t.UpdateAsObservable()
			        .First(_ => HorizontalReached(t.position, target.position, reach))
			        .AsUnitObservable();
		}
	}
}