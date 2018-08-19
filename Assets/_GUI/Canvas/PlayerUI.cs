using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataAssets;

public class PlayerUI : MonoBehaviour {

    [Header("UI Objects")]
    [SerializeField] Slider healthSlider;       // slider that keeps track of health     
    [SerializeField] FlashImage damageFlash;    // script that flashes UI when we take damage
    int playerMaxHealth;    // max value for player health
    int playerCurrentHealth;    // health value to keep track of  

    Health playerHealth;
    PlayerSpawner playerSpawner;

    public void Initialize(PlayerSpawner playerSpawner)
    {
        // inject
        this.playerSpawner = playerSpawner;
        // events
        playerSpawner.OnPlayerSpawn += HandlePlayerSpawn;
        playerSpawner.OnPlayerDespawn += HandlePlayerDespawn;
    }

    //Initialize UI values to their starting defaults
    public void InitializeNewPlayer(Player player)
    {
        // inject
        playerHealth = player.GetComponent<Health>();
        // events
        playerHealth.OnTakeDamage.AddListener(HandleTakeDamage);
        playerHealth.OnHeal.AddListener(HandleHeal);
        // set initial state
        InitialState();
    }

    void InitialState()
    {
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
    }

    void HandleTakeDamage()
    {
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
        // visual feedback
        damageFlash.Flash(false);
    }

    void HandleHeal()
    {
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
        // add visual
    }

    void HandlePlayerSpawn(Player player)
    {
        // enable player UI
        InitializeNewPlayer(player);
    }

    void HandlePlayerDespawn(Player player)
    {
        // disable player UI
        ClearPlayer();
    }

    // disconnect from current player
    public void ClearPlayer()
    {
        playerHealth = null;
    }
}
