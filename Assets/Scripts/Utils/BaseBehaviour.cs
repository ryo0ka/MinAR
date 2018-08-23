using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utils
{
	public abstract class BaseBehaviour : SerializedMonoBehaviour
	{
		[SerializeField]
		bool _disableDebugLog;

		protected void Log(object message)
		{
			if (!_disableDebugLog)
				Debug.Log($"{message} -- ({this})");
		}

		protected void LogWarning(object message)
		{
			Debug.LogWarning($"{message} -- ({this})");
		}

		protected void LogException(Exception e)
		{
			if (!_disableDebugLog)
				Debug.LogException(e, this);
		}

		protected void Assert(bool condition, object message = null)
		{
			Debug.Assert(condition, $"{message ?? ""} -- ({this})");
		}
	}
}