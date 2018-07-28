using System;
using UniRx;
using UnityEngine;

namespace BooAR.Games
{
	public class ScreenState : BaseBehaviour, IScreenState
	{
		[SerializeField]
		RectTransform _root;

		public void AddComponent(Transform component)
		{
			component.SetParent(_root);
		}
	}
}