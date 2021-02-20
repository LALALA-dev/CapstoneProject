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
            networkController.SetInfo("Host");
            networkController.SendOpeningBoardConfiguration(gameController.getGameBoard().ToString());
            gameController.SetBoardConfiguration(networkController.GetMove());
        }
        else if(!GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            networkController.SetInfo("Client");
            gameController.SetBoardConfiguration(networkController.GetMove());
        }
    }
}
