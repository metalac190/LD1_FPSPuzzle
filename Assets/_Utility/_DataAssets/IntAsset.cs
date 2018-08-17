using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataAssets
{
    [CreateAssetMenu(menuName = "DataAssets/IntAsset")]
    public class IntAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        public int initialValue;  //this is our initial value
                                  // we want to also keep track of our runtime value, separately
        [System.NonSerialized]
        public int value;  // runtime value that gets changed and edited. Not viewable in Inspector

        // when we're no longer in runtime, return runtime value back to the initial value
        public void OnAfterDeserialize()
        {
            value = initialValue;
        }
        // just keeping our Interface consistent
        public void OnBeforeSerialize() { }
    }
}
