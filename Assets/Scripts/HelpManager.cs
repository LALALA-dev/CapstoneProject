using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    private bool isHelpPanelsActive;
    private int currentPanel;
    // The order in which the help panels should be displayed.
    //  Changes as the game advances. 
    private int[] panelOrderKey;
    public GameObject[] helpPanels;
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

        for (int i = 0; i < helpPanels.Length; ++i)
        {
            panelOrderKey[i] = i;
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
            
            if (panelOrderKey[1] != null)
            {
                nextButton.SetActive(true);
            }
            cancelButton.SetActive(true);
            FlipTradeAndGoInteraction();
        }
    }

    public void onNextHelp()
    {
        helpPanels[panelOrderKey[currentPanel]].SetActive(false);
        helpPanels[panelOrderKey[++currentPanel]].SetActive(true);

        previousButton.SetActive(true);

        if ((currentPanel + 1) == panelOrderKey.Length || panelOrderKey[currentPanel + 1] == null)
        {
            nextButton.SetActive(false);
        }
    }

    public void onPreviousHelp()
    {
        helpPanels[currentPanel].SetActive(false);
        helpPanels[--currentPanel].SetActive(true);

        nextButton.SetActive(true);

        if (currentPanel == 0)
        {
            previousButton.SetActive(false);
        }
    }

    public void onCancelHelp()
    {
        isHelpPanelsActive = false;
        helpPanels[currentPanel].SetActive(false);
        nextButton.SetActive(false);
        previousButton.SetActive(false);
        cancelButton.SetActive(false);
        FlipTradeAndGoInteraction();
    }

    // TODO: Set up to change the ordering of the panels based on current turn.
    //  Example:
    //      0 and 1 for opening moves.
    //      1, 2, and 3 for moves 4-6|8.
    //      3, 1, and 2 for moves > 6|8.
    public void changePanelOrder()
    {

    }

    private void FlipTradeAndGoInteraction()
    {
        SendMessageUpwards("ToogleTriggers");
    }
}
