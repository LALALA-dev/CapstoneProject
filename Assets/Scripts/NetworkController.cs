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
        messageLog.text = "Connecting to server...";
        PhotonNetwork.ConnectUsingSettings();
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
}
