using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISelectionManager : MonoBehaviour
{

    public GameObject selector;

    public GameObject[] avatars;

    public void OnHostSelect()
    {
        GameInformation.playerIsHost = true;
        GameInformation.currentPlayer = "HUMAN";

        SceneLoader.LoadLocalGameScene();
    }

    public void OnAIHostSelect()
    {
        GameInformation.playerIsHost = false;
        GameInformation.currentPlayer = "AI";

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
        GameInformation.playerAvatar = "HAT";
    }

    public void OnShipSelect()
    {
        GameInformation.playerAvatar = "BATTLESHIP";
    }

    public void OnCarSelect()
    {
        GameInformation.playerAvatar = "CAR";
    }

    public void OnThimbleSelect()
    {
        GameInformation.playerAvatar = "THIMBLE";
    }

    public void OnWheelBarrelSelect()
    {
        GameInformation.playerAvatar = "WHEELBARREL";
    }
}
