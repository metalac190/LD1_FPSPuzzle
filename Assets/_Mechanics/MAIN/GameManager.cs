using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSpawner))]
[RequireComponent(typeof(LevelSpawner))]
[RequireComponent(typeof(GameInputs))]
public class GameManager : MonoBehaviour {

    public enum GameState { Intro, Game };
    public GameState CurrentGameState { get; private set; }

    public event Action OnIntroState = delegate { };
    public event Action OnGameState = delegate { };

    public PlayerSpawnData PlayerSpawn { get; private set; }
    public InventoryData PlayerInventory { get; private set; }

    public List<float> UnSavedItemIDs { get; private set; }

    public void Initialize()
    {
        // Initialize relevant scripts
        GetComponent<PlayerSpawner>().Initialize(this);
        GetComponent<LevelSpawner>().Initialize(this);
        GetComponent<GameInputs>().Initialize(this);
        // set initial game state
        ActivateIntroState();
    }

    public void ActivateIntroState()
    {
        CurrentGameState = GameState.Intro;
        OnIntroState.Invoke();
    }

    public void ActivateGameState()
    {
        CurrentGameState = GameState.Game;
        OnGameState.Invoke();
    }

    public void Load()
    {
        PlayerSpawn = DataManager.instance.SavedPlayerSpawn;
        PlayerInventory = DataManager.instance.SavedPlayerInventory;
    }

    public void Save()
    {
        //TODO
        DataManager.instance.SavePlayerSpawn(PlayerSpawn);
        DataManager.instance.SavePlayerInventory(PlayerInventory);
        // save all of our unsaved items
        foreach (float item in UnSavedItemIDs)
        {
            DataManager.instance.SavedItemIDs.Add(item);
        }
        // clear our unsaved items list

    }

    // hold on to the temporary new player spawn
    public void SetPlayerSpawn(Vector3 playerPosition, Quaternion playerRotation)
    {
        PlayerSpawn.sceneID = SceneLoader.GetCurrentSceneIndex();
        PlayerSpawn.playerPosition = playerPosition;
        PlayerSpawn.playerRotation = playerRotation;
    }

    public void AddCollectible(float uID, CollectibleType collectibleType)
    {
        // add the uID, so we know not to spawn it in the world on a future run
        // add to inventory based on collectible type
        switch (collectibleType)
        {
            case CollectibleType.small:
                //TODO add the small collectible
                break;
            case CollectibleType.large:
                //TODO add the large collectible
                break;
            default:
                Debug.LogWarning("collectible type not valid");
                break;
        }
    }
}
