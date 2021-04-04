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

    public void SendMessageToGameManager(string message)
    {
        SendMessageUpwards(message);
    }

    public void DetectNewBlockCaptures(Square[] squares)
    {
        Sprite[] captureImages;

       captureImages = playerOneCapture;

        foreach(Square square in squares)
        {
            if(square.squareState.resourceState == SquareStatus.Captured && square.squareState.ownerColor != PlayerColor.Gold)
            {
                switch (square.squareState.resourceColor)
                {
                    case SquareResourceColor.Red:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[0];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[1];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[2];
                                break;
                        }
                        break;
                    case SquareResourceColor.Blue:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[3];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[4];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[5];
                                break;
                        }
                        break;
                    case SquareResourceColor.Yellow:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[6];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[7];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[8];
                                break;
                        }
                        break;
                    case SquareResourceColor.Green:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[9];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[10];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[11];
                                break;
                        }
                        break;
                    default:
                        spriteRenderers[square.squareState.location].sprite = captureImages[12];
                        break;

                }
            }
        }

        captureImages = playerTwoCapture;

        foreach (Square square in squares)
        {
            if (square.squareState.resourceState == SquareStatus.Captured && square.squareState.ownerColor != PlayerColor.Silver)
            {
                switch (square.squareState.resourceColor)
                {
                    case SquareResourceColor.Red:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[0];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[1];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[2];
                                break;
                        }
                        break;
                    case SquareResourceColor.Blue:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[3];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[4];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[5];
                                break;
                        }
                        break;
                    case SquareResourceColor.Yellow:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[6];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[7];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[8];
                                break;
                        }
                        break;
                    case SquareResourceColor.Green:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = captureImages[9];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = captureImages[10];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = captureImages[11];
                                break;
                        }
                        break;
                    default:
                        spriteRenderers[square.squareState.location].sprite = captureImages[12];
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
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[0];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[1];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[2];
                                break;
                        }
                        break;
                    case SquareResourceColor.Blue:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[3];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[4];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[5];
                                break;
                        }
                        break;
                    case SquareResourceColor.Yellow:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[6];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[7];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[8];
                                break;
                        }
                        break;
                    case SquareResourceColor.Green:
                        switch (square.squareState.resourceAmount)
                        {
                            case SquareResourceAmount.One:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[9];
                                break;
                            case SquareResourceAmount.Two:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[10];
                                break;
                            case SquareResourceAmount.Three:
                                spriteRenderers[square.squareState.location].sprite = resourceBlock[11];
                                break;
                        }
                        break;

                }
            }
        }
    }

    public void RefreshBoardGUI()
    {
        if(GameInformation.playerIsHost)
            BroadcastMessage("UpdateAIGUI", PlayerColor.Gold);
        else
            BroadcastMessage("UpdateAIGUI", PlayerColor.Silver);
    }

    public void ToggleNodeBranchTriggers()
    {
        BroadcastMessage("ToggleTrigger");
    }

    public void SolidifyNodeSelections(int id)
    {
        BroadcastMessage("SolidifyNodeClaim", id);
    }

    public void SolidifyBranchSelection(int id)
    {
        BroadcastMessage("SolidifyBranchClaim", id);
    }

    public void OrphanBranchFound(int id)
    {
        BroadcastMessage("UnclaimOrphan", id);
    }

}
