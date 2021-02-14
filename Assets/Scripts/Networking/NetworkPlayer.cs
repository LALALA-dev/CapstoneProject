using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{

    [SerializeField] private NetworkController networkController;
    private PhotonView pView;

    public static NetworkPlayer player;
    public string playerName;

    void Awake()
    {
        pView = GetComponent<PhotonView>();

        if (player == null)
            player = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void SendPlayerInfo(string playerName)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendInfo", RpcTarget.AllBuffered, playerName);
    }

    public void SendPlayerMove(/*MoveObject Param*/)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendPlayerMove", RpcTarget.All /*, MoveObject param*/);
    }

    public void SendHostBoardConfiguration(string boardConfig)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendBoardConfig", RpcTarget.AllBuffered, boardConfig);
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
    void RPC_SendBoardConfig(string boardConfig)
    {
        Debug.Log("RPC_SendBoardConfig() was called");
        if (!GameInformation.playerIsHost)
            networkController.SetClientBoardConfiguration(boardConfig);
    }
    #endregion

}
