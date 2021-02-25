using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISelectionManager : MonoBehaviour
{
    public void OnHostSelect()
    {
        GameInformation.playerIsHost = true;
        GameInformation.currentPlayer = "HUMAN";

        SceneLoader.LoadLocalGameScene();
    }

    public void OnAIHostSelect()
    {
        GameInformation.playerIsHost = false;
        GameInformation.currentPlayer = "AI";

        SceneLoader.LoadLocalGameScene();
    }
}
