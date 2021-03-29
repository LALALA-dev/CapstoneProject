using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinController : MonoBehaviour
{
    public TMP_Text winnerText;
    public TMP_Text winnerScoreText;
    public TMP_Text loserScoreText;
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
            GameInformation.tutorialNeeded = true;
            EnableWinPanel();
        }
    }

    public void EnableWinPanel()
    {
        if (GameInformation.playerOneScore > GameInformation.playerTwoScore && GameInformation.playerOneScore >= 10)
        {
            currentPlayerMessage.text = winnerText.text = "Player One Wins!";
            winnerScoreText.text = "Player One: " + GameInformation.playerOneScore.ToString();
            loserScoreText.text = "Player Two: " + GameInformation.playerTwoScore.ToString();
        }
        else if (GameInformation.playerTwoScore > GameInformation.playerOneScore && GameInformation.playerTwoScore >= 10)
        {
            currentPlayerMessage.text = winnerText.text = "Player Two Wins!";
            winnerScoreText.text = "Player Two: " + GameInformation.playerTwoScore.ToString();
            loserScoreText.text = "Player One: " + GameInformation.playerOneScore.ToString();
        }
        panel.gameObject.SetActive(true);
    }

    public void OnCancelClick()
    {
        panel.gameObject.SetActive(false);
    }
}
