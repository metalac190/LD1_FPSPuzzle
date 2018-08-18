using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] GameUI gameUI;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] PauseUI pauseUI;

    PlayerSpawner playerSpawner;
    GameManager gameManager;

    private void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
        // events
        gameManager.OnIntroState += HandleIntroState;
        gameManager.OnGameState += HandleGameState;
    }

    private void OnDestroy()
    {
        gameManager.OnIntroState -= HandleIntroState;
        gameManager.OnGameState -= HandleGameState;
    }

    void HandleIntroState()
    {

    }

    void HandleGameState()
    {

    }

    void HandlePlayerSpawn(Player player)
    {
        playerUI.ConnectToNewPlayer(player);
    }

    void HandlePlayerDespawn()
    {
        playerUI.ClearPlayer();
    }

    public void DisablePlayerUI()
    {
        playerUI.gameObject.SetActive(false);
    }

    //Use this function to disable all of your panels. Useful for creating a clean slate
    public void DisableAllPanels()
    {
        // disable each one of the panel game objects

    }


}
