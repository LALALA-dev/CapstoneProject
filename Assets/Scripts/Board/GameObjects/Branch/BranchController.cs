using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class BranchController : MonoBehaviour
{
    public Sprite playerOneSprite;
    public Sprite playerTwoSprite;
    public Sprite blankSprite;

    public Sprite highlight;

    public Sprite playerOneHighlight;
    public Sprite playerTwoHighlight;

    public Branch branchEntity;

    public AudioSource place;
    public AudioSource remove;

    void Start()
    {
        ClaimBranch(blankSprite);
    }

    private void OnMouseDown()
    {
        if(!GameInformation.gameOver && GameInformation.currentPlayer != "AI")
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

                        if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
                            ClaimBranch(playerOneHighlight);
                        else
                            ClaimBranch(playerTwoHighlight);
                        place.Play();
                    }
                }
                else if (isBranchColorOfCurrentPlayer() && GameInformation.openingMoveBranchSet && GameInformation.openingBranchId == branchEntity.id)
                {
                    branchEntity.branchState.ownerColor = PlayerColor.Blank;
                    branchEntity.branchState.branchColor = PlayerColor.Blank;
                    GameInformation.openingMoveBranchSet = false;
                    ClaimBranch(blankSprite);
                    remove.Play();
                }

            }
            else if ((isBranchBlank() && hasEnoughResources() && isBranchConnectedToBranch()) || isBranchSurroundedByCurrentPlayer())
            {
                branchEntity.branchState.ownerColor = branchEntity.gameController.getCurrentPlayerColor();
                branchEntity.branchState.branchColor = branchEntity.gameController.getCurrentPlayerColor();
                // Change color
                if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
                {
                    ClaimBranch(playerOneHighlight);
                    GameInformation.playerOneResources[0]--;
                    GameInformation.playerOneResources[1]--;
                }
                else
                {
                    ClaimBranch(playerTwoHighlight);
                    GameInformation.playerTwoResources[0]--;
                    GameInformation.playerTwoResources[1]--;
                }
                place.Play();
                GameInformation.currentRoundPlacedBranches.Add(branchEntity.id);
                SendMessageUpwards("SendMessageToGameManager", "UpdateResourcesUI");
            }
            else if (isBranchColorOfCurrentPlayer() && isUndoAttemptOnBranchPlaceThisRound())
            {
                int branchesUndone = 1;
                int nodesUndone = 0;
                branchEntity.branchState.ownerColor = PlayerColor.Blank;
                branchEntity.branchState.branchColor = PlayerColor.Blank;
                GameInformation.currentRoundPlacedBranches.Remove(branchEntity.id);

                branchesUndone += CheckForBranchOrphans();
                nodesUndone += CheckForNodeOrphans();

                if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
                {
                    GameInformation.playerOneResources[0] += branchesUndone;
                    GameInformation.playerOneResources[1] += branchesUndone;
                    GameInformation.playerOneResources[2] += (nodesUndone * 2);
                    GameInformation.playerOneResources[3] += (nodesUndone * 2);
                }
                else
                {
                    GameInformation.playerTwoResources[0] += branchesUndone;
                    GameInformation.playerTwoResources[1] += branchesUndone;
                    GameInformation.playerTwoResources[2] += (nodesUndone * 2);
                    GameInformation.playerTwoResources[3] += (nodesUndone * 2);
                }
                SendMessageUpwards("SendMessageToGameManager", "UpdateResourcesUI");
                ClaimBranch(blankSprite);
                remove.Play();
            }
        }
    }

    public void OnMouseEnter()
    {
        if (GameInformation.gameType != 'T' && GameInformation.currentPlayer != "AI")
        {
            if ((isBranchBlank() && hasEnoughResources() && (isBranchConnectedToBranch()) || isBranchSurroundedByCurrentPlayer()))
            {
                ClaimBranch(highlight);
            }
            else if (GameInformation.openingSequence && GameInformation.openingMoveNodeSet && !GameInformation.openingMoveBranchSet && isBranchBlank() && isOpeningBranchConnectedToNewNode())
            {
                ClaimBranch(highlight);
            }
        }
    }

    public void OnMouseExit()
    {
        if (GameInformation.gameType != 'T' && GameInformation.currentPlayer != "AI")
        {
            if (!GameInformation.openingSequence)
            {

                if (!GameInformation.currentRoundPlacedBranches.Contains(branchEntity.id) && (branchEntity.branchState.ownerColor == PlayerColor.Blank || isBranchSurroundedByCurrentPlayer()))
                {
                    ClaimBranch(blankSprite);
                }
            }
            else
            {
                if (!GameInformation.openingMoveBranchSet && isBranchBlank())
                {
                    ClaimBranch(blankSprite);
                }
            }
        }
    }

    public void SolidifyBranchClaim(int id)
    {
        if (branchEntity.id == id)
        {
            if (branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
                ClaimBranch(playerOneSprite);
            else
                ClaimBranch(playerTwoSprite);
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

    private bool isUndoAttemptOnBranchPlaceThisRound()
    {
        if(GameInformation.currentRoundPlacedBranches.Contains(branchEntity.id))
        {
            return true;
        }

        return false;
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

            if (color == PlayerColor.Silver)
                ClaimBranch(playerOneSprite);
            else
                ClaimBranch(playerTwoSprite);
        }
    }

    public bool hasEnoughResources()
    {
        int[] resources = new int[4];
        if(branchEntity.gameController.getCurrentPlayerColor() == PlayerColor.Silver)
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
            if (branchEntity.gameController.getGameBoard().branches[branchId].branchState.branchColor == branchEntity.gameController.getCurrentPlayerColor())
            {
                return true;
            }
        }

        return false;
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

    public void BroadcastOrphanBranchFound(int id)
    {
        SendMessageUpwards("OrphanBranchFound", id);
    }

    public void BroadcastOrphanNodeFound(int id)
    {
        SendMessageUpwards("OrphanNodeFound", id);
    }

    public void UnclaimOrphanBranch(int id)
    {
        if(branchEntity.id == id)
        {
            branchEntity.branchState.branchColor = PlayerColor.Blank;
            branchEntity.branchState.ownerColor = PlayerColor.Blank;
            ClaimBranch(blankSprite);
            remove.Play();
        }
    }

    public int CheckForBranchOrphans()
    {
        // Definition of an orphan, a branch or branches that are no longer connected to either the existing network
        // nor to a current round placed branch that is connected to the existing network 

        // TODO: FIND THE BRANCHES THAT ARE STILL CONNECTED TO THE EXISTING NETWORK, PUSH THE ONES THAT AREN'T ONTO A LIST OF POTENTIAL ORPHANS
        // PUSH THE ONES THAT ARE ONTO ANOTHER LIKE OF CONFIRMED LEGAL BRANCHES
        List<int> confirmedLegalBranches = new List<int>();
        List<int> possibleOrphans = new List<int>();
        int numberRemoved = 0;
        for (int i = 0; i < GameInformation.currentRoundPlacedBranches.Count; i++)
        {
            bool ownedBranchFound = false;
            int[] connectedBranches = ReferenceScript.branchConnectsToTheseBranches[GameInformation.currentRoundPlacedBranches[i]];
            for (int j = 0; j < connectedBranches.Length; j++)
            {
                if (branchEntity.gameController.getGameBoard().branches[connectedBranches[j]].branchState.ownerColor == branchEntity.gameController.getCurrentPlayerColor()
                    && !GameInformation.currentRoundPlacedBranches.Contains(connectedBranches[j]))
                {
                    ownedBranchFound = true;
                }
            }

            if(ownedBranchFound)
                confirmedLegalBranches.Add(GameInformation.currentRoundPlacedBranches[i]);
            else
                possibleOrphans.Add(GameInformation.currentRoundPlacedBranches[i]);
        }

        // TODO: CYCLE THROUGH POTENTIAL ORPHAN BRANCHES -- FOR EACH ONE -- IF THE BRANCH IS NOT CONNECTED TO ONE OF THE CONFIRM LEGAL BRANCHES, ITS AN ORPHAN, REMOVE IT
        List<int> orphans = new List<int>();
        for (int i = 0; i < possibleOrphans.Count; i++)
        {
            bool ownedBranchFound = false;
            int[] connectedBranches = ReferenceScript.branchConnectsToTheseBranches[possibleOrphans[i]];

            for (int j = 0; j < connectedBranches.Length; j++)
            {
                if (confirmedLegalBranches.Contains(connectedBranches[j]))
                {
                    ownedBranchFound = true;
                }
            }

            if (!ownedBranchFound)
            {
                BroadcastOrphanBranchFound(possibleOrphans[i]);
                orphans.Add(possibleOrphans[i]);
                numberRemoved++;
            }
        }

        foreach(int orphan in orphans)
        {
            GameInformation.currentRoundPlacedBranches.Remove(orphan);
        }

        return numberRemoved;
    }

    public int CheckForNodeOrphans()
    {
        List<int> confirmedLegalNodes = new List<int>();
        List<int> orphans = new List<int>();
        int numberRemoved = 0;
        for (int i = 0; i < GameInformation.currentRoundPlacedNodes.Count; i++)
        {
            bool ownedNodeFound = false;
            int[] connectedBranches = ReferenceScript.nodeConnectsToTheseBranches[GameInformation.currentRoundPlacedNodes[i]];
            for (int j = 0; j < connectedBranches.Length; j++)
            {
                if (branchEntity.gameController.getGameBoard().branches[connectedBranches[j]].branchState.ownerColor == branchEntity.gameController.getCurrentPlayerColor())
                {
                    ownedNodeFound = true;
                }
            }

            if (!ownedNodeFound)
            {
                BroadcastOrphanNodeFound(GameInformation.currentRoundPlacedNodes[i]);
                orphans.Add(GameInformation.currentRoundPlacedNodes[i]);
                numberRemoved++;
            }
        }

        foreach (int orphan in orphans)
        {
            GameInformation.currentRoundPlacedNodes.Remove(orphan);
        }

        return numberRemoved;
    }
}
