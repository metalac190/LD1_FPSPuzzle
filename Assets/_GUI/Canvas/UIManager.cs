using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for feeding gameplay connections and references into the individual UI Canvases. It
/// also displays/hides relevant canvases at different points during the game state
/// </summary>
public class UIManager : MonoBehaviour {

    [SerializeField] WaitUI waitUI;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] PauseUI pauseUI;
    [SerializeField] CursorController cursorController;

    PlayerSpawner playerSpawner;
    GameManager gameManager;

    public void Awake ()
    {
        // inject
        gameManager = FindObjectOfType<GameManager>();
        playerSpawner = FindObjectOfType<PlayerSpawner>();
        // events
        gameManager.OnWaitState += HandleWaitState;
        gameManager.OnGameState += HandleGameState;
        gameManager.OnPause += HandlePause;
        gameManager.OnUnpause += HandleUnpause;
        // initialize
        playerUI.Initialize(playerSpawner);
        pauseUI.Initialize(gameManager);
        cursorController.Initialize(gameManager);
        // disable all panels by default, to account for designer leaving them on
        DisableAllPanels();
    }

    private void OnDestroy()
    {
        gameManager.OnWaitState -= HandleWaitState;
        gameManager.OnGameState -= HandleGameState;
        gameManager.OnPause -= HandlePause;
        gameManager.OnPause -= HandleUnpause;
    }

    void HandleWaitState()
    {
        // activate wait state panels
        DisableAllPanels();
        waitUI.gameObject.SetActive(true);
    }

    void HandleGameState()
    {
        // activate game state panels
        DisableAllPanels();
        messageUI.gameObject.SetActive(true);
        playerUI.gameObject.SetActive(true);
    }

    void HandlePause()
    {
        pauseUI.gameObject.SetActive(true);
    }

    void HandleUnpause()
    {
        pauseUI.gameObject.SetActive(false);
    }

    //Use this function to disable all of your panels. Useful for creating a clean slate
    public void DisableAllPanels()
    {
        // disable each one of the panel game objects
        waitUI.gameObject.SetActive(false);
        playerUI.gameObject.SetActive(false);
        messageUI.gameObject.SetActive(false);
        pauseUI.gameObject.SetActive(false);
    }


}
