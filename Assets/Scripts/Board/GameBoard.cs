using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class GameBoard
{
    private const int MAX_SQUARES = 13;
    private const int MAX_NODES = 24;
    private const int MAX_BRANCHES = 36;

    public Square[] squares;
    public Node[] nodes;
    public Branch[] branches;

    private BoardState boardState;

    public GameBoard()
    {
        squares = new Square[MAX_SQUARES];
        nodes = new Node[MAX_NODES];
        branches = new Branch[MAX_BRANCHES];

        boardState.squareStates = new SquareState[MAX_SQUARES];
        boardState.nodeStates = new NodeState[MAX_NODES];
        boardState.branchStates = new BranchState[MAX_BRANCHES];
    }

    public void setSquares(SquareState[] squareStates)
    {
        int i = 0;
        foreach (SquareState squareState in squareStates)
        {
            squares[i].squareState = squareState;
            ++i;
        }
    }

    public void setBoard(SquareState[] resourceStates, NodeState[] nodeStates, BranchState[] branchStates)
    {
        int nodeIndex = 0, branchIndex = 0;

        setSquares(resourceStates);
        foreach (NodeState nodeState in nodeStates)
        {
            nodes[nodeIndex].nodeState = nodeState;
            ++nodeIndex;
        }
        foreach (BranchState branchState in branchStates)
        {
            branches[branchIndex].branchState = branchState;
            ++branchIndex;
        }
    }

    public BoardState getBoardState()
    {
        foreach (Square square in squares)
        {
            boardState.squareStates[square.id] = square.squareState;
        }

        foreach (Node node in nodes)
        {
            boardState.nodeStates[node.id] = node.nodeState;
        }

        foreach (Branch branch in branches)
        {
            boardState.branchStates[branch.id] = branch.branchState;
        }

        return boardState;
    }

    public SquareState[] ShuffleSquares()
    {
        boardState.squareStates = Reference.defaultSquareState;

        for (int i = 0; i < MAX_SQUARES; i++)
        {
            SquareState tmp = boardState.squareStates[i];
            int r = Random.Range(i, MAX_SQUARES);
            boardState.squareStates[i] = boardState.squareStates[r];
            boardState.squareStates[r] = tmp;
        }

        for(int i = 0; i < MAX_SQUARES; i++)
        {
            boardState.squareStates[i].location = i;
        }

        return boardState.squareStates; 
    }

    public override string ToString()
    {
        string gameBoardString = "";

        foreach (SquareState square in boardState.squareStates)
        {
            gameBoardString += square.location.ToString();

            switch (square.ownerColor)
            {
                case PlayerColor.Blank:
                    gameBoardString += "B";
                    break;
                case PlayerColor.Orange:
                    gameBoardString += "O";
                    break;
                case PlayerColor.Purple:
                    gameBoardString += "P";
                    break;
            }

            switch (square.resourceState)
            {
                case SquareStatus.Open:
                    gameBoardString += "O";
                    break;
                case SquareStatus.Captured:
                    gameBoardString += "C";
                    break;
                case SquareStatus.Blocked:
                    gameBoardString += "B";
                    break;
            }

            switch (square.resourceAmount)
            {
                case SquareResourceAmount.Blank:
                    gameBoardString += "0";
                    break;
                case SquareResourceAmount.One:
                    gameBoardString += "1";
                    break;
                case SquareResourceAmount.Two:
                    gameBoardString += "2";
                    break;
                case SquareResourceAmount.Three:
                    gameBoardString += "3";
                    break;
            }

            switch (square.resourceColor)
            {
                case SquareResourceColor.Blue:
                    gameBoardString += "B";
                    break;
                case SquareResourceColor.Yellow:
                    gameBoardString += "Y";
                    break;
                case SquareResourceColor.Red:
                    gameBoardString += "R";
                    break;
                case SquareResourceColor.Green:
                    gameBoardString += "G";
                    break;
                default:
                    gameBoardString += "L";
                    break;
            }

        }

        return gameBoardString;
    }

    public void StringToConfiguration(string networkBoardConfig)
    {
        int stringIndex = 0;

        for (int i = 0; i < MAX_SQUARES; i++)
        {
            squares[i].squareState.location = i;
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case 'B':
                    squares[i].squareState.ownerColor = PlayerColor.Blank;
                    break;
                case 'O':
                    squares[i].squareState.ownerColor = PlayerColor.Orange;
                    break;
                case 'P':
                    squares[i].squareState.ownerColor = PlayerColor.Purple;
                    break;
            }
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case 'O':
                    squares[i].squareState.resourceState = SquareStatus.Open;
                    break;
                case 'C':
                    squares[i].squareState.resourceState = SquareStatus.Captured;
                    break;
                case 'B':
                    squares[i].squareState.resourceState = SquareStatus.Blocked;
                    break;
            }
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case '0':
                    squares[i].squareState.resourceAmount = SquareResourceAmount.Blank;
                    break;
                case '1':
                    squares[i].squareState.resourceAmount = SquareResourceAmount.One;
                    break;
                case '2':
                    squares[i].squareState.resourceAmount = SquareResourceAmount.Two;
                    break;
                case '3':
                    squares[i].squareState.resourceAmount = SquareResourceAmount.Three;
                    break;

            }
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case 'B':
                    squares[i].squareState.resourceColor = SquareResourceColor.Blue;
                    break;
                case 'R':
                    squares[i].squareState.resourceColor = SquareResourceColor.Red;
                    break;
                case 'Y':
                    squares[i].squareState.resourceColor = SquareResourceColor.Yellow;
                    break;
                case 'G':
                    squares[i].squareState.resourceColor = SquareResourceColor.Green;
                    break;
                case 'L':
                    squares[i].squareState.resourceColor = SquareResourceColor.Blue;
                    break;
            }
            stringIndex++;
        }
    }

    public SquareState[] GetSquareStates()
    {
        return boardState.squareStates;
    }

    public NodeState[] GetNodeStates()
    {
        return boardState.nodeStates;
    }

    public BranchState[] GetBranchStates()
    {
        return boardState.branchStates;
    }

    public List<int> GetPlayersBranches(PlayerColor playerColor)
    {
        List<int> ownedBranches = new List<int>();

        foreach(BranchState branch in boardState.branchStates)
        {
            if(branch.branchColor == playerColor)
            {
                ownedBranches.Add(branch.location);
            }
        }
        return ownedBranches;
    }

    public int GetNumberOfPlayerNodes(PlayerColor playerColor)
    {
        int ownedNodes = 0;

        foreach (Node node in nodes)
        {
            if (node.nodeState.nodeColor == playerColor)
            {
                ownedNodes++;
            }
        }
        return ownedNodes;
    }

    public int GetNumberOfPlayerCapturedTiles(PlayerColor playerColor)
    {
        int ownedTiles = 0;
        DetectTileCaptures();

        foreach (SquareState square in boardState.squareStates)
        {
            if (square.resourceState == SquareStatus.Captured && square.ownerColor == playerColor)
            {
                ownedTiles++;
            }
        }
        return ownedTiles;
    }

    // A fail fast method that checks the branches surrounding each square and stops detection on a square, moving to the next if it
    //  encounters either a blank branch or one owned by the opposing player.
    public void DetectTileCaptures()
    {
        for (int currentSquare = 0; currentSquare < MAX_SQUARES; ++currentSquare)
        {
            bool isCaptured = true;
            PlayerColor currentCaptureColor = branches[Reference.branchesOnSquareConnections[currentSquare, 0]].branchState.branchColor;
            for (int connectedBranch = 0; connectedBranch < 4 && isCaptured; ++connectedBranch)
            {
                Branch currentBranch = branches[Reference.branchesOnSquareConnections[currentSquare, connectedBranch]];
                if (currentBranch.branchState.branchColor != currentCaptureColor || currentCaptureColor == PlayerColor.Blank)
                {
                    isCaptured = false;
                }
            }

            if (isCaptured)
            {
                squares[currentSquare].squareState.ownerColor = currentCaptureColor;
                squares[currentSquare].squareState.resourceState = SquareStatus.Captured;
            }
        };
    }

    public void DetectTileOverloads()
    {
        for (int i = 0; i < MAX_SQUARES; i++)
        {
            int numberOwnedNodes = 0;
            if(squares[i].squareState.resourceState == SquareStatus.Open)
            {
                bool isBlocked = false;
                int[] connectedNodes = ReferenceScript.tileConnectsToTheseNodes[i];

                foreach(int node in connectedNodes)
                {
                    if (nodes[node].nodeState.nodeColor != PlayerColor.Blank)
                        numberOwnedNodes++;
                }

                switch(squares[i].squareState.resourceAmount)
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

                if(isBlocked)
                {
                   squares[i].squareState.resourceState = SquareStatus.Blocked;
                }
            }
        }
    }
}
