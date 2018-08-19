using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class CursorController {

    [SerializeField] Texture2D cursorImage;     //TODO

    GameManager gameManager;

    #region Setup
    public void Initialize(GameManager gameManager)
    {
        // inject
        this.gameManager = gameManager;
        // events
        gameManager.OnGameState += HandleGameState;
        gameManager.OnWaitState += HandleWaitState;
        gameManager.OnPause += HandlePause;
        gameManager.OnUnpause += HandleUnpause;
        // local state
        AssignCursorImage();
    }

    void OnDestroy()
    {
        gameManager.OnGameState -= HandleGameState;
        gameManager.OnWaitState -= HandleWaitState;
        gameManager.OnPause -= HandlePause;
        gameManager.OnUnpause -= HandleUnpause;
    }

    void AssignCursorImage()
    {
        //TODO
    }
    #endregion

    #region Game Events
    public void HandleWaitState()
    {
        UnlockCursor();
    }

    public void HandleGameState()
    {
        LockCursor();
    }

    public void HandlePause()
    {
        UnlockCursor();
    }

    public void HandleUnpause()
    {
        LockCursor();
    }
    #endregion

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
