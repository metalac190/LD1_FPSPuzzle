using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Collectible : MonoBehaviour {

    [SerializeField] AudioClip collectSound;

    public float UniqueIdentifier { get; private set; }       // unique identifier
    [SerializeField] CollectibleType collectibleType = CollectibleType.small;
    public CollectibleType CollectibleType { get { return collectibleType; } }

    CollectibleManager collectibleManager;

    private void Awake()
    {
        collectibleManager = FindObjectOfType<CollectibleManager>();
        // do a crazy calculation to get a seemingly unique identifying value for this object
        CalculateUniqueIdentifier();
    }

    private void CalculateUniqueIdentifier()
    {
        UniqueIdentifier = transform.position.sqrMagnitude;
    }

    public virtual void Collect()
    {
        // add to inventory here
        if (collectSound != null)
            PlayCollectSound();

        AddToInventory();
        DisableCollectibleInWorld();
    }

    private void PlayCollectSound()
    {

    }

    void AddToInventory()
    {
        collectibleManager.AddCollectible(UniqueIdentifier, collectibleType);
    }

    void DisableCollectibleInWorld()
    {
        gameObject.SetActive(false);
    }
}
