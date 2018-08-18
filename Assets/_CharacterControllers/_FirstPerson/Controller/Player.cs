using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

//NOTE: does not support multiple colliders on the root. Just pick one, assuming Character Controller
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour, IPlayer {

    #region Variables and Classes
    [Header("Setup")]
    //[SerializeField] PlayerUI playerUIPrefab;     // UI that we will spawn
    [SerializeField] Camera playerCamera;        // camera that should be active after player spawn
    public Camera PlayerCamera { get { return playerCamera; } }
    [SerializeField] Behaviour[] disableComponentsOnDeath;    // components to disable on death
    [SerializeField] GameObject[] disableGameObjectsOnDeath;    // gameObjects to disable on death

    public event Action OnPlayerDeath = delegate { };   // player has died

    public bool CanControl { get; private set; }
    public bool IsPlayerStunned { get; private set; }

    //TODO hook up the SFX
    [Header("SFX")]
    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip stunSound;

    [Header("GFX")]
    [SerializeField] GameObject spawnGFX;        // effect that happens when we spawn
    [SerializeField] GameObject stunGFX;        // effect that happens when we're stunned

    // states
    private bool isInvulnerable = false;        // whether or not the player is invulnerable

    // book-keeping
    private bool[] wasEnabled;      //Store starting enabled state for components in the 'on death disable' list

    // caching
    Health health = null;          //Reference to player's health, so that we can access it later

    Collider playerCollider = null;
    //PlayerUI playerUIInstance = null;
    IPlayerMotor playerMotor = null;      // reference to the player motor

    #endregion

    #region Setup
    void Awake()
    {
        // fill references
        playerCollider = GetComponent<Collider>();
        health = GetComponent<Health>();

        playerMotor = GetComponent<IPlayerMotor>();
        if(playerMotor == null)
        {
            Debug.LogWarning("No Player Motor script found on the player!");
        }
        // subscribe to events
        health.OnDeath.AddListener(HandleDeath);
    }

    private void Start()
    {
        // call the Setup function on the Player to initialize variables
        Setup();
    }

    void OnDestroy()
    {
        health.OnDeath.RemoveListener(HandleDeath);
    }

    //Set up the player default camera and communicate new player info to the host
    public void Setup()
    {
        // set state variables
        isInvulnerable = false;
        CanControl = true;
        IsPlayerStunned = false;
        
        // activate spawn graphics
        if(spawnGFX != null)
        {
            //Create spawn effect
            GameObject spawnGFX = (GameObject)Instantiate(this.spawnGFX, transform.position, Quaternion.identity);
            Destroy(spawnGFX, 3f);
        }

        // set active camera to this player
        //if (playerCamera != null)
            //playerCamera = Camera.main;

        //TODO add Spawn Audio

        // run any designated player spawn events
        //OnPlayerSpawn.Invoke();
    }
    #endregion

    #region Custom Functions
    void HandleDeath()
    {
        DisablePlayer();
        OnPlayerDeath.Invoke();
        //StartCoroutine(PlayerDespawn(despawnTimeAfterDeath));
    }

    //Disable the player properly
    public void DisablePlayer()
    {
        DisableDeathComponents();    // disable components previously specified
        DisableDeathGameObjects();
    }

    //Disables designated components on the player
    void DisableDeathComponents()
    {
        // disable Components so the player can't continue moving
        for (int i = 0; i < disableComponentsOnDeath.Length; i++)
        {
            disableComponentsOnDeath[i].enabled = false;
        }
        // check to make sure we also disable the collider (it won't be caught by the behaviour loop)
        if (playerCollider != null)
            playerCollider.enabled = false;
    }

    void DisableDeathGameObjects()
    {
        // disable GameObjects associated with player (especially the gfx)
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }
    }

    //Coroutine for Stun function
    IEnumerator IEStun(float stunDuration)
    {
        // switch state to stunned
        IsPlayerStunned = true;
        // disable player attacks

        //TODO visual feedback
        //TODO audio feedback

        // wait designated time
        yield return new WaitForSeconds(stunDuration);
        // TODO disable visual feedback
        // TODO disable audio feedback

        // state change, no longer stunned
        IsPlayerStunned = false;
    }



    #endregion

    #region Player Functions

    /// <summary>
    /// This function will enable or disable player controls. This boolean gets fed into the Movement Controller
    /// </summary>
    /// <param name="canControl"></param>
    public void SetControllable(bool canControl)
    {
        CanControl = canControl;
    }

    /// <summary>
    /// Player takes a hit, which passes in a variety of attributes. Most enemy damage events will call this.
    /// </summary>
    /// <param name="damageTaken"></param>
    /// <param name="pushAmount"></param>
    /// <param name="stunDuration"></param>
    /// <param name="impactLocation"></param>
    public void PlayerHit(int damageTaken, float pushAmount, float stunDuration, Vector3 impactLocation)
    {
        // if we're invulnerable, ignore this hit
        if (isInvulnerable)
            return;

        // apply damage
        health.TakeDamage(damageTaken);
        // then apply the extra hit attributes
        Pushback(pushAmount, impactLocation);
        Stun(stunDuration);
    }

    /// <summary>
    /// Calculate and apply a force on the player in the reverse direction
    /// </summary>
    /// <param name="pushStrength"></param>
    /// <param name="impactLocation"></param>
    public void Pushback(float pushStrength, Vector3 impactLocation)
    {
        // calculate  and apply a force on the player. Subtractacing position from force origin gives reverse direction from origin
        Vector3 forceToApply = pushStrength * (transform.position - impactLocation).normalized;
        //TODO using SendMessage to avoid creating a connection with the player Controller. Fix this later
        SendMessage("AddPlayerForce", forceToApply, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Player is stunned. Disable controls for a set amount of time;
    /// </summary>
    /// <param name="stunDuration"></param>
    public void Stun(float stunDuration)
    {
        // if we're not already stunned, start a coroutine to stun
        if (!IsPlayerStunned)
        {
            StartCoroutine(IEStun(stunDuration));
        }
    }

    /// <summary>
    /// Applies a force in the given direction with the given force.
    /// </summary>
    /// <param name="forceDirection"></param>
    /// <param name="forceAmount"></param>
    public void ApplyForce(Vector3 forceDirection, float forceAmount)
    {
        // calculate the total force to apply
        Vector3 forceToApply = forceDirection.normalized * forceAmount;
        //TODO using SendMessage to avoid creating a connection with the player Controller. Fix this later
        playerMotor.AddPlayerForce(forceToApply);
        //SendMessage("AddPlayerForce", forceToApply, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Applies a force in the given x,y,z direction with the given force.
    /// </summary>
    /// <param name="xDirection"></param>
    /// <param name="yDirection"></param>
    /// <param name="zDirection"></param>
    /// <param name="forceAmount"></param>
    public void ApplyForce(float xDirection, float yDirection, float zDirection, float forceAmount)
    {
        // calculate the total force to apply
        Vector3 forceDirection = new Vector3(xDirection, yDirection, zDirection);
        Vector3 forceToApply = forceDirection.normalized * forceAmount;
        playerMotor.AddPlayerForce(forceToApply);
        //TODO using SendMessage to avoid creating a connection with the player Controller. Fix this later
        //SendMessage("AddPlayerForce", forceToApply, SendMessageOptions.DontRequireReceiver);
    }

    #endregion

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            health.Kill();
        }
    }
}
