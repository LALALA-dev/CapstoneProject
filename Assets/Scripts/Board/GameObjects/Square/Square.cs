using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static GameObjectProperties;



public class Square : MonoBehaviour
{
    public int id;
    public SquareState squareState;

    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameController.getInstance();

        squareState.location = id;
        squareState.ownerColor = PlayerColor.Blank;
        squareState.resourceState = SquareStatus.Open;
        squareState.resourceColor = gameController.getRandomResourceColor();
        squareState.resourceAmount = gameController.getRandomResourceAmount(squareState.resourceColor);

        gameController.getGameBoard().squares[id] = this;
    }
}
