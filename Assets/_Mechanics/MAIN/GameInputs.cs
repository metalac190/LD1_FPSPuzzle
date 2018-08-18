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
        // handle Intro State inputs
        if (gameManager.CurrentGameState == GameManager.GameState.Intro)
        {
            CheckQuitInput();
            CheckSpawnInput();
        }
        // handle Game State inputs
        if (gameManager.CurrentGameState == GameManager.GameState.Game)
        {
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

    private static void CheckReloadInput()
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
            playerSpawner.SpawnPlayer();
        }
    }

    private static void CheckQuitInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    #endregion
}
