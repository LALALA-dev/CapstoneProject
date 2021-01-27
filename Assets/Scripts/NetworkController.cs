using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI messageLog;

    void Start()
    {
        Connect();
    }

    // Update is called once per frame

    public override void OnConnectedToMaster()
    {
        messageLog.text = "Connected to Master Server";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        messageLog.text = "Disconnected from server because: " + cause.ToString();
    }

    void Update()
    {

    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            messageLog.text = "Connecting to server...";
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }
}
