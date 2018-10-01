using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SmallCollectible : Collectible {

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Collect();
        }
    }
}
