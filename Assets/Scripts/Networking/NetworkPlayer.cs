using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{

    [SerializeField] private NetworkController networkController;
    private PhotonView pView;

    public static NetworkPlayer player;

    void Awake()
    {
        pView = GetComponent<PhotonView>();

        if (player == null)
            player = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void SendInfo(string name)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendInfo", RpcTarget.All, name);
    }

    public void SendOpeningBoardConfiguration(string openingBoard)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendOpeningBoard", RpcTarget.AllBuffered, openingBoard);
    }

    public void SendMove(string boardConfig)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendMove", RpcTarget.Others, boardConfig);
    }

    public void SendTurnNumber(int turn)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendTurn", RpcTarget.All, turn);
    }

    public void SendResources(string resources)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendResources", RpcTarget.All, resources);
    }

    public void SendCurrentPlayersResources(string resources)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendCurrentPlayerResources", RpcTarget.Others, resources);
    }

    public void SendCurrentPlayer(string player)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendCurrentPlayer", RpcTarget.All, player);
    }

    public void InvokeHostConfiguration()
    {
        if (pView.IsMine)
            pView.RPC("RPC_InvokeHostBoardRender", RpcTarget.AllBuffered);
    }

    #region RPC Functions
    [PunRPC]
    void RPC_SendInfo(string playerName)
    {
        Debug.Log("RPC_SendInfo() was called with playName = " + playerName);
        if (!photonView.IsMine)
            return;
        networkController.GetOpponentInfo(playerName);
    }

    [PunRPC]
    void RPC_SendOpeningBoard(string boardConfig)
    {
        Debug.Log("RPC_SendOpeningBoard() was called");
        networkController.SetOpeningBoardConfiguration(boardConfig);
    }

    [PunRPC]
    void RPC_SendMove(string boardConfig)
    {
        Debug.Log("RPC_SendMove() was called");
        networkController.SetMove(boardConfig);
    }

    [PunRPC]
    void RPC_SendTurn(int turn)
    {
        Debug.Log("RPC_SendTurn() was called");
        networkController.SetTurnNumber(turn);
    }

    [PunRPC]
    void RPC_SendResources(string resources)
    {
        Debug.Log("RPC_SendResources() was called");
        networkController.SetOpponentResources(resources);
    }

    [PunRPC]
    void RPC_SendCurrentPlayerResources(string resources)
    {
        Debug.Log("RPC_SendCurrentPlayerResources() was called");
        networkController.SetCurrentPlayersResources(resources);
    }

    [PunRPC]
    void RPC_SendCurrentPlayer(string player)
    {
        Debug.Log("RPC_SendCurrentPlayer() was called");
        networkController.SetCurrentPlayer(player);
    }

    [PunRPC]
    void RPC_InvokeHostBoardRender()
    {
        Debug.Log("RPC_InvokeHostBoardRender() was called");
        networkController.InvokeRenderHost();
    }

    #endregion

}
