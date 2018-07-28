using UnityEngine;

namespace BooAR.Players
{
	[SelectionBase]
	public class Player : BaseBehaviour, IPlayer
	{
		[SerializeField]
		Transform _player;
		
		public Transform Transform => _player;
	}
}