using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class BeginnerAI
{
    public PlayerColor AIcolor;
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
        List<int> possibleNodeMoves = new List<int>();

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
            GameInformation.playerTwoResources[2] -= 2;
            GameInformation.playerTwoResources[3] -= 2;

            foreach (int ownedBranch in aiOwnedBranches)
            {
                int[] connectingNodes = ReferenceScript.branchesConnectToTheseNodes[ownedBranch];

                foreach (int node in connectingNodes)
                {
                    if (currentBoard.nodeStates[node].nodeColor == PlayerColor.Blank)
                    {
                        possibleNodeMoves.Add(currentBoard.nodeStates[node].location);
                    }
                }
            }
        }

        if(aiResources[0] < 1 && aiResources[1] < 1 && aiResources[2] < 2 && aiResources[3] < 2)
        {
            // Trade resource
        }

        for(int i = 0; i < possibleBranchMoves.Count; i++)
        {
            BoardState newMove = new BoardState();
            newMove.branchStates = new BranchState[36];
            newMove.nodeStates = new NodeState[24];
            newMove.squareStates = new SquareState[13];

            for(int j = 0; j < 36; j++)
            {
                newMove.branchStates[j].branchColor = currentBoard.branchStates[j].branchColor;
                newMove.branchStates[j].ownerColor = currentBoard.branchStates[j].ownerColor;
                newMove.branchStates[j].location = j;
            }
            for (int j = 0; j < 24; j++)
            {
                newMove.nodeStates[j].nodeColor = currentBoard.nodeStates[j].nodeColor;
                newMove.nodeStates[j].location = j;
            }

            newMove.squareStates = currentBoard.squareStates;

            BranchState newBranch = new BranchState();
            newBranch.branchColor = AIcolor;
            newBranch.ownerColor = AIcolor;
            newBranch.location = possibleBranchMoves[i];

            newMove.branchStates[possibleBranchMoves[i]] = newBranch;

            moves.Add(newMove);
        }

        for (int i = 0; i < possibleNodeMoves.Count; i++)
        {
            BoardState newMove = new BoardState();
            newMove.branchStates = new BranchState[36];
            newMove.nodeStates = new NodeState[24];
            newMove.squareStates = new SquareState[13];

            for (int j = 0; j < 36; j++)
            {
                newMove.branchStates[j].branchColor = currentBoard.branchStates[j].branchColor;
                newMove.branchStates[j].ownerColor = currentBoard.branchStates[j].ownerColor;
                newMove.branchStates[j].location = j;
            }
            for (int j = 0; j < 24; j++)
            {
                newMove.nodeStates[j].nodeColor = currentBoard.nodeStates[j].nodeColor;
                newMove.nodeStates[j].location = j;
            }

            newMove.squareStates = currentBoard.squareStates;

            NodeState newNode = new NodeState();
            newNode.nodeColor = AIcolor;
            newNode.location = possibleNodeMoves[i];

            newMove.nodeStates[possibleNodeMoves[i]] = newNode;

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
