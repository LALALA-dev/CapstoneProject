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

    int numberOfTilesSelected = 0;
    int[] resources = new int[4] { 0, 0, 0, 0 };

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            createResourceBtn[i].gameObject.SetActive(false);
        }
    }

    public void OnConfirmClick()
    {
        ApplyResourceChanges();
        numberOfTilesSelected = 0;
        resources = new int[4] { 0, 0, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            createResourceBtn[i].gameObject.SetActive(false);
        }

        panel.SetActive(false);
    }

    public void OnCancelClick()
    {
        numberOfTilesSelected = 0;
        resources = new int[4] { 0, 0, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            createResourceBtn[i].gameObject.SetActive(false);
        }

        panel.SetActive(false);
    }

    public void AddColorTile(int colorId)
    {
        int numSelected = int.Parse(tilesSelected[colorId].text);
        if(numSelected < 3)
        {
            numSelected++;
            tilesSelected[colorId].text = numSelected.ToString();
            resources[colorId]++;
            numberOfTilesSelected++;
        }
        else
        {
            numSelected = 0;
            numberOfTilesSelected = 0;
            resources[colorId] = 0;
            tilesSelected[colorId].text = numSelected.ToString();

            for (int i = 0; i < 4; i++)
            {
                createResourceBtn[i].gameObject.SetActive(false);
            }
        }

        if(numberOfTilesSelected == 3)
        {
            RenderCreateBtnChoices();
        }
    }

    public void RenderCreateBtnChoices()
    {
        for (int i = 0; i < 4; i++)
        {
            if (resources[i] == 0)
                createResourceBtn[i].gameObject.SetActive(true);
        }
    }

    public void ApplyResourceChanges()
    {

    }
}
