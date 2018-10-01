using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct PlayerSpawnData
{
    public int sceneID;     // what scene index we are in
    public bool usePlayerStart;     // whether or not to spawn from the start point
    public Vector3 playerPosition;
    public Quaternion playerRotation;
}

[Serializable]
public struct CollectibleInventory
{
    public int smallCollectibles;
    public int largeCollectibles;
    public int keys;
}

[Serializable]
public struct LevelSavedCollectibleUIDs
{
    public int sceneID; // which scene these collectibles belong to
    public List<float> savedCollectedUIDs;  // list of our stored collectibles
    // constructor
    public LevelSavedCollectibleUIDs(int newSceneID)
    {
        this.sceneID = newSceneID;
        this.savedCollectedUIDs = new List<float>();
    }
}


