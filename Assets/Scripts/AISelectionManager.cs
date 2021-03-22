using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISelectionManager : MonoBehaviour
{

    public GameObject selector;

    public GameObject[] avatars;

    public void OnGoSelect()
    {
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
        GameInformation.playerOneAvatar = "HAT";
        selector.transform.position = new Vector3(avatars[0].transform.position.x, selector.transform.position.y);
    }

    public void OnShipSelect()
    {
        GameInformation.playerOneAvatar = "BATTLESHIP";
        selector.transform.position = new Vector3(avatars[1].transform.position.x, selector.transform.position.y);
    }

    public void OnCarSelect()
    {
        GameInformation.playerOneAvatar = "CAR";
        selector.transform.position = new Vector3(avatars[2].transform.position.x, selector.transform.position.y);
    }

    public void OnThimbleSelect()
    {
        GameInformation.playerOneAvatar = "THIMBLE";
        selector.transform.position = new Vector3(avatars[3].transform.position.x, selector.transform.position.y);
    }

    public void OnWheelBarrelSelect()
    {
        GameInformation.playerOneAvatar = "WHEELBARREL";
        selector.transform.position = new Vector3(avatars[4].transform.position.x, selector.transform.position.y);
    }
}
