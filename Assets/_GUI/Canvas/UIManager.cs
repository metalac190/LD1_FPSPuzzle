using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] WaitUI waitUI;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] PauseUI pauseUI;

    PlayerSpawner playerSpawner;
    GameManager gameManager;

    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
        playerSpawner = FindObjectOfType<PlayerSpawner>();
        // events
        gameManager.OnWaitState += HandleWaitState;
        gameManager.OnGameState += HandleGameState;
        gameManager.OnPause += HandlePause;
        gameManager.OnUnpause += HandleUnpause;
        playerSpawner.OnPlayerSpawn += HandlePlayerSpawn;
        playerSpawner.OnPlayerDespawn += HandlePlayerDespawn;
        // disable all panels by default, to account for designer leaving them on
        DisableAllPanels();
    }

    private void OnDestroy()
    {
        gameManager.OnWaitState -= HandleWaitState;
        gameManager.OnGameState -= HandleGameState;
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

    void HandlePlayerSpawn(Player player)
    {
        // enable player UI
        playerUI.ConnectToNewPlayer(player);
    }

    void HandlePlayerDespawn(Player player)
    {
        // disable player UI
        playerUI.ClearPlayer();
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
