using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NetworkController networkController;
    private GameController gameController;

    public TMP_Text playerOneResources;
    public TMP_Text playerTwoResources;

    private void Awake()
    {
        gameController = GameController.getInstance();
    }

    void Start()
    {
        if (GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            networkController.SendOpeningBoardConfiguration(gameController.getGameBoard().ToString());
        }
        else if(!GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            gameController.SetBoardConfiguration(networkController.GetMove());
        }
    }
}
