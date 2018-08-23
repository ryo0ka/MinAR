using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BooAR.Characters
{
	public class CharactersInstaller : MonoInstaller
	{
		[SerializeField]
		Transform _uiRoot;

		[SerializeField]
		Toggle _jackTogglePrefab;

		[SerializeField]
		Transform _doorButtonRoot;

		[SerializeField]
		Button _doorButtonPrefab;

		public override void InstallBindings()
		{
			Container.BindMemoryPool<Toggle, JackTogglePool>()
			         .FromComponentInNewPrefab(_jackTogglePrefab)
			         .UnderTransform(_uiRoot);

			Container.BindMemoryPool<Button, DoorButtonPool>()
			         .FromComponentInNewPrefab(_doorButtonPrefab)
			         .UnderTransform(_doorButtonRoot);
		}
	}

	public class JackTogglePool : MonoMemoryPool<Toggle>
	{
	}

	public class DoorButtonPool : MonoMemoryPool<Button>
	{
	}
}