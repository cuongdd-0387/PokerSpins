using System.Runtime.InteropServices;
using UnityEngine;

public class HandleLogError : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void LogErrorOfGame(string output, string stack);

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLogThreeded;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLogThreeded;
    }

    void HandleLogThreeded(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception) {
            Debug.Log(logString+ " ==== "+ stackTrace);
#if !UNITY_EDITOR
            LogErrorOfGame(logString, stackTrace);
#endif
        }
    }
}
