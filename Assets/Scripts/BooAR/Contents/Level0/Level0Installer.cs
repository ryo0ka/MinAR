using BooAR.Contents.Characters;
using UnityEngine;
using Zenject;

namespace BooAR.Contents.Level0
{
	public class Level0Installer : MonoInstaller
	{
		[SerializeField]
		GhostBoo _booPrefab;

		public override void InstallBindings()
		{
			Container.BindMemoryPool<GhostBoo, GhostBoo.Pool>()
			         .FromComponentInNewPrefab(_booPrefab);
		}
	}
}