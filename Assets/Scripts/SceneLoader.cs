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
        GameInformation.gameOver = false;
        GameInformation.openingSequence = true;
        GameInformation.playerOneScore = 0;
        GameInformation.playerTwoScore = 0;
        GameInformation.playerOneResources = new int[4] { 0, 0, 0, 0 };
        GameInformation.playerTwoResources = new int[4] { 0, 0, 0, 0 };
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNetworkScene()
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
