namespace BooAR.Games.Views
{
	public enum HapticFeedbackTypes
	{
		None,
		ImpactLight,
		ImpactMedium,
		ImpactHeavy,
		Selection,
		Success,
		Warning,
		Failure,
	}
	
	public interface IHapticFeedbackGenerator
	{
		void Trigger(HapticFeedbackTypes type);
	}
}