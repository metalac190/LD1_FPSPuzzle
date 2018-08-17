using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// This script will animate a platform to move between 2 positions, using movement functions. You may set a destination, as well as specify speed and pause amounts
/// </summary>
[RequireComponent(typeof(Rigidbody))]               //This object is moving, it needs a Rigidbody
[RequireComponent(typeof(AudioSource))]
//TODO add functionality for generic Rigidbody transforms as well
public class MovingPlatform : MonoBehaviour {

    [Header("Movement Settings")]
    [Tooltip("Destination transform to move towards")]
    [SerializeField] Transform destinationPoint;
    [Tooltip("Seconds to take to reach destination. Lower numbers means faster movement")]
    [SerializeField] float secondsForOneTrip = 5f;
    [Tooltip("Duration in seconds to pause after reaching destination")]
    [SerializeField] float pauseLength = 1f;

    [Header("General Settings")]
    [Tooltip("True will auto start movement on scene start. Disaable this if you'd like to control this on triggers, through code, etc.")]
    [SerializeField] bool playOnAwake = true;
    [Tooltip("Delay before starting movement, in seconds. Use this to stagger platform timing while keeping the same speed, or to create specific synchronizations")]
    [SerializeField] float startOffset = 0f;
    //[Tooltip("If true, player will attach to this moving platform when entering the trigger")]
    //[SerializeField] bool attachPlayer = true;
    //[Tooltip("If true, any rigidbody that enters (that aren't the player) will also follow platform")]
    //[SerializeField] bool attachOtherRigidbodies = true;

    [Header("SFX")]
    [Tooltip("Sound that plays when platform is initially turned on")]
    [SerializeField] AudioClip activateSound;   //TODO Sound that plays when the platform is activated (ie initially turns on)
    [Tooltip("Sound that plays while platform is in transit")]
    [SerializeField] AudioClip moveSound;
    [Tooltip("Sound that plays when platform begins a new trip")]
    [SerializeField] AudioClip startSound;      //TODO 
    [Tooltip("Sound that plays when platform completes a trip")]
    [SerializeField] AudioClip stopSound;       //TODO

	private bool platformActive = false;        //Activate the platform to start move cycle
    private bool platformMoving = false;        //Used to move platform between pauses
	private float delayTimer = 0f;
	private float platformTimer = 0f;
	private float nextPauseTime = 0f;
    private Vector3 startPoint;
    private Vector3 endPoint;

    // used for attaching/detaching from platforms
    private Vector3 previousPosition = Vector3.zero;    // initialize to 0, so calculations still work
    private Transform transformToAttach;            // this is our obejct that moves
    private Transform originalParent;       //Keep track of the original parent object, to return riders to
    private GameObject parentObject;        //Object that is being moved by the platform

    //AudioSource audioSource;

    //private List<Transform> attachedTransforms;
    private Transform attachedTransform;         // create our platform location object
    public Transform AttachedTransform
    {
        set
        {
            attachedTransform = value;
            // when we add a new Platform, initialize its startion position so that our calculation is correct
            previousPosition = attachedTransform.transform.position;
        }
        get { return attachedTransform; }
    }

    private void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    void Start(){

        startPoint = transform.position;        //Initialize destination transform 1

        //Error check to make sure we have a destination point. If we do fill it, if not make it equal to initial position
        if (destinationPoint != null)
        {
            endPoint = destinationPoint.position;       //Initialize destination transform 2

        }
        else
        {
            Debug.LogWarning("No destination point set on moving platform.");
            endPoint = transform.position;
        }

        nextPauseTime = secondsForOneTrip;		//Make sure that our first pause isn't until the end of 1 trip

		//If playOnAwake == true, Pause for the startOffset then activate
		if(playOnAwake){
			Invoke("ActivatePlatform", startOffset);
		}

        //Create an empty object for parenting objects to when riding the platform
        parentObject = new GameObject("PlatformParentNode");
        //Parent underneath this object
        parentObject.transform.SetParent(gameObject.transform);
        //Reset transformations, so that child objects don't get distorted transforms
        parentObject.transform.position = new Vector3(0, 0, 0);
        parentObject.transform.eulerAngles = new Vector3(0, 0, 0);
        parentObject.transform.localScale = new Vector3(1, 1, 1);
	}

	void Update(){

        //if our platform is Active, continue
        if (platformActive)
        {
            //If the platform is moving, make sure we're counting it to the timer
            if (platformMoving)
            {
                platformTimer += Time.deltaTime;        //Increment the platform timer, so that our loop calculates appropriately
            }
            //If it's not moving, make sure we're counting down the delay/pause timer
            if (!platformMoving)
            {
                UpdateDelayTimers();
            }
        }
	}

	void FixedUpdate(){
		//This controls platform movement
		if(platformActive){
			//Bounce the platform back and forth between the 2 points
			GetComponent<Rigidbody>().position = Vector3.Lerp(startPoint, endPoint, Mathf.SmoothStep(0f, 1f, Mathf.PingPong(platformTimer/secondsForOneTrip, 1f)));
			//Check to see if we've reached our next pause time, if so, pause the platform
			if(platformTimer > nextPauseTime){
				nextPauseTime += secondsForOneTrip;		//Pause after one trips time has passed
				PausePlatform(pauseLength);		//Delay the platform for the above specified amount (public variable)
			}
		}
	}

    private void LateUpdate()
    {
        // if we have an active platform, do the additional calculations
        if (attachedTransform != null)
        {
            // we are calculating how much the platform moved by subtracting last frame's position, then adding to moving object position
            transformToAttach.transform.position += AttachedTransform.position - previousPosition;
            // set previous position for our next frame
            previousPosition = AttachedTransform.position;
        }
    }

    //TODO get rid of script dependency
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            AttachTransform(other.gameObject.transform);
        }
        /*
        if(attachOtherRigidbodies == true && (other.GetComponent<Rigidbody>() != null))
        {
            AttachTransform(other.gameObject.transform);
        }
        */
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            RemoveTransform();
        }
    }

    /// <summary>
    /// Update timers that calculate the platform pause/delay between movements. If timer runs out, initiate movement cycle
    /// </summary>
    void UpdateDelayTimers()
    {
        //If the platform is not moving Count down the delay timer, so that the platform pauses before restarting its journey
        if (delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
        }
        //If the pause is finished, reset the the delay and prevent unnecessary calculations when below 0
        if (delayTimer <= 0)
        {
            delayTimer = 0;         //Reset delay timer to 0 to maintain clean code
            MovePlatform();     //Reactivate the platform
        }
    }

    /// <summary>
    /// Activate the platform. This activates the entire move/pause cycle, and is used to entirely shut off the platform cycle (inactive, no power, etc.)
    /// </summary>
	void ActivatePlatform(){
		//Change the platform
		platformActive = true;
        //Play activate audio
        
	}

    /// <summary>
    /// Move the platform between estinations. This tracks whether it's moving during it's move/pause cycle
    /// </summary>
    void MovePlatform()
    {
        //Move platform between destinations
        platformMoving = true;
    }

    /// <summary>
    /// Temporarily pause the platform at the destination, before continuing the movement cycle
    /// </summary>
    /// <param name="secondsPaused"></param>
	void PausePlatform(float secondsPaused){
		//Delay the platform
		delayTimer = secondsPaused;
		platformMoving = false;
	}

    void AttachTransform(Transform transformToAttach)
    {
        //Add a transform, return the parentObject's start position
        AttachedTransform = parentObject.transform;
        // assign the gameObject's movement to calculate additional movements from platform
        this.transformToAttach = transformToAttach;
    }

    //Delete our platform
    public void RemoveTransform()
    {
        // set our previous position to zero, just in case
        previousPosition = Vector3.zero;
        // clear our platform
        attachedTransform = null;
        // clear our movig object
        transformToAttach = null;
    }
}
