using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController networkController;
    public GameController gameController;

    public string boardState = "";
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

    #region Public Functions

    public void SendMove()
    {
        NetworkPlayer.player.SendMove(boardState);
    }

    public void SetMove(string boardState)
    {
        networkController.boardState = boardState;
    }

    public string GetMove()
    {
        return boardState;
    }

    public void GetOpponentInfo(string name)
    {
        netOpponentsName = name;
        Debug.Log("Your opponent's name is " + netOpponentsName + "!");
    }

    public void SetInfo(string name)
    {
        netPlayerName = name;
    }

    public void SendMove(string gameBoard)
    {
        NetworkPlayer.player.SendMove(gameBoard);
    }

    public void SendOpeningBoardConfiguration(string openingBoardState)
    {
        NetworkPlayer.player.SendMove(openingBoardState);
    }

    #endregion
}
