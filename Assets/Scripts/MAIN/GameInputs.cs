using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerSpawner))]
public class GameInputs : MonoBehaviour {

    PlayerSpawner playerSpawner;

    private void Awake()
    {
        playerSpawner = GetComponent<PlayerSpawner>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            // Reload the current scene, using the updated player spawn location
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            playerSpawner.ReverseCheckPoint();
        }

        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            playerSpawner.AdvanceCheckPoint();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerSpawner.SpawnPlayer();
        }
    }
}
