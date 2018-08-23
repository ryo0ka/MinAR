using UnityEngine;
using Utils;

namespace BooAR
{
	public class Player : BaseBehaviour, IPlayer
	{
		[SerializeField]
		Transform _transform;

		[SerializeField]
		float _reach;

		[SerializeField]
		Collider _body;

		[SerializeField]
		Collider _core;

		[SerializeField]
		Camera _camera;

		public Transform Transform => _transform;
		public float HandReach => _reach;
		public Collider BodyReach => _body;
		public Collider Core => _core;

		public Camera Camera => _camera;
	}
}