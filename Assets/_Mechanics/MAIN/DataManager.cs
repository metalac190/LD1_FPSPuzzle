using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour {

    public static DataManager instance = null;

    public PlayerSpawnData SavedPlayerSpawn { get; private set; }
    public InventoryData SavedPlayerInventory { get; private set; }

    private List<LevelCollectibleData> savedLevelCollectibles = new List<LevelCollectibleData>();
    public List<float> SavedItemIDs { get; private set; }

    private void Awake()
    {
        ///SingleTon Pattern!
        // check if there is already an instance of DataManager
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
            //Destroy this, to enforce our singleton pattern and make sure this is the only instance of DataManager
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        // events
        SceneManager.sceneLoaded += OnSceneLoaded;
        // local state
        SavedPlayerSpawn = new PlayerSpawnData();
        SavedPlayerInventory = new InventoryData();
    }

    // Called anytime a new scene is loaded/reloaded. Optional, just wanted the hookup for now
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    public void SavePlayerInventory(InventoryData playerInventoryToSave)
    {
        SavedPlayerInventory.smallCollectibles = playerInventoryToSave.smallCollectibles;
        SavedPlayerInventory.largeCollectibles = playerInventoryToSave.largeCollectibles;
        SavedPlayerInventory.keys = playerInventoryToSave.keys;
    }

    public void SavePlayerSpawn(PlayerSpawnData playerSpawnToSave)
    {
        // store these values
        SavedPlayerSpawn.playerPosition = playerSpawnToSave.playerPosition;
        SavedPlayerSpawn.playerRotation = playerSpawnToSave.playerRotation;
    }
}
