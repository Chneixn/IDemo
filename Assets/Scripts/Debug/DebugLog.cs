using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLog : MonoBehaviour
{
    public bool EnableLog = false;
    private void OnEnable()
    {
        Debug.unityLogger.logEnabled = EnableLog;
    }
}
