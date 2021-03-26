using Photon.Pun;
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

    public void LoadAIScene()
    {
        SceneManager.LoadScene("AISelectionScene");
    }

    public void LoadMenuScene()
    {
        if(GameInformation.gameType == 'N')
        {
            if(PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            }
        }
        GameController.Destroy();
        GameInformation.gameOver = false;
        GameInformation.openingSequence = true;
        GameInformation.currentPlayer = "HUMAN";
        GameInformation.playerIsHost = true;
        GameInformation.playerOneScore = 0;
        GameInformation.playerTwoScore = 0;
        GameInformation.playerOneNetwork = 0;
        GameInformation.playerTwoNetwork = 0;
        GameInformation.playerOneResources = new int[4] { 0, 0, 0, 0 };
        GameInformation.playerTwoResources = new int[4] { 0, 0, 0, 0 };
        GameInformation.openingMoveBranchSet = false;
        GameInformation.openingMoveNodeSet = false;
        GameInformation.gameType = 'A';
        GameInformation.HumanNetworkProtocol = false;
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

    public static void LoadLocalGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public static void LoadNetworkLobbyScene()
    {
        SceneManager.LoadScene("NetworkLobbyScene");
    }

    public void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
