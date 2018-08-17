using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This asset will allow you to store an event call with hooked up event listeners to file. This is useful as you
/// don't need to find the event hookups at runtime, and lets you call it whenever desired
/// </summary>

namespace DataAssets
{
    [CreateAssetMenu(menuName = "DataAssets/GameEventAsset")]
    public class GameEventAsset : ScriptableObject
    {
        // list of game event listeners
        private List<GameEventListenerAsset> listeners = new List<GameEventListenerAsset>();

        // loop through listeners and call the events
        public void Raise()
        {
            // go down our list backwards, so we can clean up along the way
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }
        // add the listener to the list
        public void RegisterListener(GameEventListenerAsset listener)
        {
            listeners.Add(listener);
        }
        // remove the listener to the list
        public void UnregisterListener(GameEventListenerAsset listener)
        {
            listeners.Remove(listener);
        }
    }

}
