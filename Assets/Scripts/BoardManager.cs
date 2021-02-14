using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class BoardManager : MonoBehaviour
{
    private GameController gameController;

    public Sprite[] images;
    public SpriteRenderer[] spriteRenderers;

    private void Start()
    {
        gameController = GameController.getInstance();
        ReshuffleBoard();
    }

    public void ReshuffleBoard()
    {
        SquareState[] squares = gameController.NewGame();

        for(int i = 0; i < 13; i++)
        {
            SetSquareSpite(squares[i], spriteRenderers[i]);
        }
    }

    public void EndCurrentPlayersTurn()
    {
        Game.playerOneTurn = !Game.playerOneTurn;
        gameController.endTurn();
    }

    public void SetSquareSpite(SquareState squareInfo, SpriteRenderer squareSprite)
    {

    }

}
