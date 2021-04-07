using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static GameObjectProperties;
using static ReferenceScript;
using static TreeNode;
using System.Threading;
using UnityEngine;
public class ExpertAI
{
    public struct MyBoard
    {
        public BoardState boardState;
        public int[] aiResources;
        public int[] playerResources;

        public MyBoard Clone()
        {
            BoardState temp = CopyBoard(boardState);
            int[] tempp = (int[])aiResources.Clone();
            int[] temppp = (int[])playerResources.Clone();
            MyBoard ttt = new MyBoard();
            ttt.aiResources = tempp;
            ttt.playerResources = temppp;
            ttt.boardState = temp;
            return ttt;
        }
    }

    static PlayerColor getCapturedSquareOwner(BoardState currentBoard, int squareId)
    {
        PlayerColor captureColor;
        // Check the surrounding branches for an owner color.
        for (int branch = 0; branch < 4; ++branch)
        {
            captureColor = currentBoard.branchStates[Reference.branchesOnSquareConnections[squareId, branch]].branchColor;
            // If found, return it.
            if (captureColor != PlayerColor.Blank)
            {
                return captureColor;
            }
        }
        // If all surrounding branches are blank, go to the square above and check it for an owner color. 
        return getCapturedSquareOwner(currentBoard, Reference.squareOnSquareConnections[squareId, 0]);
    }

    static void captureArea(BoardState currentBoard, int startSquare, List<int> possibleCaptures, List<int> captures)
    {
        List<int> blankBranches = getBlankBranches(currentBoard, startSquare);
        possibleCaptures.Remove(startSquare);
        captures.Add(startSquare);

        // For every blank branch on this captured square...
        foreach (int blankBranch in blankBranches)
        {
            // If the connected square hasn't yet been moved from possible to captured then move it.
            if (possibleCaptures.Contains(getConnectedSquare(blankBranch, startSquare)))
            {
                captureArea(currentBoard, getConnectedSquare(blankBranch, startSquare), possibleCaptures, captures);
            }
        }
    }

    static bool IsValidNodeMoves(BoardState currentBoard, PlayerColor AIcolor)
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
        if (possibleBranchMoves.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    static void ResourceTrading(int[] aiResources, int[] initialResources, BoardState currentBoardState, PlayerColor color, ref int trad)
    {
        int[] debug = (int[])aiResources.Clone();
        for (int i = 0; i < initialResources.Length; i++)
        {
            if (initialResources[i] == 0 || aiResources[0] + aiResources[1] + aiResources[2] + aiResources[3] > 8)
            {
                switch (i)
                {
                    case 0:
                        if (aiResources[i] == 0 && trad == 0)
                        {
                            if (aiResources[1] + aiResources[2] + aiResources[3] >= 3)
                            {
                                if (initialResources[1] != 0 || ((initialResources[1] == 0) && aiResources[1] > 1))
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        int max = -1;
                                        int ind = -1;
                                        for (int k = 0; k < 4; k++)
                                        {
                                            if (max < aiResources[k] && k != i)
                                            {
                                                if (aiResources[k] == 1 && k == 1)
                                                {
                                                    if (aiResources[2] == 1)
                                                    {
                                                        ind = 2;
                                                        max = aiResources[2];
                                                    }
                                                    else if (aiResources[3] == 1)
                                                    {
                                                        ind = 3;
                                                        max = aiResources[3];
                                                    }
                                                    else
                                                    {
                                                        ind = k;
                                                        max = aiResources[k];
                                                    }
                                                }
                                                else
                                                {
                                                    ind = k;
                                                    max = aiResources[k];
                                                }

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
                                                if (aiResources[k] == 1 && k == 0)
                                                {
                                                    if (aiResources[2] == 1)
                                                    {
                                                        ind = 2;
                                                        max = aiResources[2];
                                                    }
                                                    else if (aiResources[3] == 1)
                                                    {
                                                        ind = 3;
                                                        max = aiResources[3];
                                                    }
                                                    else
                                                    {
                                                        ind = k;
                                                        max = aiResources[k];
                                                    }
                                                }
                                                else
                                                {
                                                    ind = k;
                                                    max = aiResources[k];
                                                }
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
                        if (aiResources[i] < 2 && trad == 0 && IsValidNodeMoves(currentBoardState, color) == true)
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
                    case 3:
                        if (aiResources[i] < 2 && trad == 0 && IsValidNodeMoves(currentBoardState, color) == true)
                        {
                            if (aiResources[0] + aiResources[1] + aiResources[2] >= 3)
                            {
                                if (initialResources[2] != 0 || ((initialResources[2] == 0) && aiResources[2] > 2))
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

    static List<int> CalculatePossibleBranches(BoardState currentBoard, int[] aiResources, PlayerColor CurrentPlayerColor)
    {
        List<int> aiOwnedBranches = new List<int>();

        foreach (BranchState branch in currentBoard.branchStates)
        {
            if (branch.branchColor == CurrentPlayerColor || branch.ownerColor == CurrentPlayerColor)
            {
                aiOwnedBranches.Add(branch.location);
            }
        }

        List<int> possibleBranchMoves = new List<int>();

        foreach (int ownedBranch in aiOwnedBranches)
        {
            int[] connectingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];

            foreach (int branch in connectingBranches)
            {
                if (currentBoard.branchStates[branch].branchColor == PlayerColor.Blank && (currentBoard.branchStates[branch].ownerColor == PlayerColor.Blank || currentBoard.branchStates[branch].ownerColor == CurrentPlayerColor))
                {
                    possibleBranchMoves.Add(currentBoard.branchStates[branch].location);
                }
            }
        }
        if (possibleBranchMoves.Count > 0 && aiResources[0] >= 1 && aiResources[1] >= 1)
        {
            aiResources[0]--;
            aiResources[1]--;
        }
        else
        {
            possibleBranchMoves.Clear();
        }
        return possibleBranchMoves;
    }

    static List<int> CalculatePossibleNodes(BoardState currentBoard, int[] aiResources, PlayerColor CurrentPlayerColor)
    {
        List<int> aiOwnedBranches = new List<int>();

        foreach (BranchState branch in currentBoard.branchStates)
        {
            if (branch.branchColor == CurrentPlayerColor || branch.ownerColor == CurrentPlayerColor)
            {
                aiOwnedBranches.Add(branch.location);
            }
        }

        List<int> possibleNodeMoves = new List<int>();

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
        if (possibleNodeMoves.Count > 0 && aiResources[2] >= 2 && aiResources[3] >= 2)
        {
            aiResources[2] -= 2;
            aiResources[3] -= 2;
        }
        else
        {
            possibleNodeMoves.Clear();
        }
        return possibleNodeMoves;
    }

    static List<int> GetPlayersBranches(BoardState currentBoard, PlayerColor playerColor)
    {
        List<int> ownedBranches = new List<int>();

        foreach (BranchState branch in currentBoard.branchStates)
        {
            if (branch.branchColor == playerColor)
            {
                ownedBranches.Add(branch.location);
            }
        }
        return ownedBranches;
    }

    static int CalculatePlayerLongestNetwork(BoardState currentBoard, PlayerColor playerColor)
    {
        int longestNetwork = 0;
        int currentNetwork = 0;
        List<int> runningNetworkBranches = new List<int>();

        List<int> playerBranches = GetPlayersBranches(currentBoard, playerColor);

        runningNetworkBranches.Add(playerBranches[0]);
        currentNetwork++;
        foreach (int ownedBranch in playerBranches)
        {
            if (!runningNetworkBranches.Contains(ownedBranch))
            {
                longestNetwork = currentNetwork;
                currentNetwork = 0;
                runningNetworkBranches.Clear();
            }

            int[] touchingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];
            foreach (int touchedBranch in touchingBranches)
            {
                if (!runningNetworkBranches.Contains(touchedBranch) && playerBranches.Contains(touchedBranch))
                {
                    runningNetworkBranches.Add(touchedBranch);
                    currentNetwork++;
                }
            }
        }
        if (currentNetwork > longestNetwork)
            longestNetwork = currentNetwork;

        return longestNetwork;
    }

    static List<int> getBlankBranches(BoardState currentBoard, int squareId)
    {
        List<int> blankBranches = new List<int>();
        for (int branch = 0; branch < 4; ++branch)
        {
            int branchId = Reference.branchesOnSquareConnections[squareId, branch];
            if (currentBoard.branchStates[branchId].branchColor == PlayerColor.Blank)
            {
                blankBranches.Add(branchId);
            }
        }
        return blankBranches;
    }

    static int getConnectedSquare(int branchId, int squareId)
    {
        int branchDirection = -1;
        for (int i = 0; branchDirection == -1 && i < 4; ++i)
        {
            if (Reference.branchesOnSquareConnections[squareId, i] == branchId)
            {
                branchDirection = i;
            }
        }
        return Reference.squareOnSquareConnections[squareId, branchDirection];
    }

    static bool isConnectedSquareCaptured(BoardState currentBoard, int square, List<int> checkedSquares, List<int> captures, List<int> possibleCaptures)
    {
        List<int> blankBranches = getBlankBranches(currentBoard, square);
        checkedSquares.Add(square);

        foreach (int blankBranchId in blankBranches)
        {
            int connectedSquareId = getConnectedSquare(blankBranchId, square);
            if (!checkedSquares.Contains(connectedSquareId))
            {
                if (!possibleCaptures.Contains(connectedSquareId) ||
                    !isConnectedSquareCaptured(currentBoard, connectedSquareId, checkedSquares, captures, possibleCaptures))
                {
                    possibleCaptures.Remove(square);
                    return false;
                }
            }
        }
        return true;
    }

    static bool isCaptured(BoardState currentBoard, int startingSquare, List<int> captures, List<int> possibleCaptures)
    {
        List<int> blankBranches = getBlankBranches(currentBoard, startingSquare);
        List<int> checkedSquares = new List<int>();
        checkedSquares.Add(startingSquare);

        foreach (int blankBranch in blankBranches)
        {
            int connectedSquareId = getConnectedSquare(blankBranch, startingSquare);
            if (!possibleCaptures.Contains(connectedSquareId) ||
                !isConnectedSquareCaptured(currentBoard, connectedSquareId, checkedSquares, captures, possibleCaptures))
            {
                possibleCaptures.Remove(startingSquare);
                return false;
            }
        }
        return true;
    }

    static PlayerColor getOpponentColor(PlayerColor currentPlayer)
    {
        if (currentPlayer == PlayerColor.Blank)
        {
            return PlayerColor.Blank;
        }

        if (currentPlayer == PlayerColor.Silver)
        {
            return PlayerColor.Gold;
        }
        else
        {
            return PlayerColor.Silver;
        }
    }

    static void DetectMultiTileCaptures(BoardState currentBoard)
    {
        const int MAX_SQUARES = 13;
        List<int> captures = new List<int>();
        List<int> possibleCaptures = new List<int>();

        for (int currentSquare = 0; currentSquare < MAX_SQUARES; ++currentSquare)
        {
            bool couldBeCaptured = true;
            List<int> blankBranches = new List<int>();
            // The color of the first branch found on the square with a player's piece associated with it.

            PlayerColor currentCaptureColor = currentBoard.branchStates[Reference.branchesOnSquareConnections[currentSquare, 0]].branchColor;
            // Look for a player's color.
            for (int connectedBranch = 1; currentCaptureColor == PlayerColor.Blank && connectedBranch < 4; ++connectedBranch)
            {
                currentCaptureColor = currentBoard.branchStates[Reference.branchesOnSquareConnections[currentSquare, connectedBranch]].branchColor;
            }
            // If no player has placed a branch along this square then it can be captured, but only if it's surrounded by captured squares,
            //  so we'll assign it's possible color later and ignore the following for loop. 
            if (currentCaptureColor == PlayerColor.Blank)
            {
                couldBeCaptured = false;
                possibleCaptures.Add(currentSquare);
            }

            // If there is at least one branch that has a player's piece on it then check if the other branches are either blank or that color.
            for (int connectedBranch = 0; connectedBranch < 4 && couldBeCaptured; ++connectedBranch)
            {
                BranchState currentBranchState = currentBoard.branchStates[Reference.branchesOnSquareConnections[currentSquare, connectedBranch]];

                // If the node has an unclaimed branch boardering the edge of the board then it cannot be captured.
                if (currentBranchState.branchColor == PlayerColor.Blank && Reference.squareOnSquareConnections[currentSquare, connectedBranch] == -1)
                {
                    couldBeCaptured = false;
                }
                // If a connecting branch belongs to the opponent then it cannot be captured.
                else if (currentBranchState.branchColor == getOpponentColor(currentCaptureColor))
                {
                    couldBeCaptured = false;
                }
                // If a connecting branch is blank and there's a tile on the otherside, we need to check that tile for capture.
                else if (currentBranchState.branchColor == PlayerColor.Blank)
                {
                    blankBranches.Add(currentBranchState.location);
                }
                // Otherwise the branch should belong to the currentCaptureColor and nothing needs to happen.
            }

            // If the node can possibly be captured, add it to possibleCaptures.
            if (couldBeCaptured)
            {
                // If the none of the branches connected to the tile are blank, it's a single tile capture.
                if (blankBranches.Count == 0)
                {
                    captures.Add(currentSquare);
                }
                else
                {
                    possibleCaptures.Add(currentSquare);
                }
            }
        }

        // Check the list of possible captures for actual captures.
        while (possibleCaptures.Count > 0)
        {
            int square = possibleCaptures.First();
            if (isCaptured(currentBoard, square, captures, possibleCaptures))
            {
                captureArea(currentBoard, square, possibleCaptures, captures);
            }
            else
            {
                possibleCaptures.Remove(square);
            }
        }

        foreach (int squareId in captures)
        {
            PlayerColor captureColor = getCapturedSquareOwner(currentBoard, squareId);

            currentBoard.squareStates[squareId].ownerColor = captureColor;
            currentBoard.squareStates[squareId].resourceState = SquareStatus.Captured;
        }
    }

    static int GetNumberOfPlayerNodes(BoardState currentBoard, PlayerColor playerColor)
    {
        int ownedNodes = 0;

        foreach (NodeState node in currentBoard.nodeStates)
        {
            if (node.nodeColor == playerColor)
            {
                ownedNodes++;
            }
        }
        return ownedNodes;
    }

    static int GetNumberOfPlayerCapturedTiles(BoardState currentBoard, PlayerColor playerColor)
    {
        int ownedTiles = 0;
        DetectMultiTileCaptures(currentBoard);

        foreach (SquareState square in currentBoard.squareStates)
        {
            if (square.resourceState == SquareStatus.Captured && square.ownerColor == playerColor)
            {
                ownedTiles++;
            }
        }
        return ownedTiles;
    }

    static PlayerColor isEnd(BoardState currentBoard)
    {
        int playerOneScore = GetNumberOfPlayerNodes(currentBoard, PlayerColor.Silver);
        int playerTwoScore = GetNumberOfPlayerNodes(currentBoard, PlayerColor.Gold);
        playerOneScore += GetNumberOfPlayerCapturedTiles(currentBoard, PlayerColor.Silver);
        playerTwoScore += GetNumberOfPlayerCapturedTiles(currentBoard, PlayerColor.Gold);

        int playerOneNetwork = CalculatePlayerLongestNetwork(currentBoard, PlayerColor.Silver);
        int playerTwoNetwork = CalculatePlayerLongestNetwork(currentBoard, PlayerColor.Gold);

        if (playerOneNetwork > playerTwoNetwork)
        {
            playerOneScore += 2;
        }
        else if (playerOneNetwork < playerTwoNetwork)
        {
            playerTwoScore += 2;
        }

        if (playerOneScore >= 10)
        {
            return PlayerColor.Silver;
        }
        else if (playerTwoScore >= 10)
        {
            return PlayerColor.Gold;
        }
        else
        {
            return PlayerColor.Blank;
        }
    }

    static BoardState CopyBoard(BoardState myBoard)
    {
        BoardState newBoard = new BoardState();
        for (int i = 0; i < myBoard.squareStates.Length; i++)
        {
            newBoard.squareStates[i] = myBoard.squareStates[i];
        }
        for (int i = 0; i < myBoard.nodeStates.Length; i++)
        {
            newBoard.nodeStates[i] = myBoard.nodeStates[i];
        }
        for (int i = 0; i < myBoard.branchStates.Length; i++)
        {
            newBoard.branchStates[i] = myBoard.branchStates[i];
        }
        return newBoard;
    }

    //after instantiate this class, call findNextMove with time limit(5 for exopert AI) 
    public class AI
    {
        private int t; //total number of simulations
        private PlayerColor AIcolor;
        private MyBoard beginBoard;

        public AI(PlayerColor aiColor, BoardState openningBoardState, int[] aiResources, int[] playerResources)
        {
            AIcolor = aiColor;
            beginBoard.boardState = CopyBoard(openningBoardState);
            beginBoard.aiResources = aiResources;
            beginBoard.playerResources = playerResources;
        }

        private PlayerColor getcurrentPlayerColor(TreeNode node)
        {
            PlayerColor currentPlayer;
            if (node.level % 2 != 0)
            {
                currentPlayer = AIcolor;
            }
            else if (AIcolor == PlayerColor.Silver)
            {
                currentPlayer = PlayerColor.Gold;
            }
            else
            {
                currentPlayer = PlayerColor.Silver;
            }
            return currentPlayer;
        }
      
        private List<MyBoard> GetPossibleMoves(MyBoard currentBoard, PlayerColor currentPlayer, ref int trad, ref int flag_moreMoves)  // trad == 0 : haven't trade yet
        {
            List<MyBoard> newBoard = new List<MyBoard>();
            flag_moreMoves = 0;
            if (currentPlayer == AIcolor)
            {
                List<int> possibleBranches = CalculatePossibleBranches(currentBoard.boardState, currentBoard.aiResources, currentPlayer);

                for (int i = 0; i < possibleBranches.Count; i++)
                {
                    //type1: have nodes and branches
                    MyBoard temp = new MyBoard();
                    temp.aiResources = currentBoard.aiResources;
                    temp.playerResources = currentBoard.playerResources;
                    temp.boardState = CopyBoard(currentBoard.boardState);
                    temp.boardState.branchStates[possibleBranches[i]].ownerColor = currentPlayer;
                    temp.boardState.branchStates[possibleBranches[i]].branchColor = currentPlayer;
                    List<int> possibleNodes = CalculatePossibleNodes(temp.boardState, temp.aiResources, currentPlayer);
                    foreach (int j in possibleNodes)
                    {
                        MyBoard tempp = new MyBoard();
                        tempp.aiResources = currentBoard.aiResources;
                        tempp.playerResources = currentBoard.playerResources;
                        tempp.boardState = CopyBoard(temp.boardState);
                        tempp.boardState.nodeStates[j].nodeColor = currentPlayer;
                        newBoard.Add(tempp);
                        flag_moreMoves = 1;
                    }

                    //type2: no nodes
                    if (possibleNodes.Count == 0)
                    {
                        if (trad == 0)
                        {
                            
                            ResourceTrading(temp.aiResources, BeginnerAI.CollectCurrentPlayerResources(currentBoard.boardState, currentPlayer), temp.boardState, currentPlayer, ref trad);
                            if (trad == 1)
                            {
                                possibleNodes = CalculatePossibleNodes(temp.boardState, temp.aiResources, currentPlayer);
                               
                                foreach(int j in possibleNodes)
                                {
                                    MyBoard tempp = new MyBoard();
                                    tempp.aiResources = currentBoard.aiResources;
                                    tempp.playerResources = currentBoard.playerResources;
                                    tempp.boardState = CopyBoard(temp.boardState);
                                    tempp.boardState.nodeStates[j].nodeColor = currentPlayer;
                                    newBoard.Add(tempp);
                                    flag_moreMoves = 1;
                                }
                                
                            }
                        }
                        else
                        {
                            newBoard.Add(temp);
                            flag_moreMoves = 1;
                        }

                    }
                }

                if (possibleBranches.Count == 0)
                {
                    //type3: have nodes and no branches
                    MyBoard temp = new MyBoard();
                    temp.aiResources = currentBoard.aiResources;
                    temp.playerResources = currentBoard.playerResources;
                    temp.boardState = CopyBoard(currentBoard.boardState);
                    List<int> possibleNodes = CalculatePossibleNodes(temp.boardState, temp.aiResources, currentPlayer);
                    foreach (int i in possibleNodes)
                    {
                        temp.boardState.nodeStates[i].nodeColor = currentPlayer;
                        newBoard.Add(temp);
                        flag_moreMoves = 1;
                    }
                    //type4: no branch, no node
                    if (possibleNodes.Count == 0)
                    {
                        if (trad == 0)
                        {
                            ResourceTrading(temp.aiResources, BeginnerAI.CollectCurrentPlayerResources(currentBoard.boardState, currentPlayer), temp.boardState, currentPlayer, ref trad);
                            possibleNodes = CalculatePossibleNodes(temp.boardState, temp.aiResources, currentPlayer);
                            if (possibleNodes.Count == 0)
                            {
                                possibleBranches = CalculatePossibleBranches(temp.boardState, temp.aiResources, currentPlayer);
                                possibleNodes = CalculatePossibleNodes(temp.boardState, temp.aiResources, currentPlayer);
                                foreach (int i in possibleBranches)
                                {
                                    temp.boardState.branchStates[possibleBranches[i]].ownerColor = currentPlayer;
                                    temp.boardState.branchStates[possibleBranches[i]].branchColor = currentPlayer;
                                    newBoard.Add(temp);
                                    flag_moreMoves = 1;
                                }

                            }
                            else
                            {
                                foreach (int i in possibleNodes)
                                {
                                    temp.boardState.nodeStates[i].nodeColor = currentPlayer;
                                    newBoard.Add(temp);
                                    flag_moreMoves = 1;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                List<int> possibleBranches = CalculatePossibleBranches(currentBoard.boardState, currentBoard.playerResources, currentPlayer);

                for (int i = 0; i < possibleBranches.Count; i++)
                {
                    //type1: have nodes and branches
                    MyBoard temp = new MyBoard();
                    temp.aiResources = currentBoard.aiResources;
                    temp.playerResources = currentBoard.playerResources;
                    temp.boardState = CopyBoard(currentBoard.boardState);
                    temp.boardState.branchStates[possibleBranches[i]].ownerColor = currentPlayer;
                    temp.boardState.branchStates[possibleBranches[i]].branchColor = currentPlayer;
                    List<int> possibleNodes = CalculatePossibleNodes(temp.boardState, temp.playerResources, currentPlayer);
                    foreach (int j in possibleNodes)
                    {
                        MyBoard tempp = new MyBoard();
                        tempp.aiResources = currentBoard.aiResources;
                        tempp.playerResources = currentBoard.playerResources;
                        tempp.boardState = CopyBoard(temp.boardState);
                        tempp.boardState.nodeStates[j].nodeColor = currentPlayer;
                        newBoard.Add(tempp);
                        flag_moreMoves = 1;
                    }

                    //type2: no nodes
                    if (possibleNodes.Count == 0)
                    {
                        if (trad == 0)
                        {
                            ResourceTrading(temp.playerResources, BeginnerAI.CollectCurrentPlayerResources(currentBoard.boardState, currentPlayer), temp.boardState, currentPlayer, ref trad);
                            if (trad == 1)
                            {
                                possibleNodes = CalculatePossibleNodes(temp.boardState, temp.playerResources, currentPlayer);
                              
                                foreach (int j in possibleNodes)
                                {
                                    MyBoard tempp = new MyBoard();
                                    tempp.aiResources = currentBoard.aiResources;
                                    tempp.playerResources = currentBoard.playerResources;
                                    tempp.boardState = CopyBoard(temp.boardState);
                                    tempp.boardState.nodeStates[j].nodeColor = currentPlayer;
                                    newBoard.Add(tempp);
                                    flag_moreMoves = 1;
                                }
                            }
                        }
                        else
                        {
                            newBoard.Add(temp);
                            flag_moreMoves = 1;
                        }

                    }
                }

                if (possibleBranches.Count == 0)
                {
                    //type3: have nodes and no branches
                    MyBoard temp = new MyBoard();
                    temp.aiResources = currentBoard.aiResources;
                    temp.playerResources = currentBoard.playerResources;
                    temp.boardState = CopyBoard(currentBoard.boardState);
                    List<int> possibleNodes = CalculatePossibleNodes(temp.boardState, temp.playerResources, currentPlayer);
                    foreach (int i in possibleNodes)
                    {
                        temp.boardState.nodeStates[i].nodeColor = currentPlayer;
                        newBoard.Add(temp);
                        flag_moreMoves = 1;
                    }
                    //type4: no branch, no node
                    if (possibleNodes.Count == 0)
                    {
                        if (trad == 0)
                        {

                            ResourceTrading(temp.playerResources, BeginnerAI.CollectCurrentPlayerResources(currentBoard.boardState, currentPlayer), temp.boardState, currentPlayer, ref trad);
                            possibleNodes = CalculatePossibleNodes(temp.boardState, temp.playerResources, currentPlayer);
                            if (possibleNodes.Count == 0)
                            {
                                possibleBranches = CalculatePossibleBranches(temp.boardState, temp.playerResources, currentPlayer);
                                possibleNodes = CalculatePossibleNodes(temp.boardState, temp.playerResources, currentPlayer);
                                foreach (int i in possibleBranches)
                                {
                                    temp.boardState.branchStates[possibleBranches[i]].ownerColor = currentPlayer;
                                    temp.boardState.branchStates[possibleBranches[i]].branchColor = currentPlayer;
                                    newBoard.Add(temp);
                                    flag_moreMoves = 1;
                                }
                                if (possibleBranches.Count == 0)
                                {
                                    newBoard.Add(temp);
                                }

                            }
                            else
                            {
                                foreach (int i in possibleNodes)
                                {
                                    temp.boardState.nodeStates[i].nodeColor = currentPlayer;
                                    newBoard.Add(temp);
                                    flag_moreMoves = 1;
                                }
                            }
                        }
                    }
                }
            }

            if (flag_moreMoves == 1)
            {
                List<MyBoard> res = new List<MyBoard>();
                foreach (MyBoard i in newBoard)
                {
                    List<MyBoard> temp = GetPossibleMoves(i, currentPlayer, ref trad, ref flag_moreMoves);
                    if(temp != null)
                    {
                        res = res.Union(temp).ToList();
                    }
                }
                return res;

            }
            else
            {
                return null;
            }


        }

        private TreeNode traverse(TreeNode root)
        {
            TreeNode currentNode = root;
            while(currentNode.child.Count != 0)
            {
                currentNode = UCT.findBestNode(currentNode);
            }
            return currentNode;
        }

        private void expand(TreeNode node)
        {
            int trad = 0;
            int flag_moreMoves = 0;
            PlayerColor currentPlayer = getcurrentPlayerColor(node);
            List<MyBoard> temp = GetPossibleMoves(node.localBoard, currentPlayer, ref trad, ref flag_moreMoves);
            foreach(MyBoard i in temp)
            {
                node.AddChild(i);
            }
        }

        private void backpropgation(TreeNode node, PlayerColor winner)
        {
            TreeNode temp = node;
            while(temp != null)
            {
                if(winner == AIcolor)
                {
                    temp.W++;
                }
                temp.N++;
                temp = temp.parent;
            }
        }

        private PlayerColor simulation(TreeNode node)
        {
            PlayerColor winner = PlayerColor.Blank;
            TreeNode temp = node.Copy();
            PlayerColor playerCol= getcurrentPlayerColor(node);
            PlayerColor otherCol;
            while ( winner == PlayerColor.Blank)
            {
                
                if (playerCol == PlayerColor.Silver)
                {
                    otherCol = PlayerColor.Gold;
                }
                else
                {
                    otherCol = PlayerColor.Silver;
                }

                if(AIcolor == playerCol)
                {
                    BeginnerAI tempp = new BeginnerAI(playerCol, temp.localBoard.boardState);
                    temp.localBoard.boardState = tempp.RandomMoveForMCTS(temp.localBoard.boardState, temp.localBoard.aiResources);
                    int[] res = BeginnerAI.CollectCurrentPlayerResources(temp.localBoard.boardState, otherCol);
                    for (int i = 0; i < 4; i++)
                    {
                        temp.localBoard.playerResources[i] += res[i];
                    }
                    
                }
                else
                {
                    BeginnerAI tempp = new BeginnerAI(playerCol, temp.localBoard.boardState);
                    temp.localBoard.boardState = tempp.RandomMoveForMCTS(temp.localBoard.boardState, temp.localBoard.playerResources);
                    int[] res = BeginnerAI.CollectCurrentPlayerResources(temp.localBoard.boardState, otherCol);
                    for (int i = 0; i < 4; i++)
                    {
                        temp.localBoard.aiResources[i] += res[i];
                    }
                }
                DetectMultiTileCaptures(temp.localBoard.boardState);
                winner = isEnd(temp.localBoard.boardState);
                if(playerCol == PlayerColor.Silver)
                {
                    playerCol = PlayerColor.Gold;
                }
                else
                {
                    playerCol = PlayerColor.Silver;
                }
            }
            return winner;
        }

        public BoardState findNextMove(int timeLimit) // timeLimit = 5 means 5 seconds
        {
            BoardState best = new BoardState();
            DateTime beforDT = System.DateTime.Now;
            TimeSpan t = TimeSpan.FromSeconds(timeLimit);
            TreeNode root = new TreeNode(beginBoard);
            bool timeOut = false;
            while (timeOut == false)
            {
                int max = -1;
                int loc = 0;
                TreeNode promisingNode = traverse(root);
                if(promisingNode.N == 0)
                {
                    PlayerColor winner = simulation(promisingNode);
                    backpropgation(promisingNode, winner);
                }
                else
                {
                    expand(promisingNode);
                    if(promisingNode.child.Count == 0)
                    {
                        Debug.Log("Error: promisingNode.child.Count = 0");
                    }
                    promisingNode = promisingNode.child[0];
                    PlayerColor winner = simulation(promisingNode);
                    backpropgation(promisingNode, winner);
                }
                for(int i = 0; i < root.child.Count; i++)
                {
                    if(root.child[i].N > max)
                    {
                        loc = i;
                    }
                }
                best = root.child[loc].localBoard.boardState;
                DateTime afterDT = System.DateTime.Now;
                TimeSpan ts = afterDT.Subtract(beforDT);
                if(ts >= t)
                {
                    timeOut = true;
                    Console.WriteLine(ts);
                    return best;
                }
            }
            return best;
        }
    }


    public class UCT
    {
        public static TreeNode findBestNode(TreeNode node)
        {
            List<double> score = new List<double>();
            for(int i = 0; i < node.child.Count; i++)
            {
                if(node.child[i].N == 0)
                {
                    score.Add(99999);
                }
                else
                {
                    score.Add((node.W / node.N) + Math.Sqrt(2) * Math.Sqrt(Math.Log(Math.E, node.parent.N))/node.N);
                }
            }
            double max = 0;
            int loc = -1;
            for(int i = 0; i < score.Count; i++)
            {
                if(max < score[i])
                {
                    loc = i;
                    max = score[i];
                }
            }
            return node.child[loc];
        }
    }



}
