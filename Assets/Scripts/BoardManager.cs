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
        if(GameInformation.openingSequence)
        {
            if(OpeningMoveSatisfied())
            {
                gameController.endTurn();
            }
        }
        else
        {
            gameController.endTurn();
        }
        
    }

    public void SetSquareSpite(SquareState squareInfo, SpriteRenderer squareSprite)
    {
        switch(squareInfo.resourceColor)
        {
            case SquareResourceColor.Red:
                switch (squareInfo.resourceAmount)
                {
                    case SquareResourceAmount.One:
                        squareSprite.sprite = images[0];
                        break;
                    case SquareResourceAmount.Two:
                        squareSprite.sprite = images[1];
                        break;
                    case SquareResourceAmount.Three:
                        squareSprite.sprite = images[2];
                        break;
                }
                break;
            case SquareResourceColor.Blue:
                switch (squareInfo.resourceAmount)
                {
                    case SquareResourceAmount.One:
                        squareSprite.sprite = images[3];
                        break;
                    case SquareResourceAmount.Two:
                        squareSprite.sprite = images[4];
                        break;
                    case SquareResourceAmount.Three:
                        squareSprite.sprite = images[5];
                        break;
                }
                break;
            case SquareResourceColor.Yellow:
                switch (squareInfo.resourceAmount)
                {
                    case SquareResourceAmount.One:
                        squareSprite.sprite = images[6];
                        break;
                    case SquareResourceAmount.Two:
                        squareSprite.sprite = images[7];
                        break;
                    case SquareResourceAmount.Three:
                        squareSprite.sprite = images[8];
                        break;
                }
                break;
            case SquareResourceColor.Green:
                switch (squareInfo.resourceAmount)
                {
                    case SquareResourceAmount.One:
                        squareSprite.sprite = images[9];
                        break;
                    case SquareResourceAmount.Two:
                        squareSprite.sprite = images[10];
                        break;
                    case SquareResourceAmount.Three:
                        squareSprite.sprite = images[11];
                        break;
                }
                break;
            case SquareResourceColor.Blank:
                squareSprite.sprite = images[12];
                break;
        }
    }

    public bool OpeningMoveSatisfied()
    {
        return OpeningMovePlacedNode() && OpeningMovePlacedConnectingBranch();
    }

    public bool OpeningMovePlacedNode()
    {
        return GameInformation.openingMoveNodeSet;
    }

    public bool OpeningMovePlacedConnectingBranch()
    {
        return GameInformation.openingMoveBranchSet;
    }
}
