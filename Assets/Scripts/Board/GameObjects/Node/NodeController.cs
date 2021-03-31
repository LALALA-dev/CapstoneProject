using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class NodeController : MonoBehaviour
{
    public Sprite playerOneSprite;
    public Sprite playerTwoSprite;
    public Sprite blankSprite;
    public Sprite highlight;
    public Sprite[] playerAvatars;
    public Sprite[] highlightAvatars;

    public Node nodeEntity;

    public bool avatarsSet = false;
    private int avatarIndex = 0;

    void Start()
    {
        ClaimNode(blankSprite);
    }

    private void Update()
    {
            switch (GameInformation.playerOneAvatar)
            {
                case "HAT":
                    playerOneSprite = playerAvatars[0];
                    avatarIndex = 0;
                    break;
                case "BATTLESHIP":
                    playerOneSprite = playerAvatars[1];
                    avatarIndex = 1;
                    break;
                case "CAR":
                    playerOneSprite = playerAvatars[2];
                    avatarIndex = 2;
                    break;
                case "THIMBLE":
                    playerOneSprite = playerAvatars[3];
                    avatarIndex = 3;
                    break;
                case "WHEELBARREL":
                    playerOneSprite = playerAvatars[4];
                    avatarIndex = 4;
                    break;
                default:
                    playerOneSprite = playerAvatars[2];
                    break;
            }

            switch (GameInformation.playerTwoAvatar)
            {
                case "HAT":
                    playerTwoSprite = playerAvatars[5];
                    avatarIndex = 5;
                    break;
                case "BATTLESHIP":
                    playerTwoSprite = playerAvatars[6];
                    avatarIndex = 6;
                    break;
                case "CAR":
                    playerTwoSprite = playerAvatars[7];
                    avatarIndex = 7;
                    break;
                case "THIMBLE":
                    playerTwoSprite = playerAvatars[8];
                    avatarIndex = 8;
                    break;
                case "WHEELBARREL":
                    playerTwoSprite = playerAvatars[9];
                    avatarIndex = 9;
                    break;
                default:
                    playerTwoSprite = playerAvatars[9];
                    avatarIndex = 9;
                    break;
            }
    }

    private void OnMouseDown()
    {
        if(!GameInformation.gameOver)
        {
            if (GameInformation.openingSequence)
            {
                if (isNodeBlank() && !GameInformation.openingMoveNodeSet)
                {
                    nodeEntity.nodeState.nodeColor = nodeEntity.gameController.getCurrentPlayerColor();
                    GameInformation.openingMoveNodeSet = true;
                    GameInformation.openingNodeId = nodeEntity.id;

                    if (nodeEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
                        ClaimNode(playerOneSprite);
                    else
                        ClaimNode(playerTwoSprite);

                }
                else if (isNodeColorOfCurrentPlayer() && GameInformation.openingMoveNodeSet && GameInformation.openingNodeId == nodeEntity.id)
                {
                    nodeEntity.nodeState.nodeColor = PlayerColor.Blank;
                    nodeEntity.gameController.getGameBoard().getBoardState().nodeStates[nodeEntity.id].nodeColor = PlayerColor.Blank;
                    GameInformation.openingMoveNodeSet = false;
                    ClaimNode(blankSprite);

                    if (GameInformation.openingMoveBranchSet)
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
                if (nodeEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
                {
                    ClaimNode(playerOneSprite);
                    GameInformation.playerOneResources[2] -= 2;
                    GameInformation.playerOneResources[3] -= 2;
                }
                else
                {
                    ClaimNode(playerTwoSprite);
                    GameInformation.playerTwoResources[2] -= 2;
                    GameInformation.playerTwoResources[3] -= 2;
                }

                GameInformation.currentRoundPlacedNodes.Add(nodeEntity.id);
                SendMessageUpwards("SendMessageToGameManager", "UpdateResourcesUI");
            }
            // Are you trying to undo a selection?
            else if (isNodeColorOfCurrentPlayer() && isUndoAttemptOnNodePlaceThisRound())
            {
                nodeEntity.nodeState.nodeColor = PlayerColor.Blank;
                nodeEntity.gameController.getGameBoard().getBoardState().nodeStates[nodeEntity.id].nodeColor = PlayerColor.Blank;

                GameInformation.currentRoundPlacedNodes.Remove(nodeEntity.id);

                if (nodeEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
                {
                    GameInformation.playerOneResources[2] += 2;
                    GameInformation.playerOneResources[3] += 2;
                }
                else
                {
                    GameInformation.playerTwoResources[2] += 2;
                    GameInformation.playerTwoResources[3] += 2;
                }
                SendMessageUpwards("SendMessageToGameManager", "UpdateResourcesUI");
                ClaimNode(blankSprite);
                nodeEntity.nodeState.nodeColor = PlayerColor.Blank;
                nodeEntity.gameController.getGameBoard().getBoardState().nodeStates[nodeEntity.id].nodeColor = PlayerColor.Blank;
            }
        }
    }

    public void OnMouseEnter()
    {
        if (isNodeBlank() && hasEnoughResources() && isNodeConnectedToBranch())
        {
            ClaimNode(highlight);
        }
        else if(GameInformation.openingSequence && !GameInformation.openingMoveNodeSet && isNodeBlank())
        {
            ClaimNode(highlight);
        }
    }

    public void OnMouseExit()
    {
        if (!GameInformation.openingSequence)
        {
            if (!GameInformation.currentRoundPlacedNodes.Contains(nodeEntity.id) && nodeEntity.nodeState.nodeColor == PlayerColor.Blank)
            {
                ClaimNode(blankSprite);
            }
        }
        else
        {
            if (!GameInformation.openingMoveNodeSet && isNodeBlank())
            {
                ClaimNode(blankSprite);
            }
        }
    }

    public void ClaimNode(Sprite playerAvatar)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = playerAvatar;
    }

    private bool isUndoAttemptOnNodePlaceThisRound()
    {
        if (GameInformation.currentRoundPlacedNodes.Contains(nodeEntity.id))
        {
            return true;
        }

        return false;
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
        if (nodeEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
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
            if(nodeEntity.gameController.getGameBoard().branches[branchId].branchState.branchColor == nodeEntity.gameController.getCurrentPlayerColor())
            {
                return true;
            }
        }

        return false;
    }

    public void UpdateAIGUI(PlayerColor color)
    {
        if (nodeEntity.nodeState.nodeColor == color)
        {
            nodeEntity.nodeState.nodeColor = color;

            if (color == PlayerColor.Silver)
                ClaimNode(playerOneSprite);
            else
                ClaimNode(playerTwoSprite);
        }
    }

    public void ToggleTrigger()
    {
        BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
        if (boxCollider.enabled)
        {
            boxCollider.enabled = false;
        }
        else
        {
            boxCollider.enabled = true;
        }
    }
}
