using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState { Wait, Game};
// Game Manager holds most of the level relevant game data
[RequireComponent(typeof(PlayerSpawner))]
[RequireComponent(typeof(LevelSpawner))]
[RequireComponent(typeof(GameInputs))]
public class GameManager : MonoBehaviour {

    public GameState CurrentGameState { get; private set; }
    // game state events
    public event Action OnWaitState = delegate { };
    public event Action OnGameState = delegate { };
    // game events
    public event Action OnPause = delegate { };
    public event Action OnUnpause = delegate { };
    public event Action OnCollectibleChange = delegate { };
    // level data (not persistent)
    [SerializeField] InventoryData playerInventory = new InventoryData();
    public InventoryData PlayerInventory { get { return playerInventory; } }
    [SerializeField] List<float> unsavedColledIDs = new List<float>();
    public List<float> UnsavedCollectedIDs { get { return unsavedColledIDs; } }
    // state variables
    public bool IsPaused = false;

    public void Awake()
    {
        // load game data 
        InitializeData();
        Load();
        // Initialize relevant scripts
        GetComponent<PlayerSpawner>().Initialize(this);
        GetComponent<LevelSpawner>().Initialize(this);
        GetComponent<GameInputs>().Initialize(this);
        FindObjectOfType<UIManager>().Initialize(this);
        // set initial game state
        ActivateWaitState();
    }

    void InitializeData()
    {
        //unsavedCollectedIDs = new List<float>();
        //playerInventory = new InventoryData();
    }

    public void Load()
    {
        Debug.Log("Load...");
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

    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0;
        //TODO lock player inputs
        OnPause.Invoke();
    }

    public void Unpause()
    {
        IsPaused = false;
        Time.timeScale = 1;
        //TODO lock player inputs
        OnUnpause.Invoke();
    }

    public void Save()
    {
        Debug.Log("Save...");
        DataManager.Instance.SavePlayerInventory(playerInventory);
        DataManager.Instance.SaveCollectibleUIDs(UnsavedCollectedIDs);
        // clear out our unsaved list, since we moved them all to our saved list.
        UnsavedCollectedIDs.Clear();
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
