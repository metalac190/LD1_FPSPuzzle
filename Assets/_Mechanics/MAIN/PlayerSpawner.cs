using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSpawner : MonoBehaviour {


    public PlayerSpawnData PlayerSpawn { get; private set; }
    [SerializeField] private InventoryData savedInventory = new InventoryData();     // saved inventory that gets loaded into player on spawn

    [SerializeField] Player playerToSpawn;   // Player prefab to spawn
    [SerializeField] string spawnedPlayerName = "Player";
    [SerializeField] Transform playerStartPosition; // player starting position

    public event Action<Player> OnPlayerSpawn = delegate { };

    CheckPoint[] checkPoints;       // list of all checkpoints in our level
    int activeCheckPointIndex = -1;        // start at -1 so that we can increase to 0 (the first checkpoint)

    public void Initialize()
    {
        // events
        GameManager.instance.OnLevelLoad += HandleLevelLoad;

        PlayerSpawn = new PlayerSpawnData();

        // setup checkpoints in this level
        InitializeCheckPoints();
        SetPlayerSpawnLocation(playerStartPosition.position, playerStartPosition.rotation);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnLevelLoad -= HandleLevelLoad;
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
        SetPlayerSpawnLocation(newLocation.position, newLocation.rotation);
        // reload so that we load in at the correct place
        GameManager.instance.ReloadLevel();
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
        SetPlayerSpawnLocation(newLocation.position, newLocation.rotation);
        // reload so that we load in at the correct place
        GameManager.instance.ReloadLevel();
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
            Player spawnedPlayer = Instantiate(playerToSpawn,PlayerSpawn.playerPosition,PlayerSpawn.playerRotation);
            // rename it so that we don't have the 'clone' tag after spawning
            spawnedPlayer.gameObject.name = spawnedPlayerName;
            // set in GameManager singleton so that it's easier to access
            GameManager.instance.SetPlayer(spawnedPlayer);
            // notify anything else, if they care
            OnPlayerSpawn.Invoke(spawnedPlayer);
        }
    }

    public IEnumerator PlayerRespawn(float timeUntilSpawn)
    {
        yield return new WaitForSeconds(timeUntilSpawn);
        SpawnPlayer();
    }

    public void SetPlayerSpawnLocation(Vector3 newSpawnLocation, Quaternion newSpawnRotation)
    {
        // store these values
        PlayerSpawn.playerPosition = newSpawnLocation;
        PlayerSpawn.playerRotation = newSpawnRotation;
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
