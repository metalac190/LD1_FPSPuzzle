using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerSpawnData {

    public int sceneID;     // what scene index we are in

    public Vector3 playerPosition;
    public Quaternion playerRotation;
}

[Serializable]
public class InventoryData
{
    public int smallCollectibles;
    public int largeCollectibles;
    public int keys;
}

[Serializable]
public class CollectibleData
{
    public float uID;       // unique ID associated with this Placed Collectible
}

[Serializable]
public class LevelCollectibleData
{
    public int sceneID; // which scene these collectibles belong to
    public List<CollectibleData> storedCollectibles;  // list of our stored collectibles

    // constructor for collectible list
    public LevelCollectibleData (int newSceneID)
    {
        this.sceneID = newSceneID;
        this.storedCollectibles = new List<CollectibleData>();
    }
}

