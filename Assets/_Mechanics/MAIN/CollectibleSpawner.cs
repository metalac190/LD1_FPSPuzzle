using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectibleSpawner : MonoBehaviour {

    GameManager gameManager;
    Collectible[] collectiblesInScene;

    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
        // local setup
        collectiblesInScene = FindObjectsOfType<Collectible>();
    }

    public void DestroyPreviouslyCollectedItems()
    {

    }
}
