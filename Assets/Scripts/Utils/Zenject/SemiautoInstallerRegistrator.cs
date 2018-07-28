using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Utils.Zenject
{
	[RequireComponent(typeof(SceneContext))]
	public class SemiautoInstallerRegistrator : MonoBehaviour
	{
		[SerializeField]
		List<GameObject> _targets;

		[Button]
		void Register()
		{
			SceneContext context = GetComponent<SceneContext>();
			context.Installers = _targets.SelectMany(t => t.GetComponents<MonoInstaller>());
		}
	}
}