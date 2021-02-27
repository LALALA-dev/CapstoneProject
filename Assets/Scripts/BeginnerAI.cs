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

    private List<int> CalculatePossibleBranches(BoardState currentBoard, int[] aiResources)
    {
        List<int> aiOwnedBranches = new List<int>();

        foreach (BranchState branch in currentBoard.branchStates)
        {
            if (branch.branchColor == AIcolor || branch.ownerColor == AIcolor)
            {
                aiOwnedBranches.Add(branch.location);
            }
        }

        List<int> possibleBranchMoves = new List<int>();

        if (aiResources[0] >= 1 && aiResources[1] >= 1)
        {
            if(GameInformation.playerIsHost)
            {
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;
            }
            else
            {
                GameInformation.playerOneResources[0]--;
                GameInformation.playerOneResources[1]--;
            }
            
            foreach (int ownedBranch in aiOwnedBranches)
            {
                int[] connectingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];

                foreach (int branch in connectingBranches)
                {
                    if (currentBoard.branchStates[branch].branchColor == PlayerColor.Blank && (currentBoard.branchStates[branch].ownerColor == PlayerColor.Blank || currentBoard.branchStates[branch].ownerColor == AIcolor))
                    {
                        possibleBranchMoves.Add(currentBoard.branchStates[branch].location);
                    }
                }
            }
        }
        return possibleBranchMoves;
    }

    private List<int> CalculatePossibleNodes(BoardState currentBoard, int[] aiResources)
    {
        List<int> aiOwnedBranches = new List<int>();

        foreach (BranchState branch in currentBoard.branchStates)
        {
            if (branch.branchColor == AIcolor || branch.ownerColor == AIcolor)
            {
                aiOwnedBranches.Add(branch.location);
            }
        }

        List<int> possibleNodeMoves = new List<int>();

        if (aiResources[2] >= 2 && aiResources[3] >= 2)
        {
            if(GameInformation.playerIsHost)
            {
                GameInformation.playerTwoResources[2] -= 2;
                GameInformation.playerTwoResources[3] -= 2;
            }
            else
            {
                GameInformation.playerOneResources[2] -= 2;
                GameInformation.playerOneResources[3] -= 2;
            }

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
        return possibleNodeMoves;
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
        var rand = new System.Random();
        int index = rand.Next(possibleMoves.Count);

        //*********************
        List<double> temp = evaluateBoardStatus(result);
        double max = -1;
        int loc = -1;
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i] >= max)
            {
                loc = i;
                max = temp[i];

            }
        }

        //*********************
        result.nodeStates[loc].nodeColor = AIcolor;
        int[] connectedBranche = ReferenceScript.nodeConnectsToTheseBranches[loc];
        int[] connectedBranches = new int[4];
        for (int i = 0, j = 0; i < connectedBranche.Length; i++)
        {

            if (result.branchStates[i].ownerColor == PlayerColor.Blank)
            {
                connectedBranches[j] = connectedBranche[i];
                j++;
            }
        }

        rand = new System.Random(t.Seconds);
        index = rand.Next(0,connectedBranches.Length);
        result.branchStates[connectedBranches[index]].branchColor = AIcolor;
        result.branchStates[connectedBranches[index]].ownerColor = AIcolor;

        currentBoardState = result;
        return result;
    }

    private BoardState subRandomMove(BoardState currentBoard, int[] aiResources,ref int flag)
    {
        List<int> possibleBranchMoves = CalculatePossibleBranches(currentBoard, aiResources);
        System.Random rand = new System.Random();

        if (possibleBranchMoves.Count > 0)
        {
            int index = rand.Next(0, possibleBranchMoves.Count);
            currentBoard.branchStates[possibleBranchMoves[index]].ownerColor = AIcolor;
            currentBoard.branchStates[possibleBranchMoves[index]].branchColor = AIcolor;
            flag = 1;
            List<int> possibleNodeMoves = CalculatePossibleNodes(currentBoard, aiResources);
            if (possibleNodeMoves.Count > 0)
            {
                index = rand.Next(0, possibleNodeMoves.Count);
                currentBoard.nodeStates[possibleNodeMoves[index]].nodeColor = AIcolor;
                flag = 1;
                DetectLocalTileOverloads(currentBoard, possibleNodeMoves[index]);
            }

        }
        else
        {
            List<int> possibleNodeMoves = CalculatePossibleNodes(currentBoard, aiResources);
            if (possibleNodeMoves.Count > 0)
            {
                int index = rand.Next(0, possibleNodeMoves.Count);
                currentBoard.nodeStates[possibleNodeMoves[index]].nodeColor = AIcolor;
                flag = 1;
                DetectLocalTileOverloads(currentBoard, possibleNodeMoves[index]);
            }
        }
        return currentBoard;
    }

    private void ResourceTraiding(int[] aiResources, int[] initialResources)
    {
        int trad = 0;
        for (int i = 0; i < initialResources.Length; i++)
        {
            if (initialResources[i] == 0 || aiResources[0] + aiResources[1] + aiResources[2] + aiResources[3]>8)
            {
                switch (i)
                {
                    case 0:
                        if (aiResources[i] == 0 && trad == 0) 
                        {
                            if (aiResources[1] + aiResources[2] + aiResources[3] >= 3)
                            {
                                if (initialResources[2] != 0 || ((initialResources[2] == 0) && aiResources[2] > 1))
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        int max = -1;
                                        int ind = -1;
                                        for (int k = 0; k < 4; k++)
                                        {
                                            if(max < aiResources[k] && k != i)
                                            {
                                                ind = k;
                                                max = aiResources[k];
                                            }
                                        }
                                        aiResources[ind]--;
                                    }
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                        }
                        break;
                    case 1:
                        if (aiResources[i] < 2 && trad == 0)
                        {
                            if (aiResources[0] + aiResources[2] + aiResources[3] >= 3)
                            {
                                if (initialResources[3] != 0 || ((initialResources[3] == 0) && aiResources[3] > 2))
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        int max = -1;
                                        int ind = -1;
                                        for (int k = 0; k < 4; k++)
                                        {
                                            if (max < aiResources[k] && k != i)
                                            {
                                                ind = k;
                                                max = aiResources[k];
                                            }
                                        }
                                        aiResources[ind]--;
                                    }
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                        }
                        break;
                    case 2:
                        if (aiResources[i] == 0 && trad == 0)
                        {
                            if (aiResources[0] + aiResources[1] + aiResources[3] >= 3)
                            {
                                if (initialResources[0] != 0 || ((initialResources[0] == 0) && aiResources[0] > 1))
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        int max = -1;
                                        int ind = -1;
                                        for (int k = 0; k < 4; k++)
                                        {
                                            if (max < aiResources[k] && k != i)
                                            {
                                                ind = k;
                                                max = aiResources[k];
                                            }
                                        }
                                        aiResources[ind]--;
                                    }
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                        }
                        break;
                    case 3:
                        if (aiResources[i] < 2 && trad == 0)
                        {
                            if (aiResources[0] + aiResources[1] + aiResources[2] >= 3)
                            {
                                if (initialResources[1] != 0 || ((initialResources[1] == 0) && aiResources[1] > 2))
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        int max = -1;
                                        int ind = -1;
                                        for (int k = 0; k < 4; k++)
                                        {
                                            if (max < aiResources[k] && k != i)
                                            {
                                                ind = k;
                                                max = aiResources[k];
                                            }
                                        }
                                        aiResources[ind]--;
                                    }
                                    aiResources[i]++;
                                    trad = 1;
                                }
                            }
                        }
                        break;
                }
            }
        }

    }

    public BoardState RandomMove(BoardState currentBoard, int[] aiResources)
    {
        int[] initialResources = CollectCurrentPlayerResources(currentBoard, AIcolor);
        Debug.Log(initialResources[0]+ " " + initialResources[1]+" " + initialResources[2] + " " + initialResources[3]);
        int flag = 0;
        currentBoard = subRandomMove(currentBoard, aiResources, ref flag);
        ResourceTraiding(aiResources, initialResources);
        while (flag == 1)
        {
            flag = 0;;
            currentBoard = subRandomMove(currentBoard, aiResources, ref flag);
        }
        return currentBoard;
    }

    //********************************************
    static public int[] CollectCurrentPlayerResources(BoardState gameBoard, PlayerColor CurrentPlayerColor)
    {
        List<NodeState> currentNodes = new List<NodeState>();
        foreach (NodeState node in gameBoard.nodeStates)
        {
            if (node.nodeColor == CurrentPlayerColor)
                currentNodes.Add(node);
        }

        List<SquareState> squares = new List<SquareState>();
        foreach (NodeState node in currentNodes)
        {
            foreach (int squareId in ReferenceScript.nodeConnectToTheseTiles[node.location])
            {
                if (gameBoard.squareStates[squareId].resourceState == SquareStatus.Open || (gameBoard.squareStates[squareId].ownerColor == CurrentPlayerColor))
                    squares.Add(gameBoard.squareStates[squareId]);
            }
        }

        int[] resources = new int[4] { 0, 0, 0, 0 };
        foreach (SquareState square in squares)
        {
            switch (square.resourceColor)
            {
                case SquareResourceColor.Red:
                    resources[0]++;
                    break;
                case SquareResourceColor.Yellow:
                    resources[1]++;
                    break;
                case SquareResourceColor.Blue:
                    resources[2]++;
                    break;
                case SquareResourceColor.Green:
                    resources[3]++;
                    break;
                default:
                    break;
            }
        }
        return resources;
    }
    //********************************************

    public void DetectLocalTileOverloads(BoardState currentBoarrd, int currentNodeLocation)
    {
        foreach (int loc in nodeConnectToTheseTiles[currentNodeLocation])
        {
            int i = loc;
            int numberOwnedNodes = 0;
            if (currentBoarrd.squareStates[i].resourceState == SquareStatus.Open)
            {
                bool isBlocked = false;
                int[] connectedNodes = ReferenceScript.tileConnectsToTheseNodes[i];

                foreach (int node in connectedNodes)
                {
                    if (currentBoarrd.nodeStates[node].nodeColor != PlayerColor.Blank)
                        numberOwnedNodes++;
                }

                switch (currentBoarrd.squareStates[i].resourceAmount)
                {
                    case SquareResourceAmount.One:
                        if (numberOwnedNodes >= 2)
                            isBlocked = true;
                        break;
                    case SquareResourceAmount.Two:
                        if (numberOwnedNodes >= 3)
                            isBlocked = true;
                        break;
                    case SquareResourceAmount.Three:
                        if (numberOwnedNodes >= 4)
                            isBlocked = true;
                        break;
                }

                if (isBlocked)
                {
                    currentBoarrd.squareStates[i].resourceState = SquareStatus.Blocked;
                }
            }

        }

    }
}
