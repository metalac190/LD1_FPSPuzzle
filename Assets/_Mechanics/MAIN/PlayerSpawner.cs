using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSpawner : MonoBehaviour {

    [SerializeField] Player playerToSpawn;   // Player prefab to spawn
    [SerializeField] string spawnedPlayerName = "Player";
    [SerializeField] Transform playerStartPosition; // player starting position

    public event Action<Player> OnPlayerSpawn = delegate { };

    CheckPoint[] checkPoints;       // list of all checkpoints in our level
    int activeCheckPointIndex = -1;        // start at -1 so that we can increase to 0 (the first checkpoint)

    GameManager gameManager;

    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
        // setup checkpoints in this level
        InitializeCheckPoints();
        DataManager.instance.SetPlayerSpawnLocation(playerStartPosition.position, playerStartPosition.rotation);
    }

    void HandleLevelLoad()
    {
        // setup checkpoints in this level
        InitializeCheckPoints();
    }

    void InitializeCheckPoints()
    {
        // store all level checkpoints for manipulation later
        checkPoints = FindObjectsOfType<CheckPoint>();
        // set the active checkpoint
        if (checkPoints != null)
            activeCheckPointIndex = 0;
    }

    // Advance to our next level checkpoint. Make it active an move there
    public void AdvanceCheckPoint()
    {
        // if there are no checkpoints, don't do anything
        if (checkPoints.Length == 0)
            return;

        // if we haven't hit a checkpoint yet (-1), start at 0
        if (activeCheckPointIndex < 0)
            activeCheckPointIndex = 0;
        // if we're at the end of the array, return to the start
        else if (activeCheckPointIndex >= checkPoints.Length - 1)
            activeCheckPointIndex = 0;
        // otherwise, advance!
        else
        {
            activeCheckPointIndex++;
        }
        // we hopped checkpoints, make sure we spawn there too
        Transform newLocation = checkPoints[activeCheckPointIndex].gameObject.transform;
        DataManager.instance.SetPlayerSpawnLocation(newLocation.position, newLocation.rotation);
        // reload so that we load in at the correct place
        SceneLoader.ReloadSceneStatic();
    }

    public void ReverseCheckPoint()
    {
        // if there aren't any checkpoints don't do anything
        if (checkPoints.Length == 0)
            return;

        // if we haven't hit a checkpoint yet, start at the end
        if (activeCheckPointIndex < 0)
            activeCheckPointIndex = checkPoints.Length - 1;
        // if we're at the beginning of our array, jump to the end
        else if (activeCheckPointIndex == 0)
            activeCheckPointIndex = checkPoints.Length - 1;
        // otherwise, previous checkpoint!
        else
        {
            activeCheckPointIndex--;
        }
        // we hopped checkpoints, make sure we spawn there too
        Transform newLocation = checkPoints[activeCheckPointIndex].gameObject.transform;
        DataManager.instance.SetPlayerSpawnLocation(newLocation.position, newLocation.rotation);
        // reload so that we load in at the correct place
        SceneLoader.ReloadSceneStatic();
    }

    public void SpawnPlayer()
    {
        Debug.Log("Player Spawned!");
        if (PlayerExists())
        {
            Debug.LogWarning("Player already exists, cannot spawn another.");
            return;
        }
        else
        {
            Vector3 newPlayerPosition = DataManager.instance.SavedPlayerSpawn.playerPosition;
            Quaternion newPlayerRotation = DataManager.instance.SavedPlayerSpawn.playerRotation;
            Player spawnedPlayer = Instantiate(playerToSpawn, newPlayerPosition, newPlayerRotation);
            // rename it so that we don't have the 'clone' tag after spawning
            spawnedPlayer.gameObject.name = spawnedPlayerName;
            // set in GameManager singleton so that it's easier to access
            gameManager.SetPlayer(spawnedPlayer);
            // notify anything else, if they care
            OnPlayerSpawn.Invoke(spawnedPlayer);
        }
    }

    public IEnumerator PlayerRespawn(float timeUntilSpawn)
    {
        yield return new WaitForSeconds(timeUntilSpawn);
        SpawnPlayer();
    }

    public bool PlayerExists()
    {
        Player testPlayer = FindObjectOfType<Player>();
        if (testPlayer != null)
            return true;
        else
            return false;
    }
}
