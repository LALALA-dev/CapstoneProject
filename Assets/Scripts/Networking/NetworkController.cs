﻿using Photon.Pun;
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

    #region Public Functions

    public void InvokeRenderHost()
    {
        // Send message to GameManager
        if(!GameInformation.playerIsHost)
        {
            Debug.Log("Sending Message to GameManager");
            GameInformation.renderClientBoard = true;
        }
    }

    public void SendMove()
    {
        NetworkPlayer.player.SendMove(boardState);
    }

    public void SetMove(string move)
    {
        Debug.Log("HOST = " + GameInformation.playerIsHost + " SETMOVE() CALLED, BOARDCONFIG = " + move);
        boardState = move;

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

    public void InvokeClientsRenderHost()
    {
        NetworkPlayer.player.InvokeHostConfiguration();
    }

    #endregion
}
