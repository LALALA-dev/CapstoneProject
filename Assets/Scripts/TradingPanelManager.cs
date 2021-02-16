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
    }

    public void OnConfirmClick()
    {
        ApplyResourceChanges();
        numberOfTilesSelected = 0;
        resources = new int[4] { 0, 0, 0, 0 };
        panel.SetActive(false);
    }

    public void OnCancelClick()
    {
        numberOfTilesSelected = 0;
        resources = new int[4] { 0, 0, 0, 0 };
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
        }
    }

    public void ApplyResourceChanges()
    {

    }
}
