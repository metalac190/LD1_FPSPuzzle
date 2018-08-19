using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// DataManager holds most of the persistent game data, and is responsible for holding it. Other scripts
/// can access this at any point as it's a Singleton
public class DataManager : MonoBehaviour {
    
    private static DataManager instance = null;
    // lazy load
    public static DataManager Instance
    {
        // only create this object the first time it is accessed
        get
        {
            if(instance == null)
            {
                // check to make sure it doesn't already exist
                instance = FindObjectOfType<DataManager>();
                // none exist, let's create it
                if(instance == null)
                {
                    GameObject go = new GameObject(typeof(DataManager).ToString());
                    instance = go.AddComponent<DataManager>();
                }
            }
            return instance;
        }
    }

    public event Action OnSave = delegate { };

    [SerializeField] PlayerSpawnData savedPlayerSpawn = new PlayerSpawnData();      // player spawn data
    public PlayerSpawnData SavedPlayerSpawn { get { return savedPlayerSpawn; } }
    [SerializeField] InventoryData savedPlayerInventory = new InventoryData();      // player inventory data
    public InventoryData SavedPlayerInventory { get { return savedPlayerInventory; } }

    //private List<LevelCollectibleData> savedLevelCollectibles = new List<LevelCollectibleData>();

    [SerializeField] List<float> collectedUIDs = new List<float>();     // player collected item ids
    public List<float> CollectedUIDs { get { return collectedUIDs; } }

    private void Awake()
    {
        ///SingleTon Pattern! with Lazy Instantiation
        // check if there is already an instance of DataManager
        if (Instance != this)
        {
            //Destroy this, to enforce our singleton pattern and make sure this is the only instance of DataManager
            Destroy(gameObject);

        }
        // we are the first, this is now our singleton
        else
        {
            DontDestroyOnLoad(gameObject);      // this object can no longer be destroyed
            // initialize our data
            Initialize();
        }
    }

    void Initialize()
    {
        // events
        SceneManager.sceneLoaded += OnSceneLoaded;
        // local state
        //savedPlayerSpawn = new PlayerSpawnData();
        //savedPlayerInventory = new InventoryData();
        //collectedUIDs = new List<float>();
    }

    // Called anytime a new scene is loaded/reloaded. Optional, just wanted the hookup for now
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    public void ActivateSave()
    {
        Debug.Log("...Saving...");
        OnSave.Invoke();
    }

    // Change our saved value, persistent
    public void SavePlayerInventory(InventoryData playerInventoryToSave)
    {
        SavedPlayerInventory.smallCollectibles = playerInventoryToSave.smallCollectibles;
        SavedPlayerInventory.largeCollectibles = playerInventoryToSave.largeCollectibles;
    }

    // hold on to the temporary new player spawn
    public void SetPlayerSpawn(Vector3 playerPosition, Quaternion playerRotation)
    {
        // if we're setting a new player spawn, we can assume we no longer need the default start point
        SavedPlayerSpawn.usePlayerStart = false;
        // store these values
        SavedPlayerSpawn.sceneID = SceneLoader.GetCurrentSceneIndex();
        SavedPlayerSpawn.playerPosition = playerPosition;
        SavedPlayerSpawn.playerRotation = playerRotation;
    }

    // Change our saved value, persistent
    public void SaveCollectibleUIDs(List<float> collectibleUIDsToSave)
    {
        // add each UID from the unsaved list to our saved list
        foreach(float uID in collectibleUIDsToSave)
        {
            collectedUIDs.Add(uID);
        }
    }
}
