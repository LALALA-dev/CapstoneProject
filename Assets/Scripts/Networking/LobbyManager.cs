using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject CancelButton;
    public Image connectMessage;
    public Image pinMessage;
    public Image waitingForClientMessage;
    public Image enterPINMessage;
    public Image waitingForHostMessage;
    public Image readyToBeginMessage;

    public GameObject gamePINInputBtn;
    public GameObject startGameBtn;

    public GameObject generalError;
    public GameObject playerLeftError;

    public Text HostPIN;

    public TMP_InputField privateRoomNameField;

    private bool roomReady = false;

    public GameObject waitingAnimation;
    public AudioSource whistle;
    public AudioSource button;

    public void OnButtonClick()
    {
        button.Play();
    }

    #region Set Up
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        pinMessage.gameObject.SetActive(false);
        waitingForClientMessage.gameObject.SetActive(false);
        enterPINMessage.gameObject.SetActive(false);
        waitingForHostMessage.gameObject.SetActive(false);
        readyToBeginMessage.gameObject.SetActive(false);
        gamePINInputBtn.SetActive(false);
        privateRoomNameField.gameObject.SetActive(false);
        HostPIN.gameObject.SetActive(false);
        startGameBtn.gameObject.SetActive(false);
        playerLeftError.SetActive(false);
        generalError.SetActive(false);
    }

    private void Start()
    {
        connectMessage.gameObject.SetActive(true);
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

        switch (GameInformation.networkGameType)
        {
            case NetworkGameType.Public:
                JoinPublicRoom();
                break;
            case NetworkGameType.Private:
                GameInformation.playerTwoAvatar = GameInformation.ownAvatar;
                EnableInput();
                break;
            case NetworkGameType.Host:
                GameInformation.playerOneAvatar = GameInformation.ownAvatar;
                CreateHostRoom();
                break;
            default:
                JoinPublicRoom();
                break;
        }
    }

    public void EnableInput()
    {
        connectMessage.gameObject.SetActive(false);
        enterPINMessage.gameObject.SetActive(true);
        privateRoomNameField.gameObject.SetActive(true);
        CancelButton.SetActive(true);
        gamePINInputBtn.SetActive(true);
        waitingAnimation.SetActive(false);
    }

    public void SetRoomName()
    {
        if (privateRoomNameField.text.Trim() != "" && privateRoomNameField.text.Trim().Length == 4)
        {
            GameInformation.roomName = privateRoomNameField.text.Trim().Substring(0,4);
            CancelButton.SetActive(true);
        }
        JoinPrivateRoom();
    }

    #endregion

    private void Update()
    {
        if(PhotonNetwork.IsConnected && roomReady && PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            roomReady = false;
            playerLeftError.SetActive(true);
            readyToBeginMessage.gameObject.SetActive(false);
        }
    }

    #region Join Matches
    public void CreateHostRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        GameInformation.roomName = GenerateRandomGamePIN();
        GameInformation.playerIsHost = true;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        GameInformation.currentPlayer = "HOST";
        PhotonNetwork.CreateRoom(GameInformation.roomName, roomOptions, TypedLobby.Default);
        CancelButton.SetActive(false);
    }

    public string GenerateRandomGamePIN()
    {
        int min = 1000;
        int max = 9999;
        var rdm = new System.Random();
        return rdm.Next(min, max).ToString();
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
            GameInformation.playerIsHost = false;
            GameInformation.currentPlayer = "HOST";
            PhotonNetwork.JoinRoom(GameInformation.roomName.Trim());

            enterPINMessage.gameObject.SetActive(false);
            privateRoomNameField.gameObject.SetActive(false);
            gamePINInputBtn.SetActive(false);
            connectMessage.gameObject.SetActive(true);
            waitingAnimation.SetActive(false);
        }
        else
        {
            waitingAnimation.SetActive(false);
            generalError.SetActive(true);
            whistle.Play();
        }
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    public override void OnCreatedRoom()
    {
        connectMessage.gameObject.SetActive(false);
        waitingForClientMessage.gameObject.SetActive(true);

        if(GameInformation.networkGameType == NetworkGameType.Host)
        {
            pinMessage.gameObject.SetActive(true);
            HostPIN.gameObject.SetActive(true);
            HostPIN.text = GameInformation.roomName;
        }
            
        CancelButton.SetActive(true);
        GameInformation.currentPlayer = "HOST";
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            GameInformation.playerIsHost = false;
            GameInformation.playerOneAvatar = "CAR";
            GameInformation.playerTwoAvatar = "WHEELBARREL";

            connectMessage.gameObject.SetActive(false);
            waitingForHostMessage.gameObject.SetActive(true);
            CancelButton.SetActive(true);
            waitingAnimation.SetActive(true);
        }
        else
            GameInformation.playerIsHost = true;
    }

    public override void OnLeftRoom()
    {
        SceneLoader.LoadNetworkScene();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            GameInformation.currentPlayer = "HOST";
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                readyToBeginMessage.gameObject.SetActive(true);
                pinMessage.gameObject.SetActive(false);
                waitingForClientMessage.gameObject.SetActive(false);
                HostPIN.gameObject.SetActive(false);
                startGameBtn.SetActive(true);
                waitingAnimation.SetActive(false);
            }
        }
    }

    public void OnHostStartGameClick()
    {
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            playerLeftError.SetActive(true);
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;

            pinMessage.gameObject.SetActive(false);
            waitingForClientMessage.gameObject.SetActive(false);
            enterPINMessage.gameObject.SetActive(false);
            waitingForHostMessage.gameObject.SetActive(false);
            readyToBeginMessage.gameObject.SetActive(false);
            gamePINInputBtn.SetActive(false);
            privateRoomNameField.gameObject.SetActive(false);
            HostPIN.gameObject.SetActive(false);
            startGameBtn.gameObject.SetActive(false);
            generalError.SetActive(false);
            waitingAnimation.SetActive(false);
        }
    }

    #endregion

    #region Failure Functions

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        generalError.SetActive(true);
        whistle.Play();
        connectMessage.gameObject.SetActive(false);
        waitingAnimation.SetActive(false);
        Debug.Log("Joined Room Failed: " + message);
        Invoke("AutoNavigate", 3.0f);
        CancelButton.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        generalError.SetActive(true);
        whistle.Play();
        waitingAnimation.SetActive(false);
        connectMessage.gameObject.SetActive(false);
        Invoke("AutoNavigate", 3.0f);
        CancelButton.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        generalError.SetActive(true);
        waitingAnimation.SetActive(false);
        whistle.Play();
        connectMessage.gameObject.SetActive(false);
        CancelButton.SetActive(true);
        Invoke("AutoNavigate", 3.0f);
    }

    public void AutoNavigate()
    {
        SceneLoader.LoadNetworkScene();
    }

    #endregion
}