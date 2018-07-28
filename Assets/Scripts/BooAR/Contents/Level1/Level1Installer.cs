using BooAR.Contents.Characters;
using UnityEngine;
using Zenject;

namespace BooAR.Contents.Level1
{
	public class Level1Installer : MonoInstaller
	{
		[SerializeField]
		GhostSiren _sirenPrefab;
		
		public override void InstallBindings()
		{
			Container.BindMemoryPool<GhostSiren, GhostSiren.Pool>()
			         .FromComponentInNewPrefab(_sirenPrefab);
		}
	}
}