using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class NodeController : MonoBehaviour
{
    public Sprite playerOneSprite;
    public Sprite playerTwoSprite;
    public Sprite blankSprite;

    public Node nodeEntity;

    void Start()
    {
        ClaimNode(blankSprite);
    }

    private void OnMouseDown()
    {
        if (GameInformation.openingSequence)
        {
            if(isNodeBlank() && !GameInformation.openingMoveNodeSet)
            {
                nodeEntity.nodeState.nodeColor = nodeEntity.gameController.getCurrentPlayerColor();
                GameInformation.openingMoveNodeSet = true;
                GameInformation.openingNodeId = nodeEntity.id;

                if (nodeEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
                    ClaimNode(playerOneSprite);
                else
                    ClaimNode(playerTwoSprite);
            }
            else if(isNodeColorOfCurrentPlayer() && GameInformation.openingMoveNodeSet && GameInformation.openingNodeId == nodeEntity.id)
            {
                nodeEntity.nodeState.nodeColor = PlayerColor.Blank;
                GameInformation.openingMoveNodeSet = false;
                ClaimNode(blankSprite);

                if(GameInformation.openingMoveBranchSet)
                {
                    SendMessageUpwards("BranchUIUpdate", GameInformation.openingBranchId);
                    GameInformation.openingMoveBranchSet = false;
                }
            }
        }
        else if (hasEnoughResources() && isNodeConnectedToBranch() && isNodeBlank())
        {
            nodeEntity.nodeState.nodeColor = nodeEntity.gameController.getCurrentPlayerColor();

            // Change color
            if (nodeEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
                ClaimNode(playerOneSprite);
            else
                ClaimNode(playerTwoSprite);
        }
        // Are you trying to undo a selection?
        else if (isNodeColorOfCurrentPlayer())
        {
            nodeEntity.nodeState.nodeColor = PlayerColor.Blank;

            ClaimNode(blankSprite);
        }
    }

    private void ClaimNode(Sprite playerColor)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = playerColor;
    }

    private bool isNodeBlank()
    {
        return nodeEntity.nodeState.nodeColor == PlayerColor.Blank;
    }


    private bool isNodeColorOfCurrentPlayer()
    {
        return nodeEntity.nodeState.nodeColor == nodeEntity.gameController.getCurrentPlayerColor();
    }

    public bool hasEnoughResources()
    {
        int[] resources = new int[4];
        if (nodeEntity.gameController.getCurrentPlayerColor() == PlayerColor.Orange)
        {
            resources = GameInformation.playerOneResources;
        }
        else
        {
            resources = GameInformation.playerTwoResources;
        }

        return (resources[2] >= 2 && resources[3] >= 2);
    }

    public bool isNodeConnectedToBranch()
    {
        int[] branchConnections = ReferenceScript.nodeConnectsToTheseBranches[nodeEntity.id];

        foreach(int branchId in branchConnections)
        {
            if(nodeEntity.gameController.GetBranchState(branchId).branchColor == nodeEntity.gameController.getCurrentPlayerColor())
            {
                return true;
            }
        }

        return false;
    }
}
