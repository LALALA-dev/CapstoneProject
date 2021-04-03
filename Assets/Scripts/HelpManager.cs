using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameObjectProperties;

public class HelpManager : MonoBehaviour
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
            Debug.Log("First Panel Order Key: " + panelOrderKey[currentPanel]);
            helpPanels[panelOrderKey[currentPanel]].SetActive(true);
            
            nextButton.SetActive(true);
            cancelButton.SetActive(true);
            FlipTradeAndGoInteraction();
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
        FlipTradeAndGoInteraction();
    }

    // TODO: Set up to change the ordering of the panels based on current turn.
    //  Example:
    //      0, 1, 2, and 3 for opening moves.
    //      1, 2, 3, and 0 for moves 5-8.
    //      3, 1, 2, and 0 for moves > 8.
    public void SetPanelOrderTurnFive()
    {
        Debug.Log("Panel Order Turn Five");
        panelOrderKey[0] = 1;
        panelOrderKey[1] = 2;
        panelOrderKey[2] = 3;
        panelOrderKey[3] = -1;
    }

    public void SetPanelOrderTurnNine()
    {
        Debug.Log("Panel Order Turn Nine");
        panelOrderKey[0] = 3;
        panelOrderKey[1] = 1;
        panelOrderKey[2] = 2;
        panelOrderKey[3] = -1;
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
        SendMessageUpwards("ToogleTriggers");
    }
}
