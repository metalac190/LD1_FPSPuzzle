using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataAssets;

public class PlayerUI : MonoBehaviour {

    [Header("UI Objects")]
    [SerializeField] Slider healthSlider;       // slider that keeps track of health
    //[SerializeField] IntAsset playerMaxHealth;   
    //[SerializeField] IntAsset playerCurrentHealth;      
    int playerMaxHealth;    // max value for player health
    int playerCurrentHealth;    // health value to keep track of  
    Health playerHealth;

    //Initialize UI values to their starting defaults
    public void ConnectToNewPlayer(Player player)
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
        // add visual
    }

    void HandleHeal()
    {
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;
        // add visual
    }

    // disconnect from current player
    public void ClearPlayer()
    {
        
    }
}
