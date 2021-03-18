using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{

    public GameObject toggleButton;
    public Image toggleBackground;
    public Sprite playerOneColor;
    public Sprite playerTwoColor;


    void Awake()
    {
        toggleBackground.sprite = playerOneColor;
    }

    public void OnToggle()
    {
        if (toggleBackground.sprite == playerOneColor)
        {
            toggleButton.transform.position = new Vector3(toggleButton.transform.position.x + 112.0f, toggleButton.transform.position.y);
            toggleBackground.sprite = playerTwoColor;
            GameInformation.playerIsHost = false;
            GameInformation.currentPlayer = "AI";
        }
        else
        {
            toggleButton.transform.position = new Vector3(toggleButton.transform.position.x - 112.0f, toggleButton.transform.position.y);
            toggleBackground.sprite = playerOneColor;
            GameInformation.playerIsHost = true;
            GameInformation.currentPlayer = "HUMAN";
        }
    }
}
