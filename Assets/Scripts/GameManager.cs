using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NetworkController networkController;
    private GameController gameController;

    private void Awake()
    {
        networkController = NetworkController.NetController;
        gameController = GameController.getInstance();
    }

    void Start()
    {
        if (GameInformation.playerIsHost && GameInformation.gameType == 'N')
            networkController.SendOpponentBoardConfiguration(gameController.getGameBoard().ToString());
    }
}
