using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is large so that a large game mechanic can be contained as much as possible on a single script, without too many dependencies.
/// This script allows the player to move object rigidbodies (that are flagged) forward and backward while the corresponding keys are held.
/// We also create Checkspheres while pushing/pulling objects to make sure we cannot pull colliders into the player.
/// by: Adam Chandler
/// </summary>

[RequireComponent(typeof(AudioSource))]

public class PushPull : MonoBehaviour {

    [Header("Push/Pull")]
    [Tooltip("Force amount we are applying to the object. Objects with higher mass require more force to move.")]
    public float forceAmount = 30f;          //Intensity of Push
    [Tooltip("Distance we can activate Push/Pull on an object")]
    public float pushPullRange = 10f;       //How far can we push/pull objects
    [Tooltip("Independent of force, what is the max speed we want objects to move while being push/pulled")]
    public float maxTranslateSpeed = 2.5f;  //Max speed objects will be pushed/pulled
    [Tooltip("Independent of torque, what is the max speed we want objects to rotate while being push/pulled")]
    public float maxRotateSpeed = 1f;       //max speed objects will be rotated
    [Tooltip("How quickly objects will stop moving once released. We are forcing an animated stop to prevent launching from very high forces")]
    public float brakeSpeed = .85f;          //How fast will objects slow when no longer pushed

    [Header("General Settings")]
    [Tooltip("How often to shoot interact raycast checks. Lower values check more often, higher values check less often but are more optimized")]
    public float interactFrequency = .2f;     //How often we will shoot the interact raycast, for optimization
    [Tooltip("How often to shoot raycasts to detect if we're still hovering over object while moving. Lower values check more often, higher values check less often but are more optimized")]
    public float hoverFrequency = .2f;          //How often we will shoot the hover raycast, for optimization
    [Tooltip("How large is the sphere used to detect if the player is too close to the object (and prematurely releasing). Larger numbers create larger sphere collider checks, meaning it will break from further away from the player ")]
    public float detectBreakRadius = 1f;         //How far away from player the object will break when touching

    [Header("Layer Masks")]
    [Tooltip("Which layers can be push/pulled")]
    public LayerMask pushPullLayers;        //Objects on these layers will be detected by Raycast
    [Tooltip("Layers used to check for premature breaks while push/pulling. A sphere will be drawn at the raycast point, if anything on these layers tough this sphere it will break the push/pull")]
    public LayerMask breakLayersDetected;   //What layers to detect when checking for an intersect break

    [Header("Particle FX")]
    [Tooltip("Particles to activate if the current object in front of the player can be push/pulled")]
    public GameObject interactParticle;     //Visual to indicate if object is interactive
    [Tooltip("Particles to activate while in pushing state")]
    public GameObject pushParticle;         //Visual to activate while pushing
    [Tooltip("Particles to activate while in pulling state")]
    public GameObject pullParticle;         //Visual to activate while pulling

    [Header("SFX")]
    [Tooltip("Sound to play when push/pull is activated")]
    public AudioClip sfx_activate;              //Sound effect for push/pulling
    [Tooltip("Sound to play when push/pull is released")]
    public AudioClip sfx_release;              //Sound effect for releasing the push/pull

    enum MoveState { Releasing, Pushing, Pulling, Idle };   //States of push/pull objects
    MoveState currentState;

    bool interactReady;                      //Are we ready to accept another interact raycast
    bool hoverReady;                        //Are we ready to check hover over interactive object
    bool isInRange;                         //Are we in range to push/pull object
    float interactTimer;                     //We use this to make sure we dont shoot raycasts every frame
    float hoverTimer;                       //Make sure we dont shoot raycasts every frame

    Camera playerCam;                       //Player's camera, used for determing raycasting directions
    AudioSource aSource;                    //AudioSource for playing pushPull sounds
    RaycastHit hit;                         //Reference to the hit information of the object detected by raycast
    Rigidbody rb;                           //Reference to the Rigidbody on the object that was hit
    Vector3 rayPos;                         //Start point of ray
    Vector3 pushDirection;                  //Direction we are applying the force
    Material objectMat;                     //Store the object's material, so that we can change it and revert

    void Awake()
    {
        //Grab our references
        playerCam = Camera.main;
        aSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //Set default state
        currentState = MoveState.Idle;

        //Ready to receive a raycast by default
        interactReady = true;
        hoverReady = true;

        //Make sure our visuals are disabled
        if(interactParticle != null)
        {
            interactParticle.SetActive(false);
        }
        if(pushParticle != null)
        {
            pushParticle.SetActive(false);
        }
        if(pullParticle != null)
        {
            pullParticle.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
        //Check and update our raycast timer, to see if we're ready to shoot
        UpdateInteractTimer();
        UpdateHoverTimer();

        //if we're currently not push/pulling and we're ready to raycast, detect interactive object
        if (hoverReady && currentState == MoveState.Idle)
        {
            if(CheckHover() == true)
            {
                ActivateCursor();
            }
            else
            {
                RevertCursor();
            }
        }

        //Detect initial input for push, if we're ready to raycast. Collect the object's info
        if (Input.GetKeyDown(KeyCode.Mouse0) && interactReady)
        {
            //Detect if we're raycasting on a pushpull object, if so then pull
            if (CheckInteract() == true)
            {
                CollectObjectInfo();

                //Test to see if the object has a rigidbody before we start pushing
                if(rb != null)
                {
                    PushStart();
                } 
            }
        }
        //Detect input for Push while button is held, if we're ready to raycast
        if (Input.GetKey(KeyCode.Mouse0) && currentState == MoveState.Pushing && interactReady)
        {
            //If our mouse moves off the object while the key is held, release early
            if (CheckInteract() == false && rb != null)
            {
                ReleaseStart();
            }
        }
        //Detect initial input for pull, if we're ready to raycast. Collect the object's info
        if (Input.GetKeyDown(KeyCode.Mouse1) && interactReady)
        {
            //Detect if we're raycasting on a pushpull object, if so then pull. Also check to make sure we're not overlapping with player
            if (CheckInteract() == true && !CheckOverlap())
            {
                CollectObjectInfo();
                //Test to see if the object has a rigidbody before we start pulling
                if(rb != null)
                {
                    PullStart();
                }
            }
        }
        //Detect continuous input for Pull while button is held, if we're ready to raycast
        if (Input.GetKey(KeyCode.Mouse1) && currentState == MoveState.Pulling && interactReady)
        {
            //If our mouse moves off the object while the key is held, release early
            if ((!CheckInteract() && rb != null) || CheckOverlap())
            {
                ReleaseStart();
            }
        }
        //Stop velocity if push or pull button is released and we're not already releasing
        if((Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1)) && currentState != MoveState.Releasing)
        {
            if(rb != null)
            {
                ReleaseStart();
            }
        }
	}

    void FixedUpdate()
    {
        //If we're pushing an object, push it
        if (currentState == MoveState.Pushing)
        {
            //Calculate the direction of force
            rayPos = playerCam.transform.position;
            //pushDirection = rayPos - hit.point;
            pushDirection = playerCam.transform.forward;
            //Move the Object
            MoveObject(pushDirection);
            //Check to make sure we're not exceeding max speed
            LimitMaxSpeed();
        }
        //If we're pulling an object, pull it
        if (currentState == MoveState.Pulling)
        {
            //Calculate the direction of force
            rayPos = playerCam.transform.position;
            //pushDirection = hit.point - rayPos;
            pushDirection = -playerCam.transform.forward;
            //Move the Object
            MoveObject(pushDirection);
            //Check to make sure we're not exceeding max speed
            LimitMaxSpeed();
        }
        //If we're not moving an object, slow it down
        if (currentState == MoveState.Releasing)
        {
            //Otherwise, gradually slow speed until we stop
            rb.velocity = rb.velocity * brakeSpeed;
            rb.angularVelocity = rb.angularVelocity * brakeSpeed;

            //If we're close enough to stopping, then stop
            if(rb.velocity.magnitude <= .1f && rb.angularVelocity.magnitude <= .1f)
            {
                StopObject();
            }
        }
    }

    /// <summary>
    /// Counts down the timers to determine if we're ready to shoot another Interact raycast. We do this so that we don't have to do this every Update tick, which is very inefficient
    /// </summary>
    void UpdateInteractTimer()
    {
        //We're still counting down, not yet ready
        if(interactTimer > 0)
        {
            //still on cooldown, count down the timer
            interactTimer -= Time.deltaTime;
            //Check to see if we just now hit 0
            if(interactTimer == 0)
            {
                interactReady = true;
            }
        }
        //if our timer has fallen below 0, deactivate and set to 0
        if(interactTimer < 0)
        {
            interactTimer = 0;
            interactReady = true;
        }
    }

    /// <summary>
    /// Counts down the timers to determine if we're ready to shoot another Hover raycast. We do this so that we don't have to do this every Update tick, which is very inefficient
    /// </summary>
    void UpdateHoverTimer()
    {
        //We're still counting down, not yet ready
        if (hoverTimer > 0)
        {
            //still on cooldown, count down the timer
            hoverTimer -= Time.deltaTime;
            //Check to see if we just now hit 0
            if (hoverTimer == 0)
            {
                hoverReady = true;
            }
        }
        //if our timer has fallen below 0, deactivate and set to 0
        if (hoverTimer < 0)
        {
            hoverTimer = 0;
            hoverReady = true;
        }
    }

    /// <summary>
    /// Check to see if our Raycast hit an obejct in the Push/Pull layer so that we know whether or not we can push/pull it
    /// </summary>
    /// <returns></returns>
    bool CheckInteract()
    {
        //We're shooting a ray, go on cooldown according to our specified frequency
        interactTimer = interactFrequency;
        interactReady = false;
        //Shoot a ray from camera position, in camera direction, in the distance we specified, looking for pushpull objects
        if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, pushPullRange, pushPullLayers))
        {
            return true;
        } else
        {
            return false;
        }
    }

    /// <summary>
    /// Check to see if our Raycast is still true while we're push/pulling, so taht we can break early if the player looks away
    /// </summary>
    /// <returns></returns>
    bool CheckHover()
    {
        //We're shooting a ray, go on cooldown according to our specified frequency
        hoverTimer = hoverFrequency;
        hoverReady = false;
        //Shoot a ray from camera position, in camera direction, in the distance we specified, looking for pushpull objects
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, pushPullRange, pushPullLayers))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Checks to see if the player is too close while pulling by drawing a sphere that will detect player collisions. We do this because we do not want to forcefully pull a collider into the player
    /// </summary>
    /// <returns></returns>
    bool CheckOverlap()
    {
        //Draw a sphere around impact point to detect if we're too close to the player
        if (Physics.CheckSphere(hit.point, detectBreakRadius, breakLayersDetected))
        {
            //Debug.Log("OVERLAP detected");
            return true;
        }
        else
        {
            //Debug.Log("NO OVERLAP");
            return false;
        }
    }

    /// <summary>
    /// Forcefully limit our max speed of objects being push/pulled, to retain control and avoid physics shenanigans
    /// </summary>
    void LimitMaxSpeed()
    {
        //Check to make sure we haven't exceeded our max speed
        if (rb.velocity.magnitude > maxTranslateSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxTranslateSpeed;
        }
        if(rb.angularVelocity.magnitude > maxRotateSpeed)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxRotateSpeed;
        }
    }

    /// <summary>
    /// Grab the new object's info (like Rigidbody component)
    /// </summary>
    void CollectObjectInfo()
    {
        //Grab the object we hit's rigidbody, so that we can apply forces
        rb = hit.collider.gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Start the pull state and play the SFX and particles
    /// </summary>
    void PullStart()
    {
        //This obejct is now pulling
        currentState = MoveState.Pulling;
        //No longer need hover over particle, or raycast checks for hover objects
        hoverReady = false;
        RevertCursor();

        //Connect and Play Audio, if Audio exists
        if (sfx_activate != null)
        {
            aSource.clip = sfx_activate;
            aSource.Play();
        }
        //Add visuals
        if (pullParticle != null)
        {
            ActivateParticle();
        }
    }

    /// <summary>
    /// Start the push state and play the SFX and particles
    /// </summary>
    void PushStart()
    {
        //This object is now pushing
        currentState = MoveState.Pushing;
        //No longer need hover over particle, or raycast checks for hover objects
        hoverReady = false;
        RevertCursor();

        //Connect and Play Audio, if Audio exists
        if (sfx_activate != null)
        {
            aSource.clip = sfx_activate;
            aSource.Play();
        }
        //Add visuals
        if(pushParticle != null)
        {
            ActivateParticle();
        }
    }

    /// <summary>
    /// Start the Release state, playing SFX and disabling the particles
    /// </summary>
    void ReleaseStart()
    {
        //We are no longer moving anything, change state
        currentState = MoveState.Releasing;
        //Connect and Play Audio, if Audio exists
        if (sfx_release != null)
        {
            aSource.clip = sfx_release;
            aSource.Play();
        }
        //Disable particle
        if(pushParticle != null && pullParticle != null)
        {
            DisableParticle();
        }
    }

    /// <summary>
    /// Apply force and torque to the object. Don't allow vertical angles and keep forces horizontal, to keep things more controlled
    /// </summary>
    /// <param name="forceDirection"></param>
    void MoveObject(Vector3 forceDirection)
    {
        //don't push the force at vertical angles, it makes for inconsistent results. Ignore Y force
        Vector3 equalizedForce = new Vector3(forceDirection.x, 0, forceDirection.z);
        rb.AddForce(equalizedForce * forceAmount);
        //Add normal force to rotations
        rb.AddTorque(forceDirection * forceAmount);
    }

    /// <summary>
    /// Kill object velocity and set to Idle state. We are now ready to move another object
    /// </summary>
    void StopObject()
    {
        //Zero out our velocity
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        //Our object is now idle once again
        currentState = MoveState.Idle;
        //Ready to receive hover events again
        hoverReady = true;
    }

    /// <summary>
    /// Turn on the apprioriate Push/Pull particles by making them active and moving them to the hit point. Parent the particles to the Rigidbody object to get better tracking
    /// </summary>
    void ActivateParticle()
    {
        if (currentState == MoveState.Pulling)
        {
            //Turn on GameObject
            pullParticle.SetActive(true);
            //Move to hit location
            pullParticle.transform.position = hit.point;
            //Parent to pull object
            pullParticle.transform.parent = rb.transform;

        }

        if(currentState == MoveState.Pushing)
        {
            //Turn on GameObject
            pushParticle.SetActive(true);
            //Move to hit location
            pushParticle.transform.position = hit.point;
            //Parent to pull object
            pushParticle.transform.parent = rb.transform;
        }
    }

    /// <summary>
    /// Deactivate the push/pull particles
    /// </summary>
    void DisableParticle()
    {
        //Disable GameObject
        pullParticle.SetActive(false);
        pushParticle.SetActive(false);
    }

    /// <summary>
    /// Turn on the hover/cursor particles
    /// </summary>
    void ActivateCursor()
    {
        if(interactParticle != null)
        {
            interactParticle.SetActive(true);
        }  
    }

    /// <summary>
    /// Disable the hover/cursor particles
    /// </summary>
    void RevertCursor()
    {
        if(interactParticle != null)
        {
            interactParticle.SetActive(false);
        }
    }

    /// <summary>
    /// Draw the checksphere while push/pulling in the editor, so that we can trouble shoot collisions and players colliding with push/pull objects
    /// </summary>
    private void OnDrawGizmos()
    {
        //If we're moving an object, show us the break threshold
        if(currentState == MoveState.Pulling || currentState == MoveState.Pushing)
        {
            Gizmos.DrawSphere(hit.point, detectBreakRadius);
        }
    }
}
