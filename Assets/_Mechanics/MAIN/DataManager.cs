using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour {

    public static DataManager instance = null;

    public PlayerSpawnData SavedPlayerSpawn { get; private set; }
    public InventoryData SavedInventory { get; private set; }

    private List<LevelCollectibleData> savedLevelCollectibles = new List<LevelCollectibleData>();
    public List<float> SavedItemIDs { get; private set; }

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

    void Initialize()
    {
        // events
        SceneManager.sceneLoaded += OnSceneLoaded;
        // local state
        SavedPlayerSpawn = new PlayerSpawnData();
        SavedInventory = new InventoryData();
    }

    // Called anytime a new scene is loaded/reloaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    public void SaveInventory(InventoryData newInventory)
    {
        SavedInventory = newInventory;
    }

    public void SetPlayerSpawnLocation(Vector3 newSpawnLocation, Quaternion newSpawnRotation)
    {
        // store these values
        SavedPlayerSpawn.playerPosition = newSpawnLocation;
        SavedPlayerSpawn.playerRotation = newSpawnRotation;
    }
}
