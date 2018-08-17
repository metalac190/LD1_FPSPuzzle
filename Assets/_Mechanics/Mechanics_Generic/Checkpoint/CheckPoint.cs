using UnityEngine;
using System.Collections;

/// <summary>
/// This script will receive an OnTriggerEnter() event, and if finding the player, will save player data to the GameManager. This is the primary
/// means of saving player progress, including inventory and respawn location.
/// NOTE: This scipt requires a Trigger Collider in order to function properly, so that it receives the TriggerEnter event
/// </summary>

[DisallowMultipleComponent]                         //Prevent multiple copies of this component to be added to the same gameObject

public class CheckPoint : MonoBehaviour {

    [Header("Spawn Settings")]
    [Tooltip("Position the player will respawn. Note that this is not necessarily the same position of this GameObject, to give designer more control")]
    public Transform checkPointSpawn;	//Spawn location when respawned

    [Header("SFX")]
    [Tooltip("Sound effect to play whenever the checkpoint is activated")]
    public AudioClip sfx_checkpointAchieved;	//sound effect when new check point is acquired

	private Vector3 spawnPosition;      //spawn position, in case one is not designated above

	//LevelManager levelManager;          //Reference to the level manager, so that we can update the player's spawn location on death
    //UIManager uiManager;            //Reference to UIManager

	void Awake(){

        //levelManager = FindObjectOfType<LevelManager>();
        //uiManager = FindObjectOfType<UIManager>();

        //If we haven't specified a spawn point, spawn above the checkpoint slightly
        if (checkPointSpawn == null)
        {
            spawnPosition = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        }
        //otherwise use the spawnpoint
        else
        {
            spawnPosition = checkPointSpawn.transform.position;
        }
    }

	void OnTriggerEnter(Collider other){

		if(other.tag == "Player"){
			//Update the player's starting location to the checkpoint's new spawn point
			//levelManager.UpdateSpawnLocation(spawnPosition);
            //We touched a checkpoint, save our progress! This also saves the player's new spawn location
            //GameManager.instance.SaveGame();
			//Feedback
			//SoundManager.instance.PlaySound2DOneShot(sfx_checkpointAchieved, 1f, false);		//Play Audio
            //uiManager.FlashScreen(Color.cyan, .25f);
		}
	}
}
