using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkUIController : MonoBehaviour
{
    public TMP_InputField hostCreateRoomNameField;
    public TMP_InputField privateRoomNameField;
    public TMP_InputField setNameField;

    void Start()
    {
        if(PlayerPrefs.HasKey("NetworkName"))
        {
            setNameField.text = PlayerPrefs.GetString("NetworkName");
            NetworkController.networkPlayerName = PlayerPrefs.GetString("NetworkName");
        }
        hostCreateRoomNameField.gameObject.SetActive(false);
        privateRoomNameField.gameObject.SetActive(false);
    }

    public void SetName()
    {
        if (setNameField.text.Trim() != "")
        {
            NetworkController.networkPlayerName = setNameField.text.Trim();
            PlayerPrefs.SetString("NetworkName", setNameField.text.Trim());
        }
        else
        {
            setNameField.text = NetworkController.networkPlayerName;
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
        hostCreateRoomNameField.gameObject.SetActive(false);
        privateRoomNameField.gameObject.SetActive(false);
    }
}
