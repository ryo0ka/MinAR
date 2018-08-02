using BooAR.Levels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace BooAR.Contents
{
	public class LevelInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<ILevelState>()
			         .To<LevelState>()
			         .AsSingle();
		}
	}
}