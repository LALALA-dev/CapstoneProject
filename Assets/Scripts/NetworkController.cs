using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI messageLog;
    public TMP_InputField inputField;

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
            CreateOrJoinRoom();
        }
        else
        {
            Debug.Log("Connecting to server...");
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }

    public void CreateOrJoinRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (inputField.text.Trim() != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            Photon.Pun.PhotonNetwork.JoinOrCreateRoom(inputField.text.Trim(), roomOptions, TypedLobby.Default);
        }
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
