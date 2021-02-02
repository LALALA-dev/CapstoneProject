using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{

    [SerializeField] private NetworkController networkController;
    [SerializeField] private PhotonView photonView;

    public static NetworkPlayer player;
    public string playerName;

    void Start()
    {
        photonView = PhotonView.Get(this);

        if (player == null)
            player = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void SendPlayerInfo(string playerName)
    {
        if (photonView.IsMine)
            photonView.RPC("RPC_SendInfo", RpcTarget.AllBuffered, playerName);
    }

    public void SendPlayerMove(/*MoveObject Param*/)
    {
        if (photonView.IsMine)
            photonView.RPC("RPC_SendPlayerMove", RpcTarget.All /*, MoveObject param*/);
    }

    public void SendHostBoardConfiguration(/*BoardConfig Param*/)
    {
        if (photonView.IsMine)
            photonView.RPC("RPC_SendBoardConfig", RpcTarget.All /*, BoardConfig param*/);
    }

    #region RPC Functions
    [PunRPC]
    void RPC_SendInfo(string playerName)
    {
        Debug.Log("RPC_SendInfo() was called with playName = " + playerName);
        networkController.GetOpponentInfo(playerName);
    }

    [PunRPC]
    void RPC_SendMove(/*MoveObject Param*/)
    {
        Debug.Log("RPC_SendMove() was called");
        // TODO: SEND A MOVE
    }

    [PunRPC]
    void RPC_SendBoardConfig(/*BoardConfig Param*/)
    {
        Debug.Log("RPC_SendBoardConfig() was called");
        // TODO: SEND A BOARD CONFIGURATION (IF HOST == TRUE)
    }
    #endregion

}
