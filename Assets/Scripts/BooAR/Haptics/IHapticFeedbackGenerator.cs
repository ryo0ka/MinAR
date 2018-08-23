namespace BooAR.Haptics
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

	public static class HapticFeedbackGeneratorUtils
	{
		public static void TriggerSelected(this IHapticFeedbackGenerator h, bool selected)
		{
			if (selected)
			{
				h.Trigger(HapticFeedbackTypes.Selection);
			}
			else
			{
				h.Trigger(HapticFeedbackTypes.ImpactLight);
			}
		}
	}
}