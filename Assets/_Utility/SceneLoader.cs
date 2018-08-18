using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//This class handles most of the loading functions for navigating scenes. Attach this to a SceneManager object
//and get a reference to it in order to call
public class SceneLoader : MonoBehaviour {

    //Load scene by string name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Static version of our LoadSceneStatic. We need the non-static version because UI buttons don't accept static version
    public static void LoadSceneStatic(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Load next scene in index
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Reloads current scene
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Static version of our ReloadScene. We need the non-static version because UI buttons don't accept static version
    public static void ReloadSceneStatic()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Quits application. Putting this into a function so that we can call from a button
    public void QuitGame()
    {
        Application.Quit();
    }

    //Static version of our QuitGame. We need the non-static version because UI buttons don't accept static version
    public static void QuitGameStatic()
    {
        Application.Quit();
    }

    public static int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}
