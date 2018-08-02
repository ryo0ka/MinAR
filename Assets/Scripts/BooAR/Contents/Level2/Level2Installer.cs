using BooAR.Contents.Characters;
using UnityEngine;
using Zenject;

namespace BooAR.Contents.Level2
{
	public class Level2Installer : MonoInstaller
	{
		[SerializeField]
		GhostBoo _booPrefab;

		[SerializeField]
		GhostSiren _sirenPrefab;

		public override void InstallBindings()
		{
			Container.BindMemoryPool<GhostBoo, GhostBoo.Pool>()
			         .FromComponentInNewPrefab(_booPrefab);

			Container.BindMemoryPool<GhostSiren, GhostSiren.Pool>()
			         .FromComponentInNewPrefab(_sirenPrefab);
		}
	}
}