using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum GameState { Wait, Game};
// Game Manager holds most of the level relevant game data
[RequireComponent(typeof(GameInputs))]
[RequireComponent(typeof(PlayerSpawner))]
[RequireComponent(typeof(CollectibleSpawner))]
[RequireComponent(typeof(CollectibleManager))]
public class GameManager : MonoBehaviour {

    public GameState CurrentGameState { get; private set; }
    // game state events
    public event Action OnWaitState = delegate { };
    public event Action OnGameState = delegate { };

    // game events
    public event Action OnSave = delegate { };
    public event Action OnPause = delegate { };
    public event Action OnUnpause = delegate { };

    // state variables
    public bool IsPaused { get; private set; }
    // easy-to-access variables
    public Player ActivePlayer { get; private set; }

    GameInputs gameInputs;
    PlayerSpawner playerSpawner;
    CollectibleSpawner collectibleSpawner;
    CollectibleManager collectibleManager;

    public void Awake()
    {
        // fill local references
        CheckFilledReferences();
        // events
        SubscribeToEvents();
        // Initialize relevant scripts
        playerSpawner.Initialize(this);
        gameInputs.Initialize(this);
        collectibleSpawner.Initialize(this);
        collectibleManager.Initialize(this);
    }

    // if required references aren't filled, search instead
    void CheckFilledReferences()
    {
        playerSpawner = GetComponent<PlayerSpawner>();
        gameInputs = GetComponent<GameInputs>();
        collectibleSpawner = GetComponent<CollectibleSpawner>();
        collectibleManager = GetComponent<CollectibleManager>();
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
        ActivePlayer.SetControllable(false);
        OnPause.Invoke();
    }

    public void ActivateUnpause()
    {
        IsPaused = false;
        Time.timeScale = 1;
        ActivePlayer.SetControllable(true);
        OnUnpause.Invoke();
    }

    public void ActivateSave()
    {
        OnSave.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ActivateSave();
        }
    }
}
