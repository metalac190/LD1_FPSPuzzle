using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour {

    [SerializeField] Text messageText;          // text for the player

    private void Awake()
    {
        ClearMessage();
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

    void HandlePlayerDeath()
    {
        DisplayMessage("DEAD...", 0);
    }
}
