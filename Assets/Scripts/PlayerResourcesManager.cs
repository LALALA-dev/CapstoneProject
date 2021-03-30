using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameObjectProperties;

public class PlayerResourcesManager : MonoBehaviour
{
    public Text[] playerOneResources;
    public Text[] playerTwoResources;

    public void UpdateResourcesUI(PlayerColor playerColor)
    {
        if (playerColor == PlayerColor.Orange)
            UpdatePlayerResources(playerOneResources, GameInformation.playerOneResources);
        else
            UpdatePlayerResources(playerTwoResources, GameInformation.playerTwoResources);
    }

    private void UpdatePlayerResources(Text[] labels, int[] resources)
    {
        for (int i = 0; i < 4; i++)
        {
            labels[i].text = resources[i].ToString();
        }
    }

    public void UpdateBothPlayersResources()
    {
        UpdateResourcesUI(PlayerColor.Orange);
        UpdateResourcesUI(PlayerColor.Purple);
    }
}
