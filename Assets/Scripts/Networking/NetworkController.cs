using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController NetController;
    public static string netOpponentsName;
    public static string networkPlayerName = "Player";

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

    public void SetNetworkPlayerName()
    {
        NetworkPlayer.player.name = networkPlayerName;
        NetworkPlayer.player.SendPlayerInfo(networkPlayerName);
    }

    #endregion
}
