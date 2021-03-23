using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinController : MonoBehaviour
{
    public TMP_Text winnerText;
    public TMP_Text playerOneScore;
    public TMP_Text playerTwoScore;
    public GameObject panel;

    void Start()
    {
        panel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameInformation.gameOver)
            EnableWinPanel();
    }

    public void EnableWinPanel()
    {
        if (GameInformation.playerOneScore > GameInformation.playerTwoScore && GameInformation.playerOneScore >= 10)
        {
            winnerText.text = "Player One Wins!";
        }
        else if (GameInformation.playerTwoScore > GameInformation.playerOneScore && GameInformation.playerTwoScore >= 10)
        {
            winnerText.text = "Player Two Wins!";
        }

        playerOneScore.text = GameInformation.playerOneScore.ToString();
        playerTwoScore.text = GameInformation.playerTwoScore.ToString();
        panel.gameObject.SetActive(true);
    }

    public void OnCancelClick()
    {
        GameInformation.gameOver = false;
        panel.gameObject.SetActive(false);
    }
}
