using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : IMatchmakingCallbacks
{
    private LoadBalancingClient loadBalancingClient;

    private void QuickMatch()
    {
        loadBalancingClient.OpJoinRandomOrCreateRoom(null, null); ;
    }

    // do not forget to register callbacks via loadBalancingClient.AddCallbackTarget
    // also deregister via loadBalancingClient.RemoveCallbackTarget

    void IMatchmakingCallbacks.OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        throw new System.NotImplementedException();
    }

    public void OnCreatedRoom()
    {
        Debug.Log("Created Room");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnLeftRoom()
    {
        throw new System.NotImplementedException();
    }
}
