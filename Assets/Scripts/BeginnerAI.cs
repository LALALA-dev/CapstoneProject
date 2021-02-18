using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class BeginnerAI
{
    private PlayerColor AIcolor;
    private BoardState currentBoardState;

    public BeginnerAI(PlayerColor aiColor, BoardState openningBoardState)
    {
        AIcolor = aiColor;
        currentBoardState = openningBoardState;
    }

    private List<BoardState> CalculatePossibleMoves(GameBoard currentBoard, int[] aiResources)
    {
        List<BoardState> moves = new List<BoardState>();
        List<Branch> aiOwnedBranches = new List<Branch>();
   
        foreach(Branch branch in currentBoard.branches)
        {
            if(branch.branchState.branchColor == AIcolor || branch.branchState.ownerColor == AIcolor)
            {
                aiOwnedBranches.Add(branch);
            }
        }

        List<Branch> possibleBranchMoves = new List<Branch>();
        List<Branch> possibleNodeMoves = new List<Branch>();

        if(aiResources[0] >= 1 && aiResources[1] >= 1)
        {
            foreach(Branch ownedBranch in aiOwnedBranches)
            {
                int[] connectingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch.id];

                foreach(int branch in connectingBranches)
                {
                    if(currentBoard.getBoardState().branchStates[branch].branchColor == PlayerColor.Blank)
                    {
                        possibleBranchMoves.Add(currentBoard.branches[branch]);
                    }
                }
            }
        }

        if (aiResources[2] >= 2 && aiResources[3] >= 2)
        {
            // ADD ALL POSSIBLE NODES
        }

        for(int i = 0; i < possibleBranchMoves.Count; i++)
        {
            BoardState newMove = currentBoard.getBoardState();
            newMove.branchStates[possibleBranchMoves[i].id].branchColor = AIcolor;
            newMove.branchStates[possibleBranchMoves[i].id].ownerColor = AIcolor;

            moves.Add(newMove);
        }

        return moves;
    }

    public BoardState MakeRandomOpeningMove()
    {
        BoardState result = new BoardState();

        if (GameInformation.openingSequence)
        {
            List<NodeState> unownedNodes = new List<NodeState>();
            foreach (NodeState i in currentBoardState.nodeStates)
            {
                if (i.nodeColor == PlayerColor.Blank)
                {
                    unownedNodes.Add(i);
                }
            }

            result = OpeningMove(unownedNodes);
        }
        else
        {
            result = RandomMove();
        }
        return result;
    }

    private BoardState OpeningMove(List<NodeState> possibleMoves)
    {
        BoardState result = currentBoardState;

        TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        var rand = new System.Random(t.Seconds);
        int index = rand.Next(possibleMoves.Count);
        result.nodeStates[possibleMoves[index].location].nodeColor = AIcolor;

        int[] connectedBranches = ReferenceScript.nodeConnectsToTheseBranches[index];
        List<int> branchChoices = new List<int>();

        foreach(int branch in connectedBranches)
            if (currentBoardState.branchStates[branch].branchColor == PlayerColor.Blank)
                branchChoices.Add(branch);

        index = rand.Next(branchChoices.Count);
        result.branchStates[branchChoices[index]].branchColor = AIcolor;
        result.branchStates[branchChoices[index]].ownerColor = AIcolor;

        currentBoardState = result;
        return result;
    }
    private BoardState RandomMove()
    {
        List<BoardState> possibleMoves = new List<BoardState>(); // CalculatePossibleMoves(currentBoard, aiResources);

        TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        var rand = new System.Random(t.Seconds);

        return possibleMoves[rand.Next(possibleMoves.Count)]; 
    }
}
