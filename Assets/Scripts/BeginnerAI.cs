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

    private List<BoardState> CalculatePossibleMoves(BoardState currentBoard, int[] aiResources)
    {
        List<BoardState> moves = new List<BoardState>();
        List<int> aiOwnedBranches = new List<int>();
   
        foreach(BranchState branch in currentBoard.branchStates)
        {
            if(branch.branchColor == AIcolor || branch.ownerColor == AIcolor)
            {
                aiOwnedBranches.Add(branch.location);
            }
        }

        List<int> possibleBranchMoves = new List<int>();
        List<Branch> possibleNodeMoves = new List<Branch>();

        if(aiResources[0] >= 1 && aiResources[1] >= 1)
        {
            GameInformation.playerTwoResources[0]--;
            GameInformation.playerTwoResources[1]--;
            foreach(int ownedBranch in aiOwnedBranches)
            {
                int[] connectingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];

                foreach(int branch in connectingBranches)
                {
                    if(currentBoard.branchStates[branch].branchColor == PlayerColor.Blank)
                    {
                        possibleBranchMoves.Add(currentBoard.branchStates[branch].location);
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
            BoardState newMove = currentBoard;
            newMove.branchStates[possibleBranchMoves[i]].branchColor = AIcolor;
            newMove.branchStates[possibleBranchMoves[i]].ownerColor = AIcolor;

            moves.Add(newMove);
        }
        return moves;
    }

    public BoardState MakeRandomOpeningMove(BoardState currentBoard)
    {
        currentBoardState = currentBoard;
        BoardState result = new BoardState();

        if (GameInformation.openingSequence)
        {
            List<NodeState> unownedNodes = new List<NodeState>();
            foreach (NodeState i in currentBoard.nodeStates)
            {
                if (i.nodeColor == PlayerColor.Blank)
                {
                    unownedNodes.Add(i);
                }
            }
            result = OpeningMove(unownedNodes);
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

        int[] connectedBranches = ReferenceScript.nodeConnectsToTheseBranches[possibleMoves[index].location];
        List<int> branchChoices = new List<int>();

        foreach(int branch in connectedBranches)
            if (currentBoardState.branchStates[branch].branchColor == PlayerColor.Blank)
                branchChoices.Add(branch);

        rand = new System.Random(t.Seconds);
        index = rand.Next(branchChoices.Count);
        result.branchStates[branchChoices[index]].branchColor = AIcolor;
        result.branchStates[branchChoices[index]].ownerColor = AIcolor;

        currentBoardState = result;
        return result;
    }
    public BoardState RandomMove(BoardState currentBoard, int[] aiResources)
    {
        currentBoardState = currentBoard;
        List<BoardState> possibleMoves = CalculatePossibleMoves(currentBoard, aiResources);

        TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        var rand = new System.Random(t.Seconds);

        if (possibleMoves.Count > 0)
        {
            int index = rand.Next(possibleMoves.Count);
            return possibleMoves[index];
        }
        else
            return currentBoard;
    }
}
