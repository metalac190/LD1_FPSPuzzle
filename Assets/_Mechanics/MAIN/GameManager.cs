using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSpawner))]
[RequireComponent(typeof(LevelSpawner))]
public class GameManager : MonoBehaviour {

    public static Player Player { get; private set; }

    public PlayerSpawnData PlayerSpawn { get; private set; }
    public InventoryData Inventory { get; private set; }

    public List<float> UnSavedItemIDs { get; private set; }

    public void Initialize()
    {
        // Initialize relevant scripts
        GetComponent<PlayerSpawner>().Initialize(this);
        GetComponent<LevelSpawner>().Initialize(this);
    }
    
    public void SetPlayer(Player newPlayer)
    {
        Player = newPlayer;
    }

    void HandleSave()
    {
        // save all of our unsaved items
        foreach (float item in UnSavedItemIDs)
        {
            DataManager.instance.SavedItemIDs.Add(item);
        }
    }
}
