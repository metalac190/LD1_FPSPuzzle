using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelSpawner : MonoBehaviour {

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
