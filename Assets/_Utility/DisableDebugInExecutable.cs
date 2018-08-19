using UnityEngine;

/// <summary>
/// This class disables all Debug.Log statement calls from Unity, and controls whether or not we're in
/// debug state.
/// </summary>
public class DisableDebugInExecutable : MonoBehaviour
{
    private void Awake()
    {
        // this saves us from unnecessary performance hits in the final build
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }
}

