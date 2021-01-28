using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI messageLog;
    private GameBoard game;

    void Start()
    {
        game = new GameBoard();
        Debug.Log(game.branches[0].ownership);
        Connect();
    }

    // Update is called once per frame

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        CreateOrJoinRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        messageLog.text = "Disconnected from server because: " + cause.ToString();
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        if(PhotonNetwork.IsConnected)
        {
            //CreateOrJoinRoom();
        }
        else
        {
            Debug.Log("Connecting to server...");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
            //CreateOrJoinRoom();
        }
    }

    public void CreateOrJoinRoom()
    {
        //if (PhotonNetwork.IsConnected)
        //    return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        Photon.Pun.PhotonNetwork.JoinOrCreateRoom("basic", roomOptions, TypedLobby.Default);
    }

    public override void OnConnected()
    {
        //CreateOrJoinRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room!");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Joined Room Failed");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed");
    }
}
