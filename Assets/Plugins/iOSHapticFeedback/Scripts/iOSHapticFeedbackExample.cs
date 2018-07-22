using UnityEngine;
using System.Collections;

public class iOSHapticFeedbackExample : MonoBehaviour {
    void OnGUI()
    {
        for (int i = 0; i < 7; i++)
        {
            if (GUI.Button(new Rect(0,i * 60, 300, 50), "Trigger "+(iOSHapticFeedback.iOSFeedbackType)i))
            {
                iOSHapticFeedback.Instance.Trigger((iOSHapticFeedback.iOSFeedbackType)i);
            }
        }
    }
}
