using BooAR.Levels;
using Zenject;

namespace BooAR.Contents
{
	public class LevelScene : BaseBehaviour
	{
		[Inject]
		ILevelState _level;

		public ILevelState LevelState => _level;
	}
}