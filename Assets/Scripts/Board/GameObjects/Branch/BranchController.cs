using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class BranchController : MonoBehaviour
{
    public Sprite playerOneSprite;
    public Sprite playerTwoSprite;
    public Sprite blankSprite;

    public Branch branchEntity;

    void Start()
    {
        ClaimBranch(blankSprite);
    }

    private void OnMouseDown()
    {
        if(GameInformation.openingSequence)
        {
            if(GameInformation.openingMoveNodeSet && !GameInformation.openingMoveBranchSet && isBranchBlank())
            {
                if (isOpeningBranchConnectedToNewNode())
                {
                    GameInformation.openingMoveBranchSet = true;
                    GameInformation.openingBranchId = branchEntity.id;
                    branchEntity.branchState.ownerColor = branchEntity.gameController.getCurrentPlayerColor();
                    branchEntity.branchState.branchColor = branchEntity.gameController.getCurrentPlayerColor();

                    if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
                        ClaimBranch(playerOneSprite);
                    else
                        ClaimBranch(playerTwoSprite);
                }
            }
            else if(isBranchColorOfCurrentPlayer() && GameInformation.openingMoveBranchSet && GameInformation.openingBranchId == branchEntity.id)
            {
                branchEntity.branchState.ownerColor = PlayerColor.Blank;
                branchEntity.branchState.branchColor = PlayerColor.Blank;
                GameInformation.openingMoveBranchSet = false;
                ClaimBranch(blankSprite);
            }

        }
        else if(isBranchBlank() || isBranchSurroundedByCurrentPlayer())
        {
            branchEntity.branchState.ownerColor = branchEntity.gameController.getCurrentPlayerColor();
            branchEntity.branchState.branchColor = branchEntity.gameController.getCurrentPlayerColor();

            // Change color
            if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
                ClaimBranch(playerOneSprite);
            else
                ClaimBranch(playerTwoSprite);
        }
        // Are you trying to undo a selection?
        else if (isBranchColorOfCurrentPlayer())
        {
            branchEntity.branchState.ownerColor = PlayerColor.Blank;
            branchEntity.branchState.branchColor = PlayerColor.Blank;

            ClaimBranch(blankSprite);
        }
    }

    private void ClaimBranch(Sprite playerColor)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = playerColor;
    }

    private bool isBranchBlank()
    {
        return branchEntity.branchState.ownerColor == PlayerColor.Blank;
    }

    private bool isBranchSurroundedByCurrentPlayer()
    {
        return ((branchEntity.branchState.ownerColor == branchEntity.gameController.getCurrentPlayerColor()) && (branchEntity.branchState.branchColor == PlayerColor.Blank));
    }

    private bool isBranchColorOfCurrentPlayer()
    {
        return branchEntity.branchState.branchColor == branchEntity.gameController.getCurrentPlayerColor();
    }

    private bool isOpeningBranchConnectedToNewNode()
    {
        bool result = false;
        int[] nodeConnections = ReferenceScript.nodeConnectsToTheseBranches[GameInformation.openingNodeId];
        foreach(int branch in nodeConnections)
        {
            if (branch == branchEntity.id)
                result = true;
        }

        return result;
    }

    public void BranchUIUpdate(int id)
    {
        if(id == branchEntity.id)
        {
            branchEntity.branchState.ownerColor = PlayerColor.Blank;
            branchEntity.branchState.branchColor = PlayerColor.Blank;
            ClaimBranch(blankSprite);
        }
    }
}
