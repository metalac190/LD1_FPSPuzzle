using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// DataManager holds most of the persistent game data, and is responsible for holding it. Other scripts
/// can access this at any point as it's a Singleton
public class DataManager : MonoBehaviour {

    #region Singleton - Lazy Load
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
    #endregion
    [SerializeField] PlayerSpawnData savedPlayerSpawn = new PlayerSpawnData();
    public PlayerSpawnData SavedPlayerSpawn { get { return savedPlayerSpawn; } }
    [SerializeField] CollectibleInventory savedCollectibles = new CollectibleInventory();
    public CollectibleInventory SavedCollectibles { get { return savedCollectibles; } }
    [SerializeField] List<LevelSavedCollectibleUIDs> savedGameUIDs = new List<LevelSavedCollectibleUIDs>();
    public List<LevelSavedCollectibleUIDs> SavedGameUIDs { get { return savedGameUIDs; } }

    private void Awake()
    {
        #region Singleton - Lazy Lode
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
        #endregion
    }

    void Initialize()
    {
        // events
        SceneManager.sceneLoaded += OnSceneLoaded;
        // local state
        SetDefaults();
    }

    // Called anytime a new scene is loaded/reloaded. Optional, just wanted the hookup for now
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateLevelList();
    }

    void SetDefaults()
    {

    }

    // hold on to the temporary new player spawn
    public void SetPlayerSpawn(Vector3 playerPosition, Quaternion playerRotation)
    {
        // if we're setting a new player spawn, we can assume we no longer need the default start point
        savedPlayerSpawn.usePlayerStart = false;
        // store these values
        savedPlayerSpawn.sceneID = SceneLoader.GetCurrentSceneIndex();
        savedPlayerSpawn.playerPosition = playerPosition;
        savedPlayerSpawn.playerRotation = playerRotation;
    }

    // Change our saved value, persistent
    public void SaveCollectibleInventory(CollectibleInventory newCollectibleInventory)
    {
        savedCollectibles = newCollectibleInventory;
    }

    public void SaveCollectibleUIDs(List<float> collectibleUIDsToSave)
    {
        float uID;
        // cycle through each of the UIDs in our list to save, and put them in our current scene list
        for (int i = 0; i < collectibleUIDsToSave.Count; i++)
        {
            uID = collectibleUIDsToSave[i];
            GetCurrentLevelUIDList().savedCollectedUIDs.Add(uID);
        }
    }

    public LevelSavedCollectibleUIDs GetCurrentLevelUIDList()
    {
        // find the list that matches current level index
        for (int i = 0; i < savedGameUIDs.Count; i++)
        {
            if (savedGameUIDs[i].sceneID == SceneManager.GetActiveScene().buildIndex)
            {
                return savedGameUIDs[i];
            }
        }
        // otherwise there are none. Make a new one.
        LevelSavedCollectibleUIDs newLevelList = new LevelSavedCollectibleUIDs(SceneManager.GetActiveScene().buildIndex);
        return newLevelList;
    }

    private void CreateLevelList()
    {
        if(savedGameUIDs == null)
        {
            CreateNewGameList();
        }

        if (DoesLevelListAlreadyExist())
        {
            return;
        }
        else
        {
            CreateNewLevelList();
        }
    }

    private void CreateNewGameList()
    {
        savedGameUIDs = new List<LevelSavedCollectibleUIDs>();
    }

    bool DoesLevelListAlreadyExist()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        for (int i = 0; i < savedGameUIDs.Count; i++)
        {
            if (savedGameUIDs[i].sceneID == currentSceneIndex)
            {
                return true;
            }
        }
        // we've made it far, haven't found an existing list for this level
        return false;
    }

    public void CreateNewLevelList()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        LevelSavedCollectibleUIDs newLevelList = new LevelSavedCollectibleUIDs(currentSceneIndex);
        savedGameUIDs.Add(newLevelList);
    }
}
