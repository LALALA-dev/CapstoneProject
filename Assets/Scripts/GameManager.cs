using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NetworkController networkController;
    [SerializeField] private BoardManager boardManager;
    private GameController gameController;

    public GameObject RenderBtn;

    private void Awake()
    {
        gameController = GameController.getInstance();

        if (GameInformation.playerIsHost)
            RenderBtn.gameObject.SetActive(false);
    }

    void Start()
    {
        if (GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            networkController.SetInfo("Host");
            networkController.SendOpeningBoardConfiguration(gameController.getGameBoard().ToString());
        }
        else if(!GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            //gameController.SetBoardConfiguration(networkController.GetMove());
        }
    }

    public void RenderHostBoard()
    {
        if (!GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            string hostBoard = networkController.GetMove();
            gameController.SetBoardConfiguration(hostBoard);
            RenderBtn.gameObject.SetActive(false);
            boardManager.ReshuffleBoard();

        }
    }
}
