using UniRx;

namespace BooAR.Levels
{
	public class LevelState : ILevelState
	{
		readonly ReactiveProperty<FloorOptions?> _begun = new ReactiveProperty<FloorOptions?>();
		readonly ReactiveProperty<bool> _goaled = new ReactiveProperty<bool>();
		readonly ReactiveProperty<bool> _failed = new ReactiveProperty<bool>();
		readonly ReactiveProperty<bool> _cancelled = new ReactiveProperty<bool>();

		public IReactiveProperty<FloorOptions?> Begun => _begun;
		public IReactiveProperty<bool> Goaled => _goaled;
		public IReactiveProperty<bool> Failed => _failed;
		public IReactiveProperty<bool> Cancelled => _cancelled;
	}
}