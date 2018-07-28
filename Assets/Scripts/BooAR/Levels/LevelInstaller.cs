using UnityEngine;
using Zenject;

namespace BooAR.Levels
{
	public class LevelInstaller : MonoInstaller
	{
		[SerializeField]
		LevelState _levelState;
		
		public override void InstallBindings()
		{
			Container.BindInstance<ILevelState>(_levelState);
		}
	}
}