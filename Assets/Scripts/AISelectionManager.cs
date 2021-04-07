using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AISelectionManager : MonoBehaviour
{
    public GameObject selector;

    public GameObject[] avatars;
    public AudioSource button;
    public Button hnpButton;

    private void Start()
    {
        GameInformation.ownAvatar = "CAR";
        GameInformation.playerOneAvatar = "CAR";
        GameInformation.playerTwoAvatar = "WHEELBARREL";

        if (!GameInformation.aiCompetition)
            hnpButton.gameObject.SetActive(false);
    }

    public void OnGoSelect()
    {
        // Assign a random avatar to the AI that is not selected by player.
        int aiAvatarID;
        do
        {
            aiAvatarID = UnityEngine.Random.Range(0, GameInformation.avatarNames.Length);
        } while (aiAvatarID == GetPlayerTokenID());
        
        GameInformation.aiAvatar = GameInformation.avatarNames[aiAvatarID];

        SceneLoader.LoadLocalGameScene();
    }

    public void OnHNPSelect()
    {
        GameInformation.playerIsHost = true;
        GameInformation.currentPlayer = "HUMAN";
        GameInformation.HumanNetworkProtocol = true;
        GameInformation.gameType = 'P';
        SceneLoader.LoadLocalGameScene();
    }

    public void OnHatSelect()
    {
        GameInformation.ownAvatar = "HAT";
        selector.transform.position = new Vector3(avatars[0].transform.position.x, selector.transform.position.y);
    }

    public void OnShipSelect()
    {
        GameInformation.ownAvatar = "BATTLESHIP";
        selector.transform.position = new Vector3(avatars[1].transform.position.x, selector.transform.position.y);
    }

    public void OnCarSelect()
    {
        GameInformation.ownAvatar = "CAR";
        selector.transform.position = new Vector3(avatars[2].transform.position.x, selector.transform.position.y);
    }

    public void OnThimbleSelect()
    {
        GameInformation.ownAvatar = "THIMBLE";
        selector.transform.position = new Vector3(avatars[3].transform.position.x, selector.transform.position.y);
    }

    public void OnWheelBarrelSelect()
    {
        GameInformation.ownAvatar = "WHEELBARREL";
        selector.transform.position = new Vector3(avatars[4].transform.position.x, selector.transform.position.y);
    }

    public void OnButtonClick()
    {
        button.Play();
    }

    private int GetPlayerTokenID()
    {
        int id;
        switch (GameInformation.ownAvatar)
        {
            case "HAT":
                id = 0;
                break;
            case "BATTLESHIP":
                id = 1;
                break;
            case "CAR":
                id = 2;
                break;
            case "THIMBLE":
                id = 3;
                break;
            case "WHEELBARREL":
                id = 4;
                break;
            default:
                id = 2;
                break;
        }
        return id;
    }
}
