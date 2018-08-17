using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelSpawner : MonoBehaviour {

    private List<LevelCollectibleData> savedLevelCollectibles = new List<LevelCollectibleData>();
    public List<float> UnSavedItemIDs { get; private set; }
    public List<float> SavedItemIDs { get; private set; }

    public void Initialize()
    {
        GameManager.instance.OnSave += HandleSave;
        GameManager.instance.OnLevelLoad += HandleLevelLoad;
    }

    private void OnDisable()
    {
        GameManager.instance.OnSave -= HandleSave;
        GameManager.instance.OnLevelLoad -= HandleLevelLoad;
    }

    void HandleSave()
    {
        // save all of our unsaved items
        foreach(float item in UnSavedItemIDs)
        {
            SavedItemIDs.Add(item);
        }
    }

    void HandleLevelLoad()
    {
        DestroyPreviouslyCollectedItems();
    }

    public void DestroyPreviouslyCollectedItems()
    {

    }
}
