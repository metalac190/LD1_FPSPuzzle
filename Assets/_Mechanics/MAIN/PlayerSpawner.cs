using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSpawner : MonoBehaviour {

    [SerializeField] Player playerToSpawn;   // Player prefab to spawn
    [SerializeField] string spawnedPlayerName = "Player";
    //[SerializeField] Transform playerStartPosition; // player starting position
    [SerializeField] float despawnTimeAfterDeath = 2f;

    public event Action<Player> OnPlayerSpawn = delegate { };
    public event Action<Player> OnPlayerDespawn = delegate { };

    [SerializeField] Transform playerStart;
    CheckPoint[] checkPoints;       // list of all checkpoints in our level
    int activeCheckPointIndex = -1;        // start at -1 so that we can increase to 0 (the first checkpoint)

    public Player ActivePlayer { get; private set; }
    GameManager gameManager;

    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
        // events
        gameManager.OnGameState += HandleGameState;
        // setup checkpoints in this level
        InitializeCheckPoints();
        //DataManager.instance.SetPlayerSpawnLocation(playerStartPosition.position, playerStartPosition.rotation);
    }

    void HandleGameState()
    {
        // the game has started
        SpawnPlayer();
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
        DataManager.Instance.SetPlayerSpawn(newLocation.position, newLocation.rotation);
        // move player to the new checkpoint
        ActivePlayer.transform.position = newLocation.position;
        ActivePlayer.transform.rotation = newLocation.rotation;
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
        DataManager.Instance.SetPlayerSpawn(newLocation.position, newLocation.rotation);
        // move player to the new checkpoint
        ActivePlayer.transform.position = newLocation.position;
        ActivePlayer.transform.rotation = newLocation.rotation;
    }

    public void SpawnPlayer()
    {
        Debug.Log("Spawn Player!");
        if (PlayerExists())
        {
            Debug.LogWarning("Player al  ready exists, cannot spawn another.");
            return;
        }
        // calculate spawn positions for readability
        Vector3 newSpawnPosition = DetermineSpawnPosition();
        Quaternion newSpawnRotation = DetermineSpawnRotation();
        // spawn the player at the new location
        ActivePlayer = Instantiate(playerToSpawn, newSpawnPosition, newSpawnRotation);
        // rename it so that we don't have the 'clone' tag after spawning
        ActivePlayer.gameObject.name = spawnedPlayerName;
        // let subscribers know
        OnPlayerSpawn.Invoke(ActivePlayer);
        // listen for when the player dies, so we know when to despawn
        ActivePlayer.OnPlayerDeath += HandlePlayerDeath;
    }

    Vector3 DetermineSpawnPosition()
    {
        // if we're respawning at level start, use that. Otherwise pull from saved spawn point
        if (DataManager.Instance.SavedPlayerSpawn.usePlayerStart)
        {
            return playerStart.position;
        }
        else
        {
            return DataManager.Instance.SavedPlayerSpawn.playerPosition;
        }
    }

    Quaternion DetermineSpawnRotation()
    {
        // if we're respawning at level start, use that. Otherwise pull from saved spawn point
        if (DataManager.Instance.SavedPlayerSpawn.usePlayerStart)
        {
            return playerStart.rotation;
        }
        else
        {
            return DataManager.Instance.SavedPlayerSpawn.playerRotation;
        }
    }

    void HandlePlayerDeath()
    {
        // stop listening, then handle despawn/respawn
        ActivePlayer.OnPlayerDeath -= HandlePlayerDeath;
        StartCoroutine(PlayerDespawn(despawnTimeAfterDeath));
    }

    bool PlayerExists()
    {
        Player testPlayer = FindObjectOfType<Player>();
        if (testPlayer != null)
            return true;
        else
            return false;
    }

    public IEnumerator PlayerRespawn(float timeUntilSpawn)
    {
        yield return new WaitForSeconds(timeUntilSpawn);
        SpawnPlayer();
    }

    IEnumerator PlayerDespawn(float despawnTime)
    {
        yield return new WaitForSeconds(despawnTime);
        OnPlayerDespawn.Invoke(ActivePlayer);
        // destory the player, as the final step
        Destroy(ActivePlayer.gameObject);
        // no player, we are now in wait state
        gameManager.ActivateWaitState();
    }
}
