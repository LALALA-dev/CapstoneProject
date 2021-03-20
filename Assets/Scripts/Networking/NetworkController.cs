using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController networkController;
    public GameController gameController;

    public static string boardState;
    public static string netPlayerName = "";
    public static string netOpponentsName = "";

    #region Set Up
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        networkController = this;
        gameController = GameController.getInstance();
    }

    #endregion

    #region Class Member Manipulation Functions
    public void SetMove(string move)
    {
        Debug.Log("HOST = " + GameInformation.playerIsHost + " SETMOVE() CALLED, BOARDCONFIG = " + move);
        boardState = move;
        GameInformation.newNetworkMoveSet = true;
    }

    public void SetOpeningBoardConfiguration(string openingBoard)
    {
        Debug.Log("HOST = " + GameInformation.playerIsHost + " SETMOVE() CALLED, BOARDCONFIG = " + openingBoard);
        boardState = openingBoard;
    }

    public string GetMove()
    {
        return boardState;
    }

    public void GetOpponentInfo(string name)
    {
        netOpponentsName = name;
    }

    public void SetInfo(string name)
    {
        netPlayerName = name;
    }

    public void InvokeRenderHost()
    {
        if (!GameInformation.playerIsHost)
        {
            GameInformation.renderClientBoard = true;
        }
    }

    public void InvokeTriggerToggle()
    {
        GameInformation.enableTriggers = true;
    }

    #endregion

    #region Network Broadcast Functions

    public void SendMove()
    {
        NetworkPlayer.player.SendMove(boardState);
    }

    public void SendMove(string gameBoard)
    {
        NetworkPlayer.player.SendMove(gameBoard);
    }

    public void SendOpeningBoardConfiguration(string openingBoardState)
    {
        NetworkPlayer.player.SendOpeningBoardConfiguration(openingBoardState);
    }

    public void InvokeClientsRenderHost()
    {
        NetworkPlayer.player.InvokeHostConfiguration();
    }

    public void EnableOpponentsTriggers()
    {
        NetworkPlayer.player.InvokeEnableTriggers();
    }

    #endregion
}
