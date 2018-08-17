using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSpawner))]
[RequireComponent(typeof(LevelSpawner))]
public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public InventoryData SavedInventory { get; private set; }

    public event Action OnSave = delegate { };  // fires anytime the Save Game event is invoked
    public event Action OnLevelLoad = delegate { };     // fires when a level is loaded

    public Player Player { get; private set; }

    PlayerSpawner playerSpawner;
    LevelSpawner levelSpawner;

    private void Awake()
    {
        ///SingleTon Pattern!
        // check if there is already an instance of SoundManager
        if (instance == null)
        {
            instance = this;    // if not, set it to this
            DontDestroyOnLoad(gameObject);      // this object can no longer be destroyed
            // initialize new game
            Initialize();
        }
        //If instance already exists:
        else if (instance != this)
        {
            //Destroy this, to enforce our singleton pattern and make sure this is the only instance of SoundManager
            Destroy(gameObject);
        }
    }

    // Called anytime a new scene is loaded/reloaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnLevelLoad.Invoke();
    }

    public void SaveGame()
    {
        OnSave.Invoke();
    }

    public void Initialize()
    {
        // events
        SceneManager.sceneLoaded += OnSceneLoaded;
        // get local references
        playerSpawner = GetComponent<PlayerSpawner>();
        levelSpawner = GetComponent<LevelSpawner>();
        // initialize relevant scripts
        playerSpawner.Initialize();
        levelSpawner.Initialize();
        // local state
        SavedInventory = new InventoryData();
    }

    public void ReloadLevel()
    {
        // Reload the current scene, using the updated player spawn location
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveInventory(InventoryData newInventory)
    {
        SavedInventory = newInventory;
    }

    public void SetPlayer(Player newPlayer)
    {
        Player = newPlayer;
    }
}
