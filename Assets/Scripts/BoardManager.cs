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
        // TODO: CALL GAMECONTROLLER.NEWGAME

        // TODO: GET SQUARES' STATE FROM GAMECONTROLLER



        //for (int i = 0; i < images.Length; i++)
        //{
        //    Sprite tmp = images[i];
        //    int r = Random.Range(i, images.Length);
        //    images[i] = images[r];
        //    images[r] = tmp;
        //}

        //for(int i = 0; i < spriteRenderers.Length; i++)
        //{
        //    spriteRenderers[i].sprite = images[i];
        //}
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
