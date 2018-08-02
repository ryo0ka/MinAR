using BooAR.Levels;
using UniRx;

namespace BooAR.Games
{
	public class GameState : IGameState
	{
		readonly ReactiveProperty<GameStateTypes> _state = new ReactiveProperty<GameStateTypes>();
		readonly ReactiveProperty<bool> _pause = new ReactiveProperty<bool>();

		public IReactiveProperty<GameStateTypes> State => _state;
		public IReactiveProperty<bool> Pause => _pause;

		public ILevelState Level { get; set; }
		public int Index { get; set; }
	}
}