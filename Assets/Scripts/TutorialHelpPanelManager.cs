using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameObjectProperties;

public class TutorialHelpPanelManager : MonoBehaviour
{
    private bool isHelpPanelsActive;
    private int currentPanel;
    // The order in which the help panels should be displayed.
    //  Changes as the game advances. 
    private int[] panelOrderKey;
    public GameObject[] helpPanels;
    public TMP_Text[] tokenScoresText;
    public TMP_Text[] propertyScoresText;
    public TMP_Text[] longestRoadsText;
    public TMP_Text[] totalScoresText;
    public GameObject nextButton;
    public GameObject previousButton;
    public GameObject cancelButton;
    public GameObject tradeButton;
    public GameObject goButton;

    // Start is called before the first frame update
    void Start()
    {
        isHelpPanelsActive = false;
        panelOrderKey = new int[helpPanels.Length];

        panelOrderKey[0] = 0;
        panelOrderKey[1] = 1;
        panelOrderKey[2] = -1;
        panelOrderKey[3] = -1;
    }

    void Update()
    {
        // Deactivate the go and trade buttons and gameboard if it's not your turn and the buttons are interactable.
        if (!isCurrentPlayer() && buttonsInteractable())
        {
            FlipTradeAndGoInteraction();
        }
        // Activate the go and trade buttons and gameboard if it's your turn, the buttons are not interactable, and the help panel is closed. 
        else if (isCurrentPlayer() && !buttonsInteractable() && !isHelpPanelsActive)
        {
            FlipTradeAndGoInteraction();
        }
        // Deactivate the go and trade buttons and gameboard if it's your turn, the buttons are interactable, and the help panel is open. 
        else if (isCurrentPlayer() && buttonsInteractable() && isHelpPanelsActive)
        {
            FlipTradeAndGoInteraction();
        }
    }

    public void onHelpClick()
    {
        if (isHelpPanelsActive)
        {
            onCancelHelp();
        }
        else
        {
            isHelpPanelsActive = true;
            currentPanel = 0;
            // Activate the first help panel that should be shown.
            helpPanels[panelOrderKey[currentPanel]].SetActive(true);

            nextButton.SetActive(true);
            cancelButton.SetActive(true);
        }
    }

    public void onNextHelp()
    {
        helpPanels[panelOrderKey[currentPanel]].SetActive(false);
        helpPanels[panelOrderKey[++currentPanel]].SetActive(true);

        previousButton.SetActive(true);

        if ((currentPanel + 1) == panelOrderKey.Length || panelOrderKey[currentPanel + 1] == -1)
        {
            nextButton.SetActive(false);
        }
    }

    public void onPreviousHelp()
    {
        helpPanels[panelOrderKey[currentPanel]].SetActive(false);
        helpPanels[panelOrderKey[--currentPanel]].SetActive(true);

        nextButton.SetActive(true);

        if (currentPanel == 0)
        {
            previousButton.SetActive(false);
        }
    }

    public void onCancelHelp()
    {
        isHelpPanelsActive = false;
        helpPanels[panelOrderKey[currentPanel]].SetActive(false);
        nextButton.SetActive(false);
        previousButton.SetActive(false);
        cancelButton.SetActive(false);
    }

    public void SetPanelOrderTurnFive()
    {
        if (!isHelpPanelsActive)
        {
            panelOrderKey[0] = 1;
            panelOrderKey[1] = 2;
            panelOrderKey[2] = 3;
            panelOrderKey[3] = -1;
        }
    }

    public void SetPanelOrderTurnNine()
    {
        if (!isHelpPanelsActive)
        {
            panelOrderKey[0] = 3;
            panelOrderKey[1] = 1;
            panelOrderKey[2] = 2;
            panelOrderKey[3] = -1;
        }
    }

    public void UpdateHelpPopupScores()
    {
        // Tokens
        tokenScoresText[0].text = GameInformation.playerOneNodes.ToString();
        tokenScoresText[1].text = GameInformation.playerTwoNodes.ToString();

        // Monopolized Properties
        propertyScoresText[0].text = GameInformation.playerOneProperties.ToString();
        propertyScoresText[1].text = GameInformation.playerTwoProperties.ToString();

        // Longest Road
        longestRoadsText[0].text = GameInformation.playerOneNetwork.ToString();
        longestRoadsText[1].text = GameInformation.playerTwoNetwork.ToString();

        // Total Scores
        totalScoresText[0].text = GameInformation.playerOneScore.ToString();
        totalScoresText[1].text = GameInformation.playerTwoScore.ToString();
    }

    private void FlipTradeAndGoInteraction()
    {
        if (!GameInformation.gameOver)
        {
            SendMessageUpwards("ToogleTriggers");
        }
    }

    private bool isCurrentPlayer()
    {
        char gameType = GameInformation.gameType;

        // Local Game
        if (gameType == 'A' || gameType == 'E' || gameType == 'P')
        {
            return GameInformation.currentPlayer == "HUMAN";
        }
        // Network Game
        if (gameType == 'N')
        {
            return (GameInformation.currentPlayer == "HOST" && GameInformation.playerIsHost) || (GameInformation.currentPlayer == "CLIENT" && !GameInformation.playerIsHost);
        }
        // Reaching this statement is an error; there are only four possibilities for GameInformation.gameType.
        return false;
    }

    private bool buttonsInteractable()
    {
        return goButton.GetComponent<Button>().interactable && tradeButton.GetComponent<Button>().interactable;
    }
}
