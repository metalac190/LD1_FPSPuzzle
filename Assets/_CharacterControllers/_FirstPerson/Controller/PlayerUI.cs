using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataAssets;

public class PlayerUI : MonoBehaviour {

    // group necessary panels into a class, to make it easier to keep track
    [System.Serializable]
    public class UIPanels
    {
        public GameObject pause;        // pause menu panel
    }

    [Header("UI Objects")]
    public UIPanels panels = new UIPanels();    // create a new UI Panel grouping
    [SerializeField] Slider healthSlider;       // slider that keeps track of health
    //[SerializeField] IntAsset playerMaxHealth;   
    //[SerializeField] IntAsset playerCurrentHealth;      
    int playerMaxHealth;    // max value for player health
    int playerCurrentHealth;    // health value to keep track of  
    [SerializeField] Text messageText;          // text for the player

    Health playerHealth;

    //Initialize UI values to their starting defaults
    public void Initialize(Player player)
    {
        // inject
        playerHealth = player.GetComponent<Health>();
        // events
        playerHealth.OnTakeDamage.AddListener(HandleTakeDamage);
        playerHealth.OnHeal.AddListener(HandleHeal);
        playerHealth.OnDeath.AddListener(HandleDeath);
        // set initial state
        InitialState();
    }

    void InitialState()
    {
        healthSlider.maxValue = playerHealth.MaxHealth;
        healthSlider.value = playerHealth.CurrentHealth;

        DisableAllPanels();
        ClearMessage();
    }

    private void OnDestroy()
    {
        playerHealth.OnTakeDamage.RemoveListener(HandleTakeDamage);
        playerHealth.OnHeal.RemoveListener(HandleHeal);
        playerHealth.OnDeath.RemoveListener(HandleDeath);
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

    private void Update()
    {
        //TODO only do this if amount is less than max

        /*
        //Pause Menu Input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
        */
    }

    //Use this function to disable all of your panels. Useful for creating a clean slate
    public void DisableAllPanels()
    {
        // disable each one of the panel game objects
        panels.pause.SetActive(false);
    }

    //Toggles the pause menu
    public void TogglePauseMenu()
    {
        //Toggle active state
        panels.pause.SetActive(!panels.pause.activeSelf);
    }

    /// <summary>
    /// Display the string received to the Message panel. This is useful for displaying helpful text in the level. 
    /// Text can be set to be cleared after a designated amount of time
    /// </summary>
    /// <param name="messageToDisplay"></param>
    /// <param name="secondsToDisplay"></param>
    public void DisplayMessage(string messageToDisplay, float secondsToDisplay)
    {
        CancelInvoke(); //Cancel all other timed invokes in process
                        //If our display time is greater than 0, display for that amount of time and then clear it
        if (secondsToDisplay == 0)
        {
            messageText.text = messageToDisplay;    //Display the Message onto the message panel
        }
        else if (secondsToDisplay > 0)
        {
            messageText.text = messageToDisplay;
            Invoke("ClearMessage", secondsToDisplay);
        }
    }

    /// <summary>
    /// Clear the Message panel by passing it an empty string
    /// </summary>
	public void ClearMessage()
    {
        messageText.text = "";  //Clear the message panel
    }

    // Gets called on player death
    void HandleDeath()
    {
        DisplayMessage("DEAD...", 0);
    }
}
