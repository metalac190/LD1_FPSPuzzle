using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectibleSpawner : MonoBehaviour {

    GameManager gameManager;

    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
    }

    public void DestroyPreviouslyCollectedItems()
    {

    }
}
