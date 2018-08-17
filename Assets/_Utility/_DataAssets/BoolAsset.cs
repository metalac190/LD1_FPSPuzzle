using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Scriptable Object asset that allows us to use a dataType on disk. This is useful to avoid unnecessary script references.
/// When editing this value inside of a script, make sure to edit the 'value' variable. Value will be reset back to the
/// initial value after runtime is exited

namespace DataAssets
{
    [CreateAssetMenu(menuName = "DataAssets/BoolAsset")]
    public class BoolAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public bool initialValue;  //this is our initial value
        // we want to also keep track of our runtime value, separately
        [System.NonSerialized]
        public bool value;  // runtime value that gets changed and edited. Not viewable in Inspector

        // when we're no longer in runtime, return runtime value back to the initial value
        public void OnAfterDeserialize()
        {
            value = initialValue;
        }
        // just keeping our Interface consistent
        public void OnBeforeSerialize() { }
    }
}

