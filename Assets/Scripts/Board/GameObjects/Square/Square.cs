using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static GameObjectProperties;



public class Square : MonoBehaviour
{
    public int id;
    public SquareState squareState;

    public GameController gameController;

    void Start()
    {
        squareState.location = id;
        squareState.ownerColor = PlayerColor.Blank;
        squareState.resourceState = SquareStatus.Open;

        gameController = GameController.getInstance();
        gameController.getGameBoard().squares[id] = this;

        squareState.resourceColor = gameController.getRandomResourceColor();
        squareState.resourceAmount = gameController.getRandomResourceAmount(squareState.resourceColor);
    }
}
