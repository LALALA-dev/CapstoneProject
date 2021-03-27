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
    public Text currentPlayerMessage;
    public GameObject panel;

    private bool gameInReview;

    void Start()
    {
        panel.gameObject.SetActive(false);
        gameInReview = false;
    }

    private void Update()
    {
        if (GameInformation.gameOver && !gameInReview) {
            gameInReview = true;
            GameInformation.firstPlayComplete = true;
            EnableWinPanel();
        }
    }

    public void EnableWinPanel()
    {
        if (GameInformation.playerOneScore > GameInformation.playerTwoScore && GameInformation.playerOneScore >= 10)
        {
            currentPlayerMessage.text = winnerText.text = "Player One Wins!";
        }
        else if (GameInformation.playerTwoScore > GameInformation.playerOneScore && GameInformation.playerTwoScore >= 10)
        {
            currentPlayerMessage.text = winnerText.text = "Player Two Wins!";
        }

        playerOneScore.text = GameInformation.playerOneScore.ToString();
        playerTwoScore.text = GameInformation.playerTwoScore.ToString();
        panel.gameObject.SetActive(true);
    }

    public void OnCancelClick()
    {
        panel.gameObject.SetActive(false);
    }
}
