using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {

    [SerializeField] InventoryData playerInventory = new InventoryData();

    private void OnEnable()
    {
        LoadInventory();
        GameManager.instance.OnSave += HandleSave;
    }

    private void OnDisable()
    {
        GameManager.instance.OnSave -= HandleSave;
    }

    void LoadInventory()
    {
        playerInventory = GameManager.instance.SavedInventory;
    }

    void HandleSave()
    {
        GameManager.instance.SaveInventory(playerInventory);
    }
}
