using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Support class for the GameEventAsset. Lets you test events with a button press
/// </summary>

namespace DataAssets
{
    [CustomEditor(typeof(GameEventAsset))]
    public class GameEventAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // inspector default draw
            DrawDefaultInspector();

            // get a reference to the other script
            GameEventAsset gameEventAsset = (GameEventAsset)target;
            // when the button is pressed, activate the function
            if (GUILayout.Button("Raise"))
            {
                gameEventAsset.Raise();
            }
        }
    }
}

