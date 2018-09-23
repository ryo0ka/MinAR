using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

namespace BooAR.Haptics
{
	/* Triggers given haptic feedback when the Selectable is selected
	 *
	 * 
	 */
	[RequireComponent(typeof(Selectable))]
	public class HapticSelectable : MonoBehaviour
	{
		[Inject]
		IHapticFeedbackGenerator _haptic;

		[SerializeField]
		HapticFeedbackTypes _type;

		void Start()
		{
			Selectable s = GetComponent<Selectable>();
			s.OnSelectAsObservable().Subscribe(_ => _haptic.Trigger(_type));
		}
	}
}