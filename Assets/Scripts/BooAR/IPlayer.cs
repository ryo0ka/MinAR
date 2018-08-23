using UnityEngine;

namespace BooAR
{
	public interface IPlayer
	{
		Transform Transform { get; }
		float HandReach { get; }
		Collider BodyReach { get; }
		Collider Core { get; }
		Camera Camera { get; }
	}

	public static class PlayerUtils
	{
		public static bool VisibleAndReachable(this IPlayer p, Vector3 position)
		{
			Vector2 viewportPos = p.Camera.WorldToViewportPoint(position);
			float distance = Vector3.Distance(p.Transform.position, position);
			return distance < p.HandReach
			       && viewportPos.x > 0f
			       && viewportPos.x < 1f
			       && viewportPos.y > 0f
			       && viewportPos.y < 1f;
		}
	}
}