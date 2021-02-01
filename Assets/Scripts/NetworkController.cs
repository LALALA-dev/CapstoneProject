using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI onlineStatusText;
    public TextMeshProUGUI currentRoomText;
    public TMP_Dropdown roomsDropdown;
    public TMP_InputField inputField;

    private void Start()
    {
        roomsDropdown.ClearOptions();
        Connect();
    }

    public override void OnConnectedToMaster()
    {
        onlineStatusText.text = "Online";

        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        onlineStatusText.text = "Disconnected from server because: " + cause.ToString();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        List<string> roomNames = new List<string>();

        if (roomList.Count > 0)
        {
            foreach (RoomInfo roomInfo in roomList)
            {
                roomNames.Add(roomInfo.Name);
            }
        }
        else
        {
            roomNames.Add("No Online Matches");
        }

        roomsDropdown.ClearOptions();
        roomsDropdown.AddOptions(roomNames);
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
        currentRoomText.text = "Created Room!";
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
