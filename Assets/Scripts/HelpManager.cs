using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : MonoBehaviour
{
    private int currentPanel;
    // The order in which the help panels should be displayed.
    //  Changes as the game advances. 
    private int[] panelOrderKey;
    public GameObject[] helpPanels;
    public GameObject nextButton;
    public GameObject previousButton;
    public GameObject cancelButton;

    // Start is called before the first frame update
    void Start()
    {
        panelOrderKey = new int[helpPanels.Length];

        for (int i = 0; i < helpPanels.Length; ++i)
        {
            panelOrderKey[i] = i;
        }
    }

    // TODO: Might also need to deactivate certain aspects of the board &| buttons.
    public void onHelpClick()
    {
        // Activate the first help panel that should be shown.
        helpPanels[panelOrderKey[0]].SetActive(true);
        currentPanel = 0;
        
        if (panelOrderKey[1] != null)
        {
            nextButton.SetActive(true);
        }
        cancelButton.SetActive(true);
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
        helpPanels[currentPanel].SetActive(false);
        nextButton.SetActive(false);
        previousButton.SetActive(false);
        cancelButton.SetActive(false);
    }

    // TODO: Set up to change the ordering of the panels based on current turn.
    //  Example:
    //      0 and 1 for opening moves.
    //      1, 2, and 3 for moves 4-6|8.
    //      3, 1, and 2 for moves > 6|8.
    public void changePanelOrder()
    {

    }
}
