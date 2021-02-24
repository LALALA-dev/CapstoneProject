using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameObjectProperties;

public class BoardManager : MonoBehaviour
{
    public Sprite[] images;
    public Sprite[] playerOneCapture;
    public Sprite[] playerTwoCapture;
    public Sprite[] resourceBlock;
    public SpriteRenderer[] spriteRenderers;

    public void SetSquareUI(SquareState[] squares)
    {
        for(int i = 0; i < 13; i++)
        {
            SetSquareSpite(squares[i], spriteRenderers[i]);
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



    public void BranchUIUpdate(int branchID)
    {
        BroadcastMessage("ResetBranchUpdate", branchID);
    }

    public void DetectNewBlockCaptures(SquareState[] squares)
    {
        Sprite[] captureImages;

       captureImages = playerOneCapture;

        foreach(SquareState square in squares)
        {
            if(square.resourceState == SquareStatus.Captured && square.ownerColor != PlayerColor.Purple)
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

        captureImages = playerTwoCapture;

        foreach (SquareState square in squares)
        {
            if (square.resourceState == SquareStatus.Captured && square.ownerColor != PlayerColor.Orange)
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

    public void DetectNewTileBlocks(Square[] squares)
    {
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

    public void RefreshForAIMoves()
    {
        if(GameInformation.playerIsHost)
            BroadcastMessage("UpdateAIGUI", PlayerColor.Purple);
        else
            BroadcastMessage("UpdateAIGUI", PlayerColor.Orange);
    }

    public void UpdateGameBoardUI()
    {

    }

}
