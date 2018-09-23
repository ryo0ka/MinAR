using System.Linq;
using System.Text.RegularExpressions;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace BooAR.Apps
{
	public class GameSavePrompt : BaseBehaviour
	{
#pragma warning disable 649
		[SerializeField]
		Button _cancelButton;

		[SerializeField]
		InputField _field;

		[SerializeField]
		Button _confirmButton;

		[Inject]
		IGameList _gameList;
#pragma warning restore 649

		public async UniTask<string> Open()
		{
			using (UniRxUtils.Toggle(SetVisible)) // open the panel, and close when done
			using (CompositeDisposable e = new CompositeDisposable())
			{
				_field.OnValueChangedAsObservable()
				      .Select(t => CheckValidId(t))
				      .Subscribe(v => _confirmButton.interactable = v)
				      .AddTo(e);

				while (!CheckValidId(_field.text))
				{
					bool confirmed = await UniRxUtils.First(
						_confirmButton.OnClickAsObservable().Select(_ => true),
						_cancelButton.OnClickAsObservable().Select(_ => false));

					if (!confirmed) return null;
				}

				return _field.text;
			}
		}

		bool CheckValidId(string id)
		{
			return !string.IsNullOrEmpty(id) &&
			       !_gameList.GetGameIDs().Contains(id) &&
			       !Regex.IsMatch(id, @"[^A-Za-z0-9]");
		}

		void SetVisible(bool visible)
		{
			gameObject.SetActive(visible);
		}
	}
}