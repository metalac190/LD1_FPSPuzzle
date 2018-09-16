using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    public float UniqueIdentifier { get; private set; }       // unique identifier
    [SerializeField] CollectibleType collectibleType = CollectibleType.small;
    public CollectibleType CollectibleType { get { return collectibleType; } }

    private void Awake()
    {
        // do a crazy calculation to get a seemingly unique identifying value for this object
        CalculateUniqueIdentifier();
    }

    private void CalculateUniqueIdentifier()
    {
        UniqueIdentifier = transform.position.sqrMagnitude;
    }

    public virtual void Collect()
    {
        // add to local player inventory, until stored at a checkpoint
        CollectibleData collectible = new CollectibleData();
        collectible.uID = UniqueIdentifier;
    }
}
