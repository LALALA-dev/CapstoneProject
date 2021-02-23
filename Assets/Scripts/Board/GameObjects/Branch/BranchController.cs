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
        if(!GameInformation.gameOver)
        {
            if (GameInformation.openingSequence)
            {
                if (GameInformation.openingMoveNodeSet && !GameInformation.openingMoveBranchSet && isBranchBlank())
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
                else if (isBranchColorOfCurrentPlayer() && GameInformation.openingMoveBranchSet && GameInformation.openingBranchId == branchEntity.id)
                {
                    branchEntity.branchState.ownerColor = PlayerColor.Blank;
                    branchEntity.branchState.branchColor = PlayerColor.Blank;
                    GameInformation.openingMoveBranchSet = false;
                    ClaimBranch(blankSprite);
                }

            }
            else if ((isBranchBlank() && hasEnoughResources() && isBranchConnectedToBranch()) || isBranchSurroundedByCurrentPlayer())
            {
                branchEntity.branchState.ownerColor = branchEntity.gameController.getCurrentPlayerColor();
                branchEntity.branchState.branchColor = branchEntity.gameController.getCurrentPlayerColor();

                // Change color
                if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
                {
                    ClaimBranch(playerOneSprite);
                    GameInformation.playerOneResources[0]--;
                    GameInformation.playerOneResources[1]--;
                }
                else
                {
                    ClaimBranch(playerTwoSprite);
                    GameInformation.playerTwoResources[0]--;
                    GameInformation.playerTwoResources[1]--;
                }
                SendMessageUpwards("UpdateResourcesUI");
            }
            // Are you trying to undo a selection?
            else if (isBranchColorOfCurrentPlayer())
            {
                branchEntity.branchState.ownerColor = PlayerColor.Blank;
                branchEntity.branchState.branchColor = PlayerColor.Blank;

                if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
                {
                    GameInformation.playerOneResources[0]++;
                    GameInformation.playerOneResources[1]++;
                }
                else
                {
                    GameInformation.playerTwoResources[0]++;
                    GameInformation.playerTwoResources[1]++;
                }
                SendMessageUpwards("UpdateResourcesUI");
                ClaimBranch(blankSprite);
            }
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

    public void ResetBranchUpdate(int id)
    {
        if(id == branchEntity.id)
        {
            branchEntity.branchState.ownerColor = PlayerColor.Blank;
            branchEntity.branchState.branchColor = PlayerColor.Blank;
            ClaimBranch(blankSprite);
        }
    }

    public void UpdateAIGUI(PlayerColor color)
    {
        if (branchEntity.branchState.branchColor == color)
        {
            branchEntity.branchState.ownerColor = color;
            branchEntity.branchState.branchColor = color;

            if (color == PlayerColor.Orange)
                ClaimBranch(playerOneSprite);
            else
                ClaimBranch(playerTwoSprite);
        }
    }

    public bool hasEnoughResources()
    {
        int[] resources = new int[4];
        if(branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
        {
            resources = GameInformation.playerOneResources;
        }
        else
        {
            resources = GameInformation.playerTwoResources;
        }

        return (resources[0] >= 1 && resources[1] >= 1);
    }

    public bool isBranchConnectedToBranch()
    {
        int[] branchConnections = ReferenceScript.branchConnectsToTheseBranches[branchEntity.id];

        foreach (int branchId in branchConnections)
        {
            if (branchEntity.gameController.GetBranchState(branchId).branchColor == branchEntity.gameController.getCurrentPlayerColor())
            {
                return true;
            }
        }

        return false;
    }
}
