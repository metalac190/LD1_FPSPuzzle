using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility {

    //Change all child objects to the new layer. Recursive because each object can only see one child down
    public static void SetLayerRecursively(GameObject _gameObject, int _newLayer)
    {
        //Error checking. Exit early if the object we passed in is null
        if (_gameObject == null)
            return;

        //Change the layer of our root objects
        _gameObject.layer = _newLayer;
        //Change the layer of all child objects
        foreach (Transform _child in _gameObject.transform)
        {
            //Recursive! Call this same method, keep moving down the hierarchy
            SetLayerRecursively(_child.gameObject, _newLayer);
        }
    }
}
