using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public TMP_InputField hostCreateRoomNameField;
    public TMP_InputField privateRoomNameField;

    public TMP_InputField nameInputField;

    public static NetworkController NetController;
    public static string netOpponentsName;
    public static string networkPlayerName = "Player";

    #region Set Up
    private void Awake()
    {
        if (NetworkPlayer.player != null)
        {
            Destroy(NetworkPlayer.player);
        }

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

    #endregion

    #region Join Matches
    public void CreateHostRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }


        if (hostCreateRoomNameField.text.Trim() != "")
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 2;
            PhotonNetwork.CreateRoom(hostCreateRoomNameField.text.Trim(), roomOptions, TypedLobby.Default);
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

        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        if (privateRoomNameField.text.Trim() != "")
        {
            PhotonNetwork.JoinRoom(privateRoomNameField.text.Trim());
        }
        else
        {
            // TODO: ADD ERROR MESSAGE 
        }
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        SetNetworkPlayerName();
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
