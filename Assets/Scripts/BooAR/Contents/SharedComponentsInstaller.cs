using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BooAR.Contents
{
	public class SharedComponentsInstaller : MonoInstaller
	{
		[SerializeField]
		Transform _rootUI;

		[SerializeField]
		Toggle _jackTogglePrefab;

		public override void InstallBindings()
		{
			Container.BindMemoryPool<Toggle, JackTogglePool>()
			         .FromComponentInNewPrefab(_jackTogglePrefab)
			         .UnderTransform(_rootUI);
		}
	}

	public class JackTogglePool : MonoMemoryPool<Toggle>
	{
	}
}