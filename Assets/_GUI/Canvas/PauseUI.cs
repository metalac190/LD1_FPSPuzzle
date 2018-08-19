using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour {

    GameObject[] objectsImmuneToPause;
    GameManager gameManager;

    [SerializeField] bool lockMouseOnUnpause = true;
    [SerializeField] Button unpauseButton;
    [SerializeField] Button quitButton;

    public void Initialize(GameManager newGameManager)
    {
        // inject
        this.gameManager = newGameManager;
        // events
        this.gameManager.OnPause += HandlePause;
        this.gameManager.OnUnpause += HandleUnpause;
        // local components
        SetupButtons();
    }

    private void OnDestroy()
    {
        gameManager.OnPause -= HandlePause;
        gameManager.OnUnpause -= HandleUnpause;
    }

    void SetupButtons()
    {
        // subscribe through code so that designers can't break things
        unpauseButton.onClick.AddListener(gameManager.ActivateUnpause);
        quitButton.onClick.AddListener(SceneLoader.QuitGameStatic);
    }

    void HandlePause()
    {
        // pause logic here
        // make sure mouse can move during pause screen
        Cursor.lockState = CursorLockMode.Confined;
    }

    void HandleUnpause()
    {
        // unpause logic here
        // revert mouse to preferred movement
        if (lockMouseOnUnpause)
        {
            Debug.Log("Lock the mouse!");
            Cursor.lockState = CursorLockMode.Locked;
        }

        else
        {
            Debug.Log("Mouse unlocked");
        }
            Cursor.lockState = CursorLockMode.None;
    }

    public void Unpause()
    {
        gameManager.ActivateUnpause();
    }

    public void Quit()
    {
        SceneLoader.QuitGameStatic();
    }
}
