using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkUIController : MonoBehaviour
{
    public GameObject avatarSelector;
    public GameObject gameTypeSelector;

    public GameObject[] avatars;
    public GameObject[] gameTypes;

    void Start()
    {
        GameInformation.gameType = 'N';
    }

    public void LeaveOnlineRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
    }

    public void OnHatSelect()
    {
        GameInformation.ownAvatar = "HAT";
        avatarSelector.transform.position = new Vector3(avatars[0].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnShipSelect()
    {
        GameInformation.ownAvatar = "BATTLESHIP";
        avatarSelector.transform.position = new Vector3(avatars[1].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnCarSelect()
    {
        GameInformation.ownAvatar = "CAR";
        avatarSelector.transform.position = new Vector3(avatars[2].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnThimbleSelect()
    {
        GameInformation.ownAvatar = "THIMBLE";
        avatarSelector.transform.position = new Vector3(avatars[3].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnWheelBarrelSelect()
    {
        GameInformation.ownAvatar = "WHEELBARREL";
        avatarSelector.transform.position = new Vector3(avatars[4].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnHostSelect()
    {
        GameInformation.networkGameType = NetworkGameType.Host;
        gameTypeSelector.transform.position = new Vector3(gameTypeSelector.transform.position.x, gameTypes[0].transform.position.y);
    }

    public void OnPublicSelect()
    {
        GameInformation.networkGameType = NetworkGameType.Public;
        gameTypeSelector.transform.position = new Vector3(gameTypeSelector.transform.position.x, gameTypes[1].transform.position.y);
    }

    public void OnPrivateSelect()
    {
        GameInformation.networkGameType = NetworkGameType.Private;
        gameTypeSelector.transform.position = new Vector3(gameTypeSelector.transform.position.x, gameTypes[2].transform.position.y);
    }
}
