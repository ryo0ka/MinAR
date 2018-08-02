using System;

namespace BooAR.Games.Views
{
	public class TouchHapticFeedbackGenerator : IHapticFeedbackGenerator
	{
		public void Trigger(HapticFeedbackTypes type)
		{
			iOSHapticFeedback.Instance.Trigger(Convert(type));
		}

		iOSHapticFeedback.iOSFeedbackType Convert(HapticFeedbackTypes t)
		{
			switch (t)
			{
				case HapticFeedbackTypes.None: return iOSHapticFeedback.iOSFeedbackType.None;
				case HapticFeedbackTypes.ImpactLight: return iOSHapticFeedback.iOSFeedbackType.ImpactLight;
				case HapticFeedbackTypes.ImpactMedium: return iOSHapticFeedback.iOSFeedbackType.ImpactMedium;
				case HapticFeedbackTypes.ImpactHeavy: return iOSHapticFeedback.iOSFeedbackType.ImpactHeavy;
				case HapticFeedbackTypes.Selection: return iOSHapticFeedback.iOSFeedbackType.SelectionChange;
				case HapticFeedbackTypes.Success: return iOSHapticFeedback.iOSFeedbackType.Success;
				case HapticFeedbackTypes.Warning: return iOSHapticFeedback.iOSFeedbackType.Warning;
				case HapticFeedbackTypes.Failure: return iOSHapticFeedback.iOSFeedbackType.Failure;
				default: throw new ArgumentOutOfRangeException(nameof(t), t, null);
			}
		}
	}
}