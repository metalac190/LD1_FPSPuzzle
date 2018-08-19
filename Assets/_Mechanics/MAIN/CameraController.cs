using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour {
    
    #region Setup
    [Header("Cameras")]
    [SerializeField] private Camera sceneCamera = null;     // camera to use when there are no players in the scene
    public Camera SceneCamera { get { return sceneCamera; } }
    [SerializeField] private Camera playerCamera = null;    // camera to use when player has spawned
    public Camera PlayerCamera { get { return playerCamera; } }

    public Camera ActiveCamera { get; private set; }
    private Camera previousCamera;      // lets us know what camera to return to (useful for cutscenes)

    public event Action<Camera> OnCameraChange = delegate { };  // on camera change, returns the currently active camera

    Player activePlayer;    // need this reference for listening for the Player Death event
    PlayerSpawner playerSpawner;
    GameManager gameManager;

    public void Awake()
    {
        // external references
        playerSpawner = FindObjectOfType<PlayerSpawner>();
        gameManager = FindObjectOfType<GameManager>();
        // fill in empty references
        SetDefaultStates();
    }

    private void Start()
    {
        DisableLevelCameras();
        DetermineActiveCamera();
    }

    private void SetDefaultStates()
    {
        if (sceneCamera == null)
        {
            Debug.LogError("No Scene Camera. Assign to Camera Controller");
        }
        else
        {
            ActiveCamera = sceneCamera;
            previousCamera = sceneCamera;
        }
    }

    private void OnEnable()
    {
        playerSpawner.OnPlayerSpawn += HandlePlayerSpawn;
        gameManager.OnWaitState += HandleWaitState;
    }

    private void OnDisable()
    {
        playerSpawner.OnPlayerSpawn -= HandlePlayerSpawn;
        gameManager.OnWaitState -= HandleWaitState;
    }

    // turn off all of our level cameras, in case they were left on accidentally
    void DisableLevelCameras()
    {
        Camera[] sceneCameras = FindObjectsOfType<Camera>();
        foreach (Camera camera in sceneCameras)
        {
            camera.enabled = false;
        }
    }

    void DetermineActiveCamera()
    {
        // use this if you're not sure which camera should be active. It's more of a fail safe
        if (PlayerCamera != null)
        {
            ActivatePlayerCamera();
        }
        else
        {
            ActivateSceneCamera();
        }
    }
    #endregion

    #region Private Functions
    void HandlePlayerSpawn(Player newPlayer)
    {
        activePlayer = newPlayer;

        // if the player has designated a camera, use that. Otherwise use CameraController specified
        if(activePlayer.PlayerCamera != null)
        {
            SetPlayerCamera(activePlayer.PlayerCamera);
        }
        ActivatePlayerCamera();
    }

    // no longer in game
    void HandleWaitState()
    {
        activePlayer = null;
        ActivateSceneCamera();
    }

    void ActivateCamera(Camera cameraToActivate)
    {
        if (cameraToActivate == null)
            return;

        previousCamera = ActiveCamera;      // store previous camera so we can return to it if necessary
        ActiveCamera = cameraToActivate;    // store which camera it is, we may need to know
        previousCamera.enabled = false;     // turn off the old one, so that we don't have multiple active cameras
        ActiveCamera.enabled = true;        // enable it, so that we starting rendering
        OnCameraChange.Invoke(ActiveCamera);    // let everything else that cares know
    }
    #endregion

    #region Public Functions
    public void RevertCameraToPrevious()
    {
        Camera oldActiveCamera;   // temporarily hold onto the new previous, since we're re-writing old previous

        ActiveCamera.enabled = false;
        // temporarily store the original active camera, so that when we reassign active to previous, we still have the original
        oldActiveCamera = ActiveCamera; // store it
        ActiveCamera = previousCamera;  // reassign active to previous
        previousCamera = oldActiveCamera;   // reassign previous to our original old active camera that we overwrote

        ActiveCamera.enabled = true;
        OnCameraChange.Invoke(ActiveCamera);
    }

    //Change the active camera
    public void ActivateSceneCamera()
    {
        //Make sure that we have a scene camera, exit early if we dont (to avoid errors)
        if (SceneCamera == null)
        {
            Debug.LogWarning("No scene camera assigned, cannot set active!");
            return;
        }
        ActivateCamera(SceneCamera);
    }

    public void ActivatePlayerCamera()
    {
        if (PlayerCamera == null)
        {
            Debug.LogWarning("No player camera assigned, cannot set active!");
            return;
        }
        ActivateCamera(PlayerCamera);
    }

    public void ActivateNewCamera(Camera cameraToActivate)
    {
        ActivateCamera(cameraToActivate);
    }

    public void SetSceneCamera(Camera newCamera)
    {
        sceneCamera = newCamera;
    }

    public void SetPlayerCamera(Camera newCamera)
    {
        playerCamera = newCamera;
    }

    public IEnumerator DelayedCameraSwitch(float secondsToDelay, Camera cameraToSwitchTo)
    {
        Debug.Log("handle player death");
        yield return new WaitForSeconds(secondsToDelay);
        ActivateCamera(cameraToSwitchTo);
    }
    #endregion
}
