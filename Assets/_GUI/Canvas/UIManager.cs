using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] GameUI gameUI;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] MessageUI messageUI;
    [SerializeField] PauseUI pauseUI;

    PlayerSpawner playerSpawner;

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Initialize()
    {
        
    }

    public void DisablePlayerUI()
    {
        playerUI.gameObject.SetActive(false);
    }

    void HandlePlayerSpawn(Player player)
    {
        playerUI.Initialize(player);
    }

    void HandlePlayerDespawn()
    {
        playerUI.gameObject.SetActive(false);
    }

    //Use this function to disable all of your panels. Useful for creating a clean slate
    public void DisableAllPanels()
    {
        // disable each one of the panel game objects

    }
}
