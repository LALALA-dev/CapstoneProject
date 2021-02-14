using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController NetController;
    public GameController gameController;

    public static string netOpponentsName;

    #region Set Up
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        NetController = this;
    }

    #endregion

    #region Public Functions
    public void GetOpponentInfo(string name)
    {
        netOpponentsName = name;
        Debug.Log("Your opponent's name is " + netOpponentsName + "!");
    }

    public void SendOpponentBoardConfiguration(string gameBoard)
    {
        NetworkPlayer.player.SendHostBoardConfiguration(gameBoard);
    }

    public void SetClientBoardConfiguration(string gameBoard)
    {
        if (!GameInformation.playerIsHost)
            gameController.SetBoardConfiguration(gameBoard);
    }

    #endregion
}
