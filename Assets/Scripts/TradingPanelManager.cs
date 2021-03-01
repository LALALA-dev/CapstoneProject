using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TradingPanelManager : MonoBehaviour
{
    public TMP_Text[] tilesSelected;
    public Button[] createResourceBtn;
    public GameObject panel;

    private int numberOfTilesSelected = 0;
    private int[] resources = new int[4] { 0, 0, 0, 0 };

    void Start()
    {
        panel.SetActive(false);
        for (int i = 0; i < 4; i++)
            createResourceBtn[i].gameObject.SetActive(false);
    }

    public void OnCancelClick()
    {
        numberOfTilesSelected = 0;
        resources = new int[4] { 0, 0, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            createResourceBtn[i].gameObject.SetActive(false);
            tilesSelected[i].text = "0";
        }

        SendMessageUpwards("ToogleTriggers");
        panel.SetActive(false);
    }

    public void AddColorTile(int colorId)
    {
        int numSelected = int.Parse(tilesSelected[colorId].text);
        if(numSelected < 3 && numberOfTilesSelected < 3 && numSelected + 1 <= GameInformation.maxTradeResources[colorId])
        {
            numSelected++;
            tilesSelected[colorId].text = numSelected.ToString();
            resources[colorId]++;
            numberOfTilesSelected++;
        }
        else if(numSelected == 3)
        {
            numSelected = 0;
            numberOfTilesSelected = 0;
            resources[colorId] = 0;
            tilesSelected[colorId].text = numSelected.ToString();
            for (int i = 0; i < 4; i++)
                createResourceBtn[i].gameObject.SetActive(false);
        }
        else if(numSelected < 3 && numberOfTilesSelected == 3)
        {
            numberOfTilesSelected -= numSelected;
            numSelected = 0;
            resources[colorId] = 0;
            tilesSelected[colorId].text = numSelected.ToString();

            for(int i = 0; i < 4; i++)
                createResourceBtn[i].gameObject.SetActive(false);
        }
            

        RenderCreateBtnChoices();
    }

    public void RenderCreateBtnChoices()
    {
        if(numberOfTilesSelected == 3)
            for (int i = 0; i < 4; i++)
                if (resources[i] == 0)
                    createResourceBtn[i].gameObject.SetActive(true);
    }

    public void CreateBtnClick(int colorId)
    {
        ApplyResourceChanges(colorId);
        numberOfTilesSelected = 0;
        resources = new int[4] { 0, 0, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            createResourceBtn[i].gameObject.SetActive(false);
            tilesSelected[i].text = "0";
        }
            
        panel.SetActive(false);
        GameInformation.resourceTrade = true;
        GameInformation.tradeHasBeenMade = true;
    }

    public void ApplyResourceChanges(int colorId)
    {
        if(GameInformation.turnNumber % 2 == 0)
        {
            GameInformation.playerOneResources[colorId]++;
            for (int i = 0; i < 4; i++)
                GameInformation.playerOneResources[i] -= resources[i];
        }
        else
        {
            GameInformation.playerTwoResources[colorId]++;
            for (int i = 0; i < 4; i++)
                GameInformation.playerTwoResources[i] -= resources[i];
        }
    }

    public void EnablePanel()
    {
        if(!GameInformation.resourceTrade)
        {
            if (GameInformation.turnNumber % 2 == 0)
                GameInformation.maxTradeResources = GameInformation.playerOneResources;
            else
                GameInformation.maxTradeResources = GameInformation.playerTwoResources;
            panel.SetActive(true);
            SendMessageUpwards("ToogleTriggers");
        }
    }
}
