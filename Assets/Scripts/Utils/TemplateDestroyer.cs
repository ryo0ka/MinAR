using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	public class TemplateDestroyer : MonoBehaviour
	{
		[SerializeField]
		List<GameObject> _templates;

		void Awake()
		{
			_templates.ForEach(Destroy);
		}
	}
}