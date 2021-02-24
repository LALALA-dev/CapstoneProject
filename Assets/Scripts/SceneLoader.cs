using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void LoadNetworkScene()
    {
        SceneManager.LoadScene("MultiplayerPreferences");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLocalGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadNetworkLobbyScene()
    {
        SceneManager.LoadScene("NetworkLobbyScene");
    }
}
