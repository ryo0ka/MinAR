using UnityEngine;
using Utils;

namespace BooAR.Games
{
	public class PausePresenter : BaseBehaviour
	{
		[SerializeField]
		GameObject _recoveryText;
		
		public void SetState(bool paused, bool recovering)
		{
			gameObject.SetActive(paused);
			_recoveryText.SetActive(recovering);
		}
	}
}