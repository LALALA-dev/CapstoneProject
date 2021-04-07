using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{

    public GameObject toggleButton;
    public Image toggleBackground;
    public Sprite optionOne;
    public Sprite optionTwo;


    void Awake()
    {
        toggleBackground.sprite = optionOne;
    }

    public void OnPlayerToggle()
    {
        if (toggleBackground.sprite == optionOne)
        {
            toggleButton.transform.position = new Vector3(toggleButton.transform.position.x + 112.0f, toggleButton.transform.position.y);
            toggleBackground.sprite = optionTwo;
            GameInformation.playerIsHost = false;
            GameInformation.currentPlayer = "AI";
        }
        else
        {
            toggleButton.transform.position = new Vector3(toggleButton.transform.position.x - 112.0f, toggleButton.transform.position.y);
            toggleBackground.sprite = optionOne;
            GameInformation.playerIsHost = true;
            GameInformation.currentPlayer = "HUMAN";
        }
    }

    public void OnAIToggle()
    {
        if (toggleBackground.sprite == optionOne)
        {
            toggleButton.transform.position = new Vector3(toggleButton.transform.position.x + 112.0f, toggleButton.transform.position.y);
            toggleBackground.sprite = optionTwo;
            GameInformation.gameType = 'E';
        }
        else
        {
            toggleButton.transform.position = new Vector3(toggleButton.transform.position.x - 112.0f, toggleButton.transform.position.y);
            toggleBackground.sprite = optionOne;
            GameInformation.gameType = 'A';
        }
    }
}
