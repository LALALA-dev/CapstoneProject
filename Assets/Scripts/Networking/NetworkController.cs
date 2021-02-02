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
    public TMP_InputField roomInputField;
    public TMP_InputField nameInputField;

    public static NetworkController NetController;
    public static string netOpponentsName;


    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        NetController = this;
    }

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
        currentRoomText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
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

    public void setPlayerName()
    {
        if(nameInputField.text.Trim() != "")
        {
            NetworkPlayer.player.name = nameInputField.text.Trim();
            NetworkPlayer.player.SendPlayerInfo(NetworkPlayer.player.name);
        }
    }
}
