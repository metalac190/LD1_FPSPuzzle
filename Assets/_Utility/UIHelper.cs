using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UIHelper
{
    /// <summary>
    /// Turns off all panels except for 1. Technically it turns them all off, then reenables the 1.
    /// </summary>
    /// <param name="panelToIsolate"></param>
    /// <param name="panelsToDisable"></param>
    public static void IsolatePanel(GameObject panelToIsolate, GameObject[] panelsToDisable)
    {
        // turn them all off, so that we can re-enable the one we want
        foreach(GameObject panel in panelsToDisable)
        {
            panel.SetActive(false);
        }
        // then enable only the panel that we want
        panelToIsolate.SetActive(true);
    }
}

