﻿using Photon.Pun;
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
        gameController = GameController.getInstance();
    }

    #endregion

    #region Public Functions
    public void GetOpponentInfo(string name)
    {
        netOpponentsName = name;
        Debug.Log("Your opponent's name is " + netOpponentsName + "!");
    }

    public void SendOpponentBoardConfiguration(GameBoard gameBoard)
    {
        NetworkPlayer.player.SendHostBoardConfiguration(gameBoard);
    }

    public void SetClientBoardConfiguration(GameBoard gameBoard)
    {
        if (!GameInformation.playerIsHost)
            gameController.SetBoardConfiguration(gameBoard);
    }

    #endregion
}
