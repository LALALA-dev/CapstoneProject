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
        for(int i = 0; i < 4; i++)
            createResourceBtn[i].onClick.AddListener(() => AddColorTile(i));
        
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
        numSelected++;
        tilesSelected[colorId].text = numSelected.ToString();
    }

    public void ApplyResourceChanges()
    {

    }
}
