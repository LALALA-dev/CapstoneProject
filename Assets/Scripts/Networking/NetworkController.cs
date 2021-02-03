using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomInputField;
    public TMP_InputField nameInputField;

    public static NetworkController NetController;
    public static string netOpponentsName;
    private string networkPlayerName = "default";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        NetController = this;
    }

    private void Start()
    {
        Connect();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server because: " + cause.ToString());
    }

    public void Connect()
    {
        if(PhotonNetwork.IsConnected)
        {
            CreateOrJoinRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }

    public void CreateOrJoinRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (roomInputField.text.Trim() != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            Photon.Pun.PhotonNetwork.JoinOrCreateRoom(roomInputField.text.Trim(), roomOptions, TypedLobby.Default);
        }
    }

    public override void OnCreatedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        setNetworkPlayerName();
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        setNetworkPlayerName();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Joined Room Failed");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed");
    }

    public void GetOpponentInfo(string name)
    {
        netOpponentsName = name;
        Debug.Log("Your opponent's name is " + netOpponentsName + "!");
    }

    private void setNetworkPlayerName()
    {
        NetworkPlayer.player.name = networkPlayerName;
        NetworkPlayer.player.SendPlayerInfo(networkPlayerName);
    }

    public void setName()
    {
        if (nameInputField.text.Trim() != "")
        {
            networkPlayerName = nameInputField.text.Trim();
        }
    }
}
