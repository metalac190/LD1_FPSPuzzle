using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState { Wait, Game};
// Game Manager holds most of the level relevant game data
[RequireComponent(typeof(GameInputs))]
[RequireComponent(typeof(PlayerSpawner))]
public class GameManager : MonoBehaviour {

    public GameState CurrentGameState { get; private set; }
    // game state events
    public event Action OnWaitState = delegate { };
    public event Action OnGameState = delegate { };
    // game events
    public event Action OnPause = delegate { };
    public event Action OnUnpause = delegate { };
    public event Action OnCollectibleChange = delegate { };
    public event Action OnSave = delegate { };
    // level data (not persistent)
    [SerializeField] InventoryData playerInventory = new InventoryData();
    public InventoryData PlayerInventory { get { return playerInventory; } }
    [SerializeField] List<float> unsavedColledIDs = new List<float>();
    public List<float> UnsavedCollectedIDs { get { return unsavedColledIDs; } }
    // state variables
    public bool IsPaused = false;
    // reference to the player
    public Player ActivePlayer { get; private set; }

    GameInputs gameInputs;
    PlayerSpawner playerSpawner;

    public void Awake()
    {
        // fill local references
        CheckFilledReferences();
        // events
        SubscribeToEvents();
        // Initialize relevant scripts
        playerSpawner.Initialize(this);
        gameInputs.Initialize(this);
        // destroy any players in level, just in case
    }

    // if required references aren't filled, search instead
    void CheckFilledReferences()
    {
        playerSpawner = GetComponent<PlayerSpawner>();
        gameInputs = GetComponent<GameInputs>();
    }

    private void OnDisable()
    {
        playerSpawner.OnPlayerSpawn -= HandlePlayerSpawn;
        playerSpawner.OnPlayerDespawn -= HandlePlayerDespawn;
    }

    private void Start()
    {
        // set initial game state
        ActivateWaitState();
    }

    void SubscribeToEvents()
    {
        playerSpawner.OnPlayerSpawn += HandlePlayerSpawn;
        playerSpawner.OnPlayerDespawn += HandlePlayerDespawn;
    }

    void HandlePlayerSpawn(Player player)
    {
        // store the new player so that other objects can easily access it
        ActivePlayer = player;
    }

    void HandlePlayerDespawn(Player player)
    {
        // player has despawned. Reload our current scene
        SceneLoader.ReloadSceneStatic();
    }

    public void LoadPlayerData()
    {
        playerInventory = DataManager.Instance.SavedPlayerInventory;
    }

    public void ActivateWaitState()
    {
        CurrentGameState = GameState.Wait;
        OnWaitState.Invoke();
    }

    public void ActivateGameState()
    {
        CurrentGameState = GameState.Game;
        OnGameState.Invoke();
    }

    public void ActivatePause()
    {
        IsPaused = true;
        Time.timeScale = 0;
        FindObjectOfType<Player>().SetControllable(false);
        OnPause.Invoke();
    }

    public void ActivateUnpause()
    {
        IsPaused = false;
        Time.timeScale = 1;
        FindObjectOfType<Player>().SetControllable(true);
        OnUnpause.Invoke();
    }

    public void ActivateSave()
    {
        DataManager.Instance.SavePlayerInventory(playerInventory);
        DataManager.Instance.SaveCollectibleUIDs(UnsavedCollectedIDs);
        // clear out our unsaved list, since we moved them all to our saved list.
        UnsavedCollectedIDs.Clear();
        // let anything else that needs to save do their thing as well
        OnSave.Invoke();
    }

    public void AddCollectible(float uID, CollectibleType collectibleType)
    {
        // add the uID, so we know not to spawn it in the world on a future run
        // add to inventory based on collectible type
        switch (collectibleType)
        {
            case CollectibleType.small:
                PlayerInventory.smallCollectibles++;
                break;
            case CollectibleType.large:
                //TODO add the large collectible
                break;
            default:
                Debug.LogWarning("collectible type not valid");
                break;
        }
        // update UI to account for collectibles
        OnCollectibleChange.Invoke();
    }
}
