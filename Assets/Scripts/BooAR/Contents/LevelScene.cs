using BooAR.Levels;
using Zenject;

namespace BooAR.Contents
{
	// Routes ILevelState instance to the root context via scene hierarchy
	public class LevelScene : BaseBehaviour
	{
		[Inject]
		ILevelState _level;

		public ILevelState LevelState => _level;
	}
}