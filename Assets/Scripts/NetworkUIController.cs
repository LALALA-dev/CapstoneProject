using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkUIController : MonoBehaviour
{
    // public TMP_InputField privateRoomNameField;
    // public TextMeshProUGUI errorMessage;

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

    //public void SetRoomName()
    //{
    //    if (hostCreateRoomNameField.IsActive())
    //    {
    //        if(hostCreateRoomNameField.text.Trim() != "")
    //        {
    //            GameInformation.roomName = hostCreateRoomNameField.text.Trim();
    //            GameInformation.networkGameType = NetworkGameType.Host;
    //            SceneLoader.LoadNetworkLobbyScene();
    //        }
    //        else
    //        {
    //            errorMessage.text = "Please Enter a Room Name";
    //        }
    //    }
    //    else if(privateRoomNameField.IsActive())
    //    {
    //        if (privateRoomNameField.text.Trim() != "")
    //        {
    //            GameInformation.roomName = privateRoomNameField.text.Trim();
    //            GameInformation.networkGameType = NetworkGameType.Private;
    //            SceneLoader.LoadNetworkLobbyScene();
    //        }
    //        else
    //        {
    //            errorMessage.text = "Please Enter a Room Name";
    //        }
    //    }
    //    else
    //    {
    //        GameInformation.networkGameType = NetworkGameType.Public;
    //    }
    //}

    public void OnHatSelect()
    {
        GameInformation.playerOneAvatar = "HAT";
        avatarSelector.transform.position = new Vector3(avatars[0].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnShipSelect()
    {
        GameInformation.playerOneAvatar = "BATTLESHIP";
        avatarSelector.transform.position = new Vector3(avatars[1].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnCarSelect()
    {
        GameInformation.playerOneAvatar = "CAR";
        avatarSelector.transform.position = new Vector3(avatars[2].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnThimbleSelect()
    {
        GameInformation.playerOneAvatar = "THIMBLE";
        avatarSelector.transform.position = new Vector3(avatars[3].transform.position.x, avatarSelector.transform.position.y);
    }

    public void OnWheelBarrelSelect()
    {
        GameInformation.playerOneAvatar = "WHEELBARREL";
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
