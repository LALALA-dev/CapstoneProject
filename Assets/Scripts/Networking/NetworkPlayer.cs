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



}
