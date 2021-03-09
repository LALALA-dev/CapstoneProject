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

    public void SendMove(string boardConfig)
    {
        if (pView.IsMine)
            pView.RPC("RPC_SendMove", RpcTarget.AllBuffered, boardConfig);
    }

    public void InvokeHostConfiguration()
    {
        if (pView.IsMine)
            pView.RPC("RPC_InvokeHostBoardRender", RpcTarget.AllBuffered);
    }

    public void InvokeEnableTriggers()
    {
        if (pView.IsMine)
            pView.RPC("RPC_InvokeHostBoardRender", RpcTarget.Others);
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
    void RPC_SendMove(string boardConfig)
    {
        Debug.Log("RPC_SendMove() was called");
        networkController.SetMove(boardConfig);
    }

    [PunRPC]
    void RPC_InvokeHostBoardRender()
    {
        Debug.Log("RPC_InvokeHostBoardRender() was called");
        networkController.InvokeRenderHost();
    }

    [PunRPC]
    void RPC_InvokeOpponentTriggerToggle()
    {
        Debug.Log("RPC_InvokeOpponentTriggerToggle() was called");
        networkController.InvokeTriggerToggle();
    }
    #endregion

}
