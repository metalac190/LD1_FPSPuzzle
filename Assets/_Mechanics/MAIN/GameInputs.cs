using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSpawner))]
public class GameInputs : MonoBehaviour {

    PlayerSpawner playerSpawner;
    GameManager gameManager;

    private void Awake()
    {
        playerSpawner = GetComponent<PlayerSpawner>();
    }

    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
    }

    // Update is called once per frame
    void Update () {
        // handle Wait State inputs
        if (gameManager.IsPaused)
        {
            CheckUnpauseInput();
            return;
        }

        if (gameManager.CurrentGameState == GameState.Wait)
        {
            CheckQuitInput();
            CheckSpawnInput();
        }
        // handle Game State inputs
        if (gameManager.CurrentGameState == GameState.Game)
        {
            CheckPauseInput();
            CheckQuitInput();
            CheckReloadInput();
            CheckReverseCheckpointInput();
            CheckAdvanceCheckpointInput();
        }
    }

    #region Inputs
    private void CheckAdvanceCheckpointInput()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            playerSpawner.AdvanceCheckPoint();
        }
    }

    private void CheckReverseCheckpointInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            playerSpawner.ReverseCheckPoint();
        }
    }

    private void CheckReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            // Reload the current scene, using the updated player spawn location
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void CheckSpawnInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.ActivateGameState();
        }
    }

    private void CheckQuitInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void CheckPauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.Pause();
        }
    }

    private void CheckUnpauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameManager.Unpause();
        }
    }
    #endregion
}
