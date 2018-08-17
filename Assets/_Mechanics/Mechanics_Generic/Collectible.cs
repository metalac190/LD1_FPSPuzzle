using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    public float uID;       // unique identifier

    private void Awake()
    {
        // do a crazy calculation to get a seemingly unique identifying value for this object
        uID = transform.position.sqrMagnitude;
    }

    public void Collect()
    {
        // add to local player inventory, until stored at a checkpoint
        CollectibleData collectible = new CollectibleData();
        collectible.uID = uID;
        
    }

}
