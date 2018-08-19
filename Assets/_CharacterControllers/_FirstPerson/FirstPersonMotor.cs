using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using DataAssets;

[RequireComponent(typeof (CharacterController))]
[RequireComponent(typeof (AudioSource))]
public class FirstPersonMotor : MonoBehaviour, IPlayerMotor
{
    #region Variables and Classes
    [Header("Movement")]
    //[SerializeField] FloatAsset moveSpeed;          
    [SerializeField] float moveSpeed = 4;   // movement speed while walking
    public float MoveSpeed { get { return moveSpeed; } }
    [SerializeField] float runSpeedMultiplier = 2;                  // multiply movement speed while running
    public float RunSpeedMultiplier { get { return runSpeedMultiplier; } }
    [SerializeField] float playerMinimumMoveSpeed = 1;      // lowest we value can adjust player move speed
    public float PlayerMinimumMoveSpeed { get { return playerMinimumMoveSpeed; } }
    [SerializeField] float mass = 3.0f;                             // character's mass. This can be adjusted to reduce pushback from faked forces
    [SerializeField] [Range(0f, 1f)] float runstepLengthen = .5f;   //While running: 0 is longer strides, 1 is short steps

    [Header("Jump")]
    [SerializeField] float jumpSpeed = 10;              // upward momentum from jump
    public float JumpSpeed { get { return jumpSpeed; } }
    [SerializeField] float stickToGroundForce = 1;      // threshold force that we need to surpass in order to launch player off the ground
    [SerializeField] float gravityMultiplier = 2;       // faked gravity amount, pulling downwards
    public float GravityMultiplier { get { return gravityMultiplier; } }

    [Header("Camera")]
    [SerializeField] Camera playerCamera = null;                     // camera used
    [SerializeField] MouseLook mouseLook;               // mouse look class that contains camera movement settings
    [SerializeField] bool useFOVKick = true;            // do we want a FOV change while running?
    [SerializeField] FOVKick fovKick = new FOVKick();   // class that contains FOV change settings

    [Header("Bob")]
    [SerializeField] bool useHeadBob = true;                                    // do we want to use headbob?
    [SerializeField] CurveControlledBob m_HeadBob = new CurveControlledBob();   // control curve for headbob amount
    [SerializeField] LerpControlledBob m_JumpBob = new LerpControlledBob();     // control curve for jump landing headbob
    [SerializeField] float m_StepInterval = 5;                                  // distance of steps

    [Header("SFX")]
    [SerializeField] AudioClip[] footstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] AudioClip jumpSound;           // the sound played when character leaves the ground.
    [SerializeField] AudioClip landSound;           // the sound played when character touches back on ground.

    // states
    private bool isWalking;                 // true while in walking state
    private bool isJumping;                 //true while in jumping state
    private bool jumpInput;                 //true while jump input is active
    private bool runInput;                  //true while running input is active
    private float yRotation;                // keep track of yRotation so we can clamp it (to avoid camera going upside down)
    private Vector2 moveInput;                      // x,y axis input for rotating camera
    private Vector2 rotateInput;                    // x,z axis input for moving the character
    private Vector3 moveVector = Vector3.zero;      // movement vector calculated each update that gets passed into Character Controller
    private CollisionFlags collisionFlags;          // collision zone we're using from Character Controller
    private float stepCycle;                        // current move distance    
    private float nextStep;                         // move distance needed to achieve next step
    private Vector3 impactForce = Vector3.zero;     // outside force pushing the player

    // book keeping
    private bool previouslyGrounded;
    private Vector3 originalCameraPosition;

    // caching
    AudioSource audioSource;                        
    CharacterController characterController;
    Player player;
    #endregion

    #region Setup
    void Awake()
    {
        // get references
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        // check to make sure Camera is assigned
        if(playerCamera == null)
        {
            Debug.LogWarning("No Camera assigned on FPS Controller");
            playerCamera = Camera.main;
        }
        // find our player character
        player = GetComponent<Player>();
    }

    // Use this for initialization
    private void Start()
    {
        // set default camera position
        originalCameraPosition = playerCamera.transform.localPosition;
        // synchronize camera
        fovKick.Setup(playerCamera);
        m_HeadBob.Setup(playerCamera, m_StepInterval);
        // start steup cycle
        stepCycle = 0f;
        nextStep = stepCycle / 2f;
        // not jumping by default
        isJumping = false;
        // initialize mouseLook script
        mouseLook.Init(transform, playerCamera.transform);
        // make sure stick to ground force is not below .5, otherwise ground hopping will occur
        if (stickToGroundForce < 0.5f)
            stickToGroundForce = 0.5f;
    }
    #endregion

    #region Game Loop
    // Update is called once per frame
    private void Update()
    {
        // rotate our camera
        RotateView();
        // we have just hit solid ground, ending our jump
        HitGround();
        // we are still in the air
        if (!characterController.isGrounded && !isJumping && previouslyGrounded)
        {
            moveVector.y = 0f;
        }
        // reset our previously grounded bool to the current state of grounding
        previouslyGrounded = characterController.isGrounded;
    }

    private void HitGround()
    {
        if (!previouslyGrounded && characterController.isGrounded)
        {
            // landing jump bob activate!
            StartCoroutine(m_JumpBob.DoBobCycle());
            // play landing audio
            PlayLandingSound();
            // no longer need downward gravity
            moveVector.y = 0f;
            // no longer jumping
            isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        // create temporary speed variable to store this frame's speed info into
        float speed;
        // calculate our desired speed to store into our temp variable
        CalculateMoveSpeed(out speed);
        // determine and apply FOV Kick when starting a run
        DetermineFOVKick();
        // convert forward movement to account for slopes
        NormalizeSlopeAngles(speed);

        // if we're grounded, pull towards ground
        if (characterController.isGrounded)
        {
            // set the y force to -stick to Ground Force. Only force amounts > this stickToGround amount can get above 0
            this.moveVector.y = -stickToGroundForce;
            // we're grounded and pressed the jump button. Jump!
            if (jumpInput)
            {
                //add our jump force
                this.moveVector.y = jumpSpeed;
                // play jump sound
                PlayJumpSound();
                // turn off jump input press
                jumpInput = false;
                // now in jumping state
                isJumping = true;
            }
        }
        // we're not grounded, apply faked gravity
        else
        {
            this.moveVector += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }

        // incorporate our impact forces into our move vector
        //ApplyImpactForces();
        Vector3 moveVector = this.moveVector + impactForce;
        // move our character with our calculated moveDirection
        MoveCharacter(moveVector);
        // check our step cycle, according to whether we're walking or running
        ProgressStepCycle(speed);
        // handle our camera, according to whether we're walking or running
        UpdateCameraHeadBobPosition(speed);
    }
    #endregion

    #region Core Movement Functions
    // applies Vector movement to the Character Controller
    void MoveCharacter(Vector3 moveVector)
    {
        // move our character, according to our move vector, per second
        collisionFlags = characterController.Move(moveVector * Time.fixedDeltaTime);
    }

    // converts forward direction with speed into the direction perpendicular of ground slope
    void NormalizeSlopeAngles(float _speed)
    {
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * moveInput.y + transform.right * moveInput.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        // detect the collidors we're touching and store it into our hit info
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                            characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        // align our vector to follow the surface (perpindicular of the normal vector)
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
        // factor speed into our movement vector
        moveVector.x = desiredMove.x * _speed;
        moveVector.z = desiredMove.z * _speed;
    }

    // play landing audio
    void PlayLandingSound()
    {
        // if an audio clip was chosen, play it
        if (landSound != null)
        {
            audioSource.clip = landSound;
            audioSource.Play();
        }
        // determine next step trigger
        nextStep = stepCycle + .5f;
    }

    // play jumping audio
    void PlayJumpSound()
    {
        // if an audio clip was chosen, play it
        if(jumpSound != null)
        {
            audioSource.clip = jumpSound;
            audioSource.Play();
        }
    }

    // determines where our steps are
    void ProgressStepCycle(float speed)
    {
        // check to see if we're moving, or receiving move input
        if (characterController.velocity.sqrMagnitude > 0 && (moveInput.x != 0 || moveInput.y != 0))
        {
            // advance the step cycle
            stepCycle += (characterController.velocity.magnitude + (speed*(isWalking ? 1f : runstepLengthen)))*
                            Time.fixedDeltaTime;
        }
        // if we haven't hit our step threshold, we're done here
        if (!(stepCycle > nextStep))
        {
            return;
        }
        // calculate the new step threshold, and play the footstep
        nextStep = stepCycle + m_StepInterval;

        PlayFootStepAudio();
    }

    // play footstep audio whenever a 'step' happens
    void PlayFootStepAudio()
    {
        //If we're in the air, no footstep audio -> Exit early
        if (!characterController.isGrounded)
            return;
              
        //If the array has sounds in it, pick a sound to play from the array
        if(footstepSounds.Length > 0)
        {
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, footstepSounds.Length);

            //If there's a sound in the slot, play it
            if(footstepSounds[n] != null)
            {
                audioSource.clip = footstepSounds[n];
                audioSource.PlayOneShot(audioSource.clip);
            }

            // move picked sound to index 0 so it's not picked next time
            footstepSounds[n] = footstepSounds[0];
            footstepSounds[0] = audioSource.clip;
        }
    }

    // move camera based on headbob settings
    void UpdateCameraHeadBobPosition (float _speed)
    {
        // temporarily store a new position for calculations, before the final result we apply
        Vector3 newCameraPosition;
        // if we're not using headbob, then exit early, nothing more to do
        if (!useHeadBob)
        {
            return;
        }

        // if we're moving, and the character is grounded, use grounded headbob
        if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
        {
            playerCamera.transform.localPosition =
                m_HeadBob.DoHeadBob(characterController.velocity.magnitude +
                                    (_speed*(isWalking ? 1f : runstepLengthen)));
            //store camera current position into the desired camera position 
            newCameraPosition = playerCamera.transform.localPosition;
            // adjust camera y for offset
            newCameraPosition.y = playerCamera.transform.localPosition.y - m_JumpBob.Offset();
        }
        // we're either not moving, or not grounded, use the jump headbob
        else
        {
            newCameraPosition = playerCamera.transform.localPosition;
            newCameraPosition.y = originalCameraPosition.y - m_JumpBob.Offset();
        }

        // apply our calculation to the actual camera position
        playerCamera.transform.localPosition = newCameraPosition;
    }

    //Read player movement input
    void DetermineFOVKick()
    {
        // store previous walking state, so that we can see if anything has changed
        bool wasWalking = isWalking;

        // keep track of whether or not the character is walking or running
        isWalking = !runInput;

        // handle speed change to give an fov kick
        // if we've just began to run, and we're not standing still, activate the FOVKick
        if (isWalking != wasWalking && useFOVKick && characterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!isWalking ? fovKick.FOVKickUp() : fovKick.FOVKickDown());
        }
    }

    //Determines the speed (based on walking/running) and stores in speed variable for this frame
    void CalculateMoveSpeed(out float speed)
    {
        // set the desired speed to be walking or running
        speed = isWalking ? moveSpeed : moveSpeed * runSpeedMultiplier;
    }

    //Drives camera rotation using the LookRotation on the mouse look script
    void RotateView()
    {
        // apply camera rotation
        mouseLook.LookRotation(transform, playerCamera.transform, rotateInput);
    }

    // callback for when the Character Collider interacts with another collider
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // automatically have access to a rigidbody, if we have the collider
        Rigidbody rigidbody = hit.collider.attachedRigidbody;

        // dont move the rigidbody if the character is on top of it
        if (collisionFlags == CollisionFlags.Below)
        {
            return;
        }

        // if the collider does not accept forces, nothing more to do here
        if (rigidbody == null || rigidbody.isKinematic)
        {
            return;
        }

        // otherwise, apply the force
        rigidbody.AddForceAtPosition(characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
    }
    #endregion

    #region Receive Inputs
    //Set movement Input
    public void SetMoveInput(Vector2 moveInput)
    {
        // if we have player access and are not stunned, process input
        if (player.CanControl && !player.IsPlayerStunned)
            this.moveInput = moveInput;
        else
        {
            this.moveInput = Vector2.zero;
        }
    }

    //Set Rotation input
    public void SetRotateInput(Vector2 rotateInput)
    {
        // if we have player access and are not stunned, process input
        if (player.CanControl && !player.IsPlayerStunned)
            this.rotateInput = rotateInput;
        else
        {
            this.rotateInput = Vector2.zero;
        }
    }

    //Set running input
    public void SetRunInput(bool isRunning)
    {
        // if we have player access and are not stunned, process input
        if (player.CanControl && !player.IsPlayerStunned)
            runInput = isRunning;
        else
        {
            this.runInput = false;
        }
    }

    //Change jump input if we're receiving input from the InputController
    public void SetJumpInput(bool jumpInput)
    {
        // if we have player access and are not stunned, process input
        if (player.CanControl && !player.IsPlayerStunned)
            // change jump input
            this.jumpInput = jumpInput;
        else
        {
            this.jumpInput = false;
        }
    }
    #endregion

    #region Player Functions

    public void AdjustMoveSpeed(float speedChangeAmount)
    {
        // check to make sure we don't go below our max. If it does, return the difference
        speedChangeAmount = ClampSpeedChange(speedChangeAmount);
        // change the speed, then after duration, change it back
        moveSpeed += speedChangeAmount;
    }

    /// <summary>
    /// Change the player's speed for a specified duration, then return it
    /// </summary>
    /// <param name="speedChangeAmount"></param>
    /// <param name="duration"></param>
    public void AdjustMoveSpeed(float speedChangeAmount, float duration)
    {
        // check to make sure we don't go below our max. If it does, return the difference
        speedChangeAmount = ClampSpeedChange(speedChangeAmount);
        // change the speed, then after duration, change it back
        StartCoroutine(IEAdjustMoveSpeed(speedChangeAmount, duration));
    }

    //Apply a force to the controller
    public void AddPlayerForce(Vector3 _force)
    {
        // cancel previous force coroutine
        StopAllCoroutines();
        //remove the previous impact force
        impactForce = Vector3.zero;

        // divide by the mass, to reduce full force effect
        _force = _force / mass;
        // apply the force
        impactForce = _force;
        // start Coroutine that slowly disables force
        StartCoroutine(FadeImpactForce(5f));
    }

    public void SetJumpSpeed(float newJumpSpeed)
    {
        jumpSpeed = newJumpSpeed;
    }
    #endregion

    #region Player Helper Functions
    //Slowly fade the impact force over time
    IEnumerator FadeImpactForce(float fadeTime)
    {
        // set our timer beginnings
        float elapsedTime = 0;
        // keep looping until we have hit our specified time limit

        while (elapsedTime < fadeTime)
        {
            // reduce impact force by a bit
            impactForce = Vector3.Lerp(impactForce, Vector3.zero, (elapsedTime / fadeTime));
            //moveDirection += impactForce;
            elapsedTime += Time.fixedDeltaTime;
            // repeat at the end of the frame
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator IEAdjustMoveSpeed(float speedChangeAmount, float duration)
    {
        // change the speed
        moveSpeed += speedChangeAmount;

        // wait the designated amount of time
        yield return new WaitForSeconds(duration);

        // revert the speed back to original
        moveSpeed -= speedChangeAmount;
    }

    //Pass a speed value into this function to retrieve a speed change value that won't push the player speed below 1
    float ClampSpeedChange(float speedChange)
    {
        //Check to make sure our speed doesn't go below 1
        if ((moveSpeed + speedChange) < playerMinimumMoveSpeed)
        {
            //If it does, return the difference between the original speed and minimum
            speedChange = moveSpeed - playerMinimumMoveSpeed;
            //Return absolute value, so that we don't mix our negatives and positives
            return speedChange;
        }
        else
        {
            //Otherwise, return the original speedChange
            //Return absolute value, so that we don't mix our negatives and positives
            return speedChange;
        }
    }
    #endregion
}
