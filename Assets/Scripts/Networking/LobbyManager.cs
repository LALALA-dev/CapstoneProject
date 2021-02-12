using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI statusText;

    #region Set Up
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        if(PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            statusText.text = "Waiting on Host to Start!";
            Debug.Log("Waiting on Host to Start!");
        }
    }

    private void Start()
    {
        statusText.text = "Connecting to Room";
        Connect();
    }
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Successfully Connected to Server");

        switch (GameInformation.networkGameType)
        {
            case NetworkGameType.Public:
                JoinPublicRoom();
                break;
            case NetworkGameType.Private:
                JoinPrivateRoom();
                break;
            case NetworkGameType.Host:
                CreateHostRoom();
                break;
            default:
                JoinPublicRoom();
                break;
        }
    }
    #endregion

    #region Join Matches
    public void CreateHostRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (GameInformation.roomName.Trim() != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(GameInformation.roomName.Trim(), roomOptions, TypedLobby.Default);
        }
        else
        {
            // TODO: ADD ERROR MESSAGE 
        }
    }

    public void JoinPublicRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("StandardRoom", roomOptions, TypedLobby.Default);
    }

    public void JoinPrivateRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (GameInformation.roomName.Trim() != "")
        {
            PhotonNetwork.JoinRoom(GameInformation.roomName.Trim());
        }
        else
        {
            // TODO: ADD ERROR MESSAGE 
        }
    }

    public override void OnCreatedRoom()
    {
        statusText.text = "Waiting for opponent to join";
        Debug.Log("Waiting for opponent to join");
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        Debug.Log("Successfully Joined Room");
    }
    #endregion

    #region Failure Functions

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Joined Room Failed");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server because: " + cause.ToString());
    }

    #endregion
}