using System;
using UnityEngine;
using Zenject;

namespace BooAR
{
	public abstract class BaseBehaviour : MonoBehaviour
	{
		[SerializeField]
		bool _disableDebugLog;

		string _name;

		[Inject]
		void _OnInjected()
		{
			_name = ToString(); // for multi-threading
		}

		protected void Log(object message)
		{
			if (!_disableDebugLog)
				Debug.Log($"{message} -- ({_name})");
		}

		protected void LogWarning(object message)
		{
			Debug.LogWarning($"{message} -- ({_name})");
		}

		protected void LogException(Exception e)
		{
			if (!_disableDebugLog)
				Debug.LogException(e, this);
		}
	}
}