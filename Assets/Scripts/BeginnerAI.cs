using System;
using System.Collections;
using System.Collections.Generic;
using static GameObjectProperties;
using UnityEngine;
using static ReferenceScript;
public class BeginnerAI

{
    private PlayerColor AIcolor;
    private BoardState currentBoardState;

    public BeginnerAI(PlayerColor aiColor, BoardState openningBoardState)
    {
        AIcolor = aiColor;
        currentBoardState = openningBoardState;
    }

    private List<double> evaluateBoardStatus(BoardState currentBoard)
    {
        List<double> res = new List<double>();
        foreach (NodeState temp in currentBoard.nodeStates)
        {
            
            double result = 0;
            int[] col = { 0, 0, 0, 0 };

            foreach (int tile in ReferenceScript.nodeConnectToTheseTiles[temp.location])
            {
               
                SquareResourceAmount flag = SquareResourceAmount.Blank;
                
                    foreach (int connectedNode in ReferenceScript.tileConnectsToTheseNodes[tile])
                    {
                        
                        if (currentBoard.nodeStates[connectedNode].nodeColor != PlayerColor.Blank)
                            {
                                flag++;
                            }
                    }
                if (flag < currentBoard.squareStates[tile].resourceAmount)
                {

                    if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Red)
                    {
                        // Debug.Log("1");
                        col[0]++;
                    }
                    if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Yellow)
                    {
                        col[1]++;
                        //   Debug.Log("2");
                    }
                    if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Blue)
                    {
                        col[2]++;
                        //    Debug.Log("3");
                    }
                    if (currentBoard.squareStates[tile].resourceColor == SquareResourceColor.Green)
                    {
                        col[3]++;
                        // Debug.Log("4");
                    }
                }

                flag = 0;                  
                
                  
            }
            if (col[0] > 0 && col[2] > 0)
            {
                if (col[0] > 1 && col[2] > 1)
                {
                    result += (1.8 + 1.8);
                }
                else
                {
                    result += (1.5 + 1.5);
                }
                
                if (col[1] > 0 && col[3] > 0)
                {
                    result += (2 + 2);
                }

            }
            else if (col[1] > 0 && col[3] > 0)
            {
                if (col[1] > 1 && col[3] > 1)
                {
                    result += (1.8 + 1.8);
                }
                else
                {
                    result += (1.5 + 1.5);
                }

            }
            for(int i = 0; i < 4; i++)
            {
                if(col[i] > 0)
                {
                    result += 1;
                }
            }
           // Debug.Log(temp.location + ": " + result);
            Debug.Log("i: " + temp.location + " color: " + col[0] + " " + col[1] + " " + col[2] + " " + col[3]);
            if (temp.nodeColor == PlayerColor.Blank)
            {
                res.Add(result);
            }
            else
            {
                res.Add(-1);
            }

            result = 0;
        }
        
        return res;
    }

    private List<BoardState> CalculatePossibleMoves(BoardState currentBoard, int[] aiResources)
    {
        List<BoardState> moves = new List<BoardState>();
        List<int> aiOwnedBranches = new List<int>();

        foreach (BranchState branch in currentBoard.branchStates)
        {
            if (branch.branchColor == AIcolor || branch.ownerColor == AIcolor)
            {
                aiOwnedBranches.Add(branch.location);
            }
        }

        List<int> possibleBranchMoves = new List<int>();
        List<int> possibleNodeMoves = new List<int>();

        if (aiResources[0] >= 1 && aiResources[1] >= 1)
        {
            GameInformation.playerTwoResources[0]--;
            GameInformation.playerTwoResources[1]--;
            foreach (int ownedBranch in aiOwnedBranches)
            {
                int[] connectingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];

                foreach (int branch in connectingBranches)
                {
                    if (currentBoard.branchStates[branch].branchColor == PlayerColor.Blank)
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

        if (aiResources[0] < 1 && aiResources[1] < 1 && aiResources[2] < 2 && aiResources[3] < 2)
        {
            // Trade resource
        }

        for (int i = 0; i < possibleBranchMoves.Count; i++)
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

        //*********************
        List<double> temp = evaluateBoardStatus(result);
        double max = -1;
        int loc = -1;
        for (int i = 0; i < temp.Count; i++)
        {
            //Debug.Log(i+ ": " + temp[i]);
            if (temp[i] >= max)
            {
                loc = i;
                max = temp[i];

            }
        }

        //*********************
        //result.nodeStates[possibleMoves[index].location].nodeColor = AIcolor;
        result.nodeStates[loc].nodeColor = AIcolor;
        //int[] connectedBranches = ReferenceScript.nodeConnectsToTheseBranches[possibleMoves[index].location];
        int[] connectedBranches = ReferenceScript.nodeConnectsToTheseBranches[loc];
        List<int> branchChoices = new List<int>();

        foreach (int branch in connectedBranches)
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
        DateTime beforDT = System.DateTime.Now;
        currentBoardState = currentBoard;
        List<BoardState> possibleMoves = CalculatePossibleMoves(currentBoard, aiResources);

        TimeSpan t = (DateTime.UtcNow - new DateTime(1970, 1, 1));

        var rand = new System.Random(t.Seconds);

        if (possibleMoves.Count > 0)
        {

            int index = rand.Next(possibleMoves.Count);

            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforDT);
            // Debug.Log(ts);
            //   Debug.Log("beforDT: "+ beforDT + "\nafterDT: " + afterDT);
            return possibleMoves[index];
        }
        else
        {
            DateTime afterDT = System.DateTime.Now;
            TimeSpan ts = afterDT.Subtract(beforDT);
            // Debug.Log(ts);
            //  Debug.Log("beforDT: " + beforDT + "\nafterDT: " + afterDT);

            return currentBoard;
        }

    }
}
