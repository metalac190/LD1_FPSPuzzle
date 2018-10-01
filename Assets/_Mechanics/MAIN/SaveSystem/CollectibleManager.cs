using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CollectibleManager : MonoBehaviour {

    public event Action OnCollect = delegate { };

    // values associated with collectibles
    [SerializeField] CollectibleInventory unsavedCollectibleInventory = new CollectibleInventory();
    public CollectibleInventory UnsavedCollectibleInventory { get; private set; }
    [SerializeField] List<float> unsavedCollectedIDs = new List<float>();
    public List<float> UnsavedCollectedIDs { get { return unsavedCollectedIDs; } }

    Collectible[] collectiblesInScene;
    GameManager gameManager;

    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
        // initialize
        SubscribeToEvents();
    }

    private void Start()
    {
        // local state
        DestroyPreviouslyCollectedItems();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    void SubscribeToEvents()
    {
        gameManager.OnSave += HandleSave;
    }

    void UnsubscribeToEvents()
    {
        gameManager.OnSave -= HandleSave;
    }

    public void DestroyPreviouslyCollectedItems()
    {
        collectiblesInScene = FindObjectsOfType<Collectible>();
        List<float> previouslyCollectedLevelUIDs = DataManager.Instance.GetCurrentLevelUIDList().savedCollectedUIDs;
        // cross reference all of our collectibles with our already saved uIDs
        foreach (Collectible collectible in collectiblesInScene)
        {
            foreach(float previouslyCollectedUID in previouslyCollectedLevelUIDs)
            {
                // we've found a UID we've already collected
                if(collectible.UniqueIdentifier == previouslyCollectedUID)
                {
                    collectible.gameObject.SetActive(false);
                }
            }
        }
    }

    public void HandleSave()
    {
        DataManager.Instance.SaveCollectibleInventory(unsavedCollectibleInventory);
        DataManager.Instance.SaveCollectibleUIDs(unsavedCollectedIDs);
        // clear out our unsaved list, since we moved them all to our saved list.
        ClearUnsavedInventory();
    }

    void ClearUnsavedInventory()
    {
        unsavedCollectibleInventory = new CollectibleInventory();
        unsavedCollectedIDs.Clear();
    }

    public void AddCollectible(float uID, CollectibleType collectibleType)
    {
        AddCollectibleToUnsavedList(uID,collectibleType);
        AddUIDToUnsavedList(uID);
        // update UI to account for collectibles
        OnCollect.Invoke();
    }

    void AddCollectibleToUnsavedList(float uID, CollectibleType collectibleType)
    {
        // add the uID, so we know not to spawn it in the world on a future run
        // add to inventory based on collectible type
        switch (collectibleType)
        {
            case CollectibleType.small:
                unsavedCollectibleInventory.smallCollectibles ++;
                break;
            case CollectibleType.large:
                //TODO add the large collectible
                unsavedCollectibleInventory.largeCollectibles ++;
                break;
            case CollectibleType.key:
                unsavedCollectibleInventory.keys++;
                break;
            default:
                Debug.LogWarning("collectible type not valid");
                break;
        }
    }

    void AddUIDToUnsavedList(float uID)
    {
        unsavedCollectedIDs.Add(uID);
    }
}
