using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameObjectProperties;

public class BoardManager : MonoBehaviour
{
    private GameController gameController;

    public Sprite[] images;
    public Sprite[] playerOneCapture;
    public Sprite[] playerTwoCapture;
    public Sprite[] resourceBlock;
    public SpriteRenderer[] spriteRenderers;

    public Text[] playerOneResources;
    public Text[] playerTwoResources;

    private void Start()
    {
        gameController = GameController.getInstance();

        SquareState[] squares = gameController.GetSquareStates();

        for (int i = 0; i < 13; i++)
        {
            SetSquareSpite(squares[i], spriteRenderers[i]);
        }
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
        if (GameInformation.openingSequence)
        {
            if (OpeningMoveSatisfied())
            {
                gameController.endTurn();
                DetectNewTileBlocks();
            }
        }
        else
        {
            gameController.endTurn();
            DetectNewTileBlocks();
            DetectNewBlockCaptures();
            if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
            {
                GameInformation.gameOver = true;
            }
        }
        
        if(GameInformation.turnNumber + 1 > 4 && !GameInformation.gameOver)
        {
            UpdateResourcesUI();
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

    public void BranchUIUpdate(int branchID)
    {
        BroadcastMessage("BranchUIUpdate", branchID);
    }

    public void UpdateResourcesUI()
    {
        if(gameController.getCurrentPlayerColor() == PlayerColor.Orange)
            UpdatePlayerResources(playerOneResources, GameInformation.playerOneResources);
        else
            UpdatePlayerResources(playerTwoResources, GameInformation.playerTwoResources);
    }

    private void UpdatePlayerResources(Text[] labels, int[] resources)
    {
        for(int i = 0; i < 4; i++)
        {
            labels[i].text = resources[i].ToString();
        }
    }

    private void DetectNewBlockCaptures()
    {
        SquareState[] squares = gameController.GetSquareStates();
        Sprite[] captureImages;

        if (gameController.getCurrentPlayerColor() == PlayerColor.Orange)
            captureImages = playerTwoCapture;
        else
            captureImages = playerOneCapture;

        foreach(SquareState square in squares)
        {
            if(square.resourceState == SquareStatus.Captured && square.ownerColor != gameController.getCurrentPlayerColor())
            {
                switch (square.resourceColor)
                {
                    case SquareResourceColor.Red:
                        spriteRenderers[square.location].sprite = captureImages[0];
                        break;
                    case SquareResourceColor.Blue:
                        spriteRenderers[square.location].sprite = captureImages[1];
                        break;
                    case SquareResourceColor.Yellow:
                        spriteRenderers[square.location].sprite = captureImages[2];
                        break;
                    case SquareResourceColor.Green:
                        spriteRenderers[square.location].sprite = captureImages[3];
                        break;
                    default:
                        spriteRenderers[square.location].sprite = captureImages[4];
                        break;

                }
            }
        }
    }

    private void DetectNewTileBlocks()
    {
        Square[] squares = gameController.getGameBoard().squares;
        foreach (Square square in squares)
        {
            if (square.squareState.resourceState == SquareStatus.Blocked)
            {
                switch (square.squareState.resourceColor)
                {
                    case SquareResourceColor.Red:
                        spriteRenderers[square.squareState.location].sprite = resourceBlock[0];
                        break;
                    case SquareResourceColor.Blue:
                        spriteRenderers[square.squareState.location].sprite = resourceBlock[1];
                        break;
                    case SquareResourceColor.Yellow:
                        spriteRenderers[square.squareState.location].sprite = resourceBlock[2];
                        break;
                    case SquareResourceColor.Green:
                        spriteRenderers[square.squareState.location].sprite = resourceBlock[3];
                        break;

                }
            }
        }
    }
}
