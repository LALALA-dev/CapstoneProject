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

    // Given an array of ResourceState, sets each square on the board to that layout.
    // Used to set up a custom gameboard.
    public void setSquares(SquareState[] squareStates)
    {
        int i = 0;
        foreach (SquareState squareState in squareStates)
        {
            squares[i].squareState = squareState;
            ++i;
        }
    }

    // Sets the board up as per the given arrays of SquareState, NodeState, and BranchState.
    // Used to set up a custom gameboard with pieces in desired locations.
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
            boardState.squareStates[i].location = stringIndex;
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case 'B':
                    boardState.squareStates[i].ownerColor = PlayerColor.Blank;
                    break;
                case 'O':
                    boardState.squareStates[i].ownerColor = PlayerColor.Orange;
                    break;
                case 'P':
                    boardState.squareStates[i].ownerColor = PlayerColor.Purple;
                    break;
            }
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case 'O':
                    boardState.squareStates[i].resourceState = SquareStatus.Open;
                    break;
                case 'C':
                    boardState.squareStates[i].resourceState = SquareStatus.Captured;
                    break;
                case 'B':
                    boardState.squareStates[i].resourceState = SquareStatus.Blocked;
                    break;
            }
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case '0':
                    boardState.squareStates[i].resourceAmount = SquareResourceAmount.Blank;
                    break;
                case '1':
                    boardState.squareStates[i].resourceAmount = SquareResourceAmount.One;
                    break;
                case '2':
                    boardState.squareStates[i].resourceAmount = SquareResourceAmount.Two;
                    break;
                case '3':
                    boardState.squareStates[i].resourceAmount = SquareResourceAmount.Three;
                    break;

            }
            stringIndex++;

            switch (networkBoardConfig[stringIndex])
            {
                case 'B':
                    boardState.squareStates[i].resourceColor = SquareResourceColor.Blue;
                    break;
                case 'R':
                    boardState.squareStates[i].resourceColor = SquareResourceColor.Red;
                    break;
                case 'Y':
                    boardState.squareStates[i].resourceColor = SquareResourceColor.Yellow;
                    break;
                case 'G':
                    boardState.squareStates[i].resourceColor = SquareResourceColor.Green;
                    break;
                case 'L':
                    boardState.squareStates[i].resourceColor = SquareResourceColor.Blue;
                    break;
            }
            stringIndex++;
        }
    }
}
