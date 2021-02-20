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
            pView.RPC("RPC_SendMove", RpcTarget.All, boardConfig);
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
        if (!photonView.IsMine)
            return;
        networkController.SetMove(boardConfig);
    }
    #endregion

}
