using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// This script will add Kill functionality to this object. If an object with the Health script touches it, that thing will by killed.
/// Note that instant killing is different than taking damage. Apply this to 
/// lava pits/endless holes/or dangerous hazards that always kill player regardless of life.
/// NOTE: This script will only work if you have a Trigger volume attached to this GameObject, as KillLife happens on TriggerEnter()
/// </summary>
[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject
public class KillVolume : MonoBehaviour {

	//Kill the player if they touch the collider on this object
	void OnTriggerEnter(Collider other){
        // if the other thing has a health script, kill it
        Health health = other.GetComponent<Health>();
        if(health != null)
        {
            health.Kill();
        }
	}
}
