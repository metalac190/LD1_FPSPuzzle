using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerSpawnData
{
    public int sceneID = 0;     // what scene index we are in
    public bool usePlayerStart = true;     // whether or not to spawn from the start point
    public Vector3 playerPosition = new Vector3(0,0,0);
    public Quaternion playerRotation;
}

[Serializable]
public class InventoryData
{
    public int smallCollectibles = 0;
    public int largeCollectibles = 0;
}

[Serializable]
public class CollectibleData
{
    public float uID = 0;       // unique ID associated with this Placed Collectible
}

[Serializable]
public class LevelCollectibleData
{
    public int sceneID; // which scene these collectibles belong to
    public List<CollectibleData> storedCollectibles;  // list of our stored collectibles

    // constructor for collectible list
    public LevelCollectibleData(int newSceneID)
    {
        this.sceneID = newSceneID;
        this.storedCollectibles = new List<CollectibleData>();
    }
}


