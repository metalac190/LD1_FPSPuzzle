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
            // calculate spawn positions for readability
            Vector3 newPlayerPosition = gameManager.PlayerSpawn.playerPosition;
            Quaternion newPlayerRotation = gameManager.PlayerSpawn.playerRotation;
            ActivePlayer = Instantiate(playerToSpawn, newPlayerPosition, newPlayerRotation);
            // rename it so that we don't have the 'clone' tag after spawning
            ActivePlayer.gameObject.name = spawnedPlayerName;
            OnPlayerSpawn.Invoke(ActivePlayer);
            // listen for when the player dies, so we know when to despawn
            ActivePlayer.OnPlayerDeath += HandlePlayerDeath;
        }
    }

    void HandlePlayerDeath()
    {
        // stop listening, then handle despawn/respawn
        ActivePlayer.OnPlayerDeath -= HandlePlayerDeath;
        PlayerDespawn(despawnTimeAfterDeath);
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

    IEnumerator PlayerDespawn(float despawnTime)
    {
        yield return new WaitForSeconds(despawnTime);
        OnPlayerDespawn.Invoke(ActivePlayer);
        // destory the player, as the final step
        Destroy(ActivePlayer.gameObject);
    }
}
