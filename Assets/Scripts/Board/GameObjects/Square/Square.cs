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
        gameController = GameController.getInstance();
        gameController.getGameBoard().squares[id] = this;
        squareState = gameController.GetSquareStates()[id];
    }
}
