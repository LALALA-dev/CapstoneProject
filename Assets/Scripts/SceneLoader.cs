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
        GameInformation.roomName = "StandardRoom";
        GameInformation.networkGameType = NetworkGameType.Public;
        GameInformation.playerIsHost = true;
        GameInformation.gameType = 'A';
        GameInformation.openingSequence = true;
        GameInformation.openingMoveNodeSet = false;
        GameInformation.openingMoveBranchSet = false;
        GameInformation.openingNodeId = 0;
        GameInformation.openingBranchId = 0;
        GameInformation.playerOneResources = new int[4] { 0, 0, 0, 0 };
        GameInformation.playerTwoResources = new int[4] { 0, 0, 0, 0 };
        GameInformation.maxTradeResources = new int[4] { 0, 0, 0, 0 };
        GameInformation.resourceTrade = false;
        GameInformation.playerOneScore = 0;
        GameInformation.playerTwoScore = 0;
        GameInformation.playerOneNetwork = 0;
        GameInformation.playerTwoNetwork = 0;
        GameInformation.gameOver = false;
        GameInformation.currentPlayer = "HUMAN";
        GameInformation.humanMoveFinished = false;
        GameInformation.tradeHasBeenMade = false;
        GameInformation.HumanNetworkProtocol = false;
        GameInformation.currentRoundPlacedNodes = new List<int>();
        GameInformation.currentRoundPlacedBranches = new List<int>();
        GameInformation.renderClientBoard = false;
        GameInformation.newNetworkMoveSet = false;
        GameInformation.ownAvatar = "CAR";
        GameInformation.playerOneAvatar = "CAR";
        GameInformation.playerTwoAvatar = "WHEELBARREL";
        GameInformation.needToSyncGameVariables = false;
        GameInformation.needToUpdateOpponentsResources = false;
        GameInformation.needToSyncAvatars = false;

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
