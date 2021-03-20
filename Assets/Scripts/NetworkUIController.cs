using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkUIController : MonoBehaviour
{
    public TMP_InputField hostCreateRoomNameField;
    public TMP_InputField privateRoomNameField;
    public TMP_InputField setNameField;
    public TextMeshProUGUI errorMessage;

    void Start()
    {
        if(PlayerPrefs.HasKey("NetworkName"))
        {
            setNameField.text = PlayerPrefs.GetString("NetworkName");
        }
        hostCreateRoomNameField.gameObject.SetActive(false);
        privateRoomNameField.gameObject.SetActive(false);
        GameInformation.gameType = 'N';
    }

    public void SetName()
    {
        if (setNameField.text.Trim() != "")
        {
            PlayerPrefs.SetString("NetworkName", setNameField.text.Trim());
        }
        else
        {
        }
    }

    public void EnableCreateHostGameInput()
    {
        hostCreateRoomNameField.gameObject.SetActive(true);
        privateRoomNameField.gameObject.SetActive(false);
    }

    public void EnableJoinPrivateGameInput()
    {
        hostCreateRoomNameField.gameObject.SetActive(false);
        privateRoomNameField.gameObject.SetActive(true);
    }

    public void DisableInputs()
    {
        SetRoomName();
        if (hostCreateRoomNameField.text.Trim() != "")
        {
            hostCreateRoomNameField.gameObject.SetActive(false);
            privateRoomNameField.gameObject.SetActive(false);
            SceneLoader.LoadNetworkLobbyScene();
        }
        else
        {
            //SceneLoader.LoadNetworkScene();
        }
    }

    public static void OtherPlayerDisconnected()
    {
        SceneLoader.LoadNetworkLobbyScene();
        //errorMessage.text = "Other player left room";
    }

    public void SetRoomName()
    {
        if (hostCreateRoomNameField.IsActive())
        {
            if(hostCreateRoomNameField.text.Trim() != "")
            {
                GameInformation.roomName = hostCreateRoomNameField.text.Trim();
                GameInformation.networkGameType = NetworkGameType.Host;
                SceneLoader.LoadNetworkLobbyScene();
            }
            else
            {
                errorMessage.text = "Please Enter a Room Name";
            }
        }
        else if(privateRoomNameField.IsActive())
        {
            if (privateRoomNameField.text.Trim() != "")
            {
                GameInformation.roomName = privateRoomNameField.text.Trim();
                GameInformation.networkGameType = NetworkGameType.Private;
                SceneLoader.LoadNetworkLobbyScene();
            }
            else
            {
                errorMessage.text = "Please Enter a Room Name";
            }
        }
        else
        {
            GameInformation.networkGameType = NetworkGameType.Public;
        }
    }
}
