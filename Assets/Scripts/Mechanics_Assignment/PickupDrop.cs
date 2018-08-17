using UnityEngine;
using System.Collections;

/// <summary>
/// This script allows the Player to pick up a Rigidbody Collider that is in front of the player and marked on the PickupDropLayer.
/// The object picked up must be tall enough to receive the raycast from the player (which goes directly out in front, it is
/// not based from camera (this is so that it will also work in 3rd person camera mode).
/// by: Adam Chandler
/// </summary>

[DisallowMultipleComponent]

public class PickupDrop : MonoBehaviour
{
    [Header("Pickup Drop")]
    [Tooltip("How far away we can pick up a Pickup/Drop object")]
    public float pickupDistance = 4.0f;     //distance the object is grab-able 
    [Tooltip("Position of the object while being held. NOTE: If your objects is too big and your position is too close, it will collider with the player and you will be unable to drop")]
    public Transform pickupLocation;        //Where to position the block while carrying  

    [Header("General Settings")]
    [Tooltip("Size of the detection sphere use to test collisions before dropping")]
    public float detectDropRadius = .75f;           //Detection sphere to check when dropping     
    [Tooltip("Offsets the raycast vertically straight in front of the character. 0 is center of character, negative is below neutral. Smaller items need a lower offset.")]
    public float vertOffset = -.5f;              //Vertical Offset from character center for detecting in front of player (so it catches tiny objects)
    [Tooltip("Key Input to activate the pickupDrop action")]
    public KeyCode pickupDropKey = KeyCode.E;           //Key to press to activate pickup/drop when in range

    [Header("Layer Masks")]
    [Tooltip("All Colliders assigned to this layer will be able to be pickup/dropped")]
    public LayerMask pickupMask;            //layer interactible objects must be assigned to  
    [Tooltip("These layers will be checked when determining if there is collision when we are trying to drop the block")]
    public LayerMask dropLayersDetected;        //What layers to detect when dropping

    private bool isHoldingSomething = false;	//If we are holding something
	private GameObject heldObject;	//The object we are holding

	RaycastHit hitInfo;		//Raycast Hit info
	GameObject mainCamera;	//Reference to FPS Cam
	Rigidbody rb;           //Rigidbody on the object we are carrying
	Collider col;           //Collider on the object we are carrying

	void Awake(){
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}

	// Update is called once per frame
	void Update ()
	{
		//Check to see if we're NOT holding something before testing a new pickup
		if(!isHoldingSomething)
		{
			//If we press the button, pick up the object
			if(Input.GetKeyDown(pickupDropKey))
			{
				PickupObject ();	//Pick up the Object
			}
		}
		//else we ARE holding something
		else 
		{
			//If holding something, update the location of the holdable object
			heldObject.transform.position = pickupLocation.position;
			//If we press the button, drop the object
			if(Input.GetKeyDown(KeyCode.E)){
				DropObject ();		//Drop the Object
			}
		}
	}

    /// <summary>
    /// Grab the object and store it at the pickup/hold position. Store collider and rigidbody info
    /// </summary>
	void PickupObject ()
	{
        //We are not holding an object
        //Calculate start position of ray so that it's closer to player's feet to catch small objects
        Vector3 positionWithOffset = new Vector3(transform.position.x, transform.position.y + vertOffset, transform.position.z);
        //Debug.DrawRay(positionWithOffset, transform.forward * pickupDistance, Color.blue, 1f);
		if (Physics.Raycast (positionWithOffset, transform.forward, out hitInfo, pickupDistance, pickupMask)) {
            //We are now holding this object
            isHoldingSomething = true;
			//Fill our 'object being held' slot
			heldObject = hitInfo.collider.gameObject;
			//Fill references for new object
			rb = hitInfo.collider.GetComponent<Rigidbody>();
			col = hitInfo.collider.GetComponent<Collider>();
			//Held object should be kinematic because we're moving it
			rb.isKinematic = true;
			//Turn into a Collider so that it will once again collide
			col.isTrigger = true;
			//Move the object to our new floating position
			hitInfo.collider.transform.position = pickupLocation.position;
		}
	}

    /// <summary>
    /// Create a collision sphere to detect whether we can drop the object safely. If we can, empty our object info and release it
    /// </summary>
	void DropObject ()
	{
		//Check to make sure we're not dropping the object into a collision
		if(Physics.CheckSphere(heldObject.transform.position, detectDropRadius, dropLayersDetected) == true){
			Debug.Log("Cannot place object because of collisions");
		}

		if(Physics.CheckSphere(heldObject.transform.position, detectDropRadius, dropLayersDetected) == false){
			//Held object is no longer kinematic
			rb.isKinematic = false;
			//Turn into a Collider so that it will once again collide
			col.isTrigger = false;
			//Empty our 'object being held' slot
			heldObject = null;
			//We are no longer holding this object
			isHoldingSomething = false;
		}
	}

	void OnDrawGizmos(){
		//If we're holding an object, show us drop collision
		if(heldObject != null){
			Gizmos.DrawSphere(heldObject.transform.position, detectDropRadius);
		}
	}
}
