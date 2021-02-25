using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for(int i = 0; i < MAX_SQUARES; i++)
        {
            boardState.squareStates[i].location = i;
            boardState.squareStates[i].ownerColor = PlayerColor.Blank;
            boardState.squareStates[i].resourceState = SquareStatus.Open;
            boardState.squareStates[i].resourceColor = Reference.defaultSquareState[i].resourceColor;
            boardState.squareStates[i].resourceAmount = Reference.defaultSquareState[i].resourceAmount;
        }

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
            // gameBoardString += square.location.ToString();

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
            // stringIndex++;

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
                    squares[i].squareState.resourceColor = SquareResourceColor.Blank;
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
        DetectMultiTileCaptures();

        foreach (SquareState square in boardState.squareStates)
        {
            if (square.resourceState == SquareStatus.Captured && square.ownerColor == playerColor)
            {
                ownedTiles++;
            }
        }
        return ownedTiles;
    }

    public void DetectMultiTileCaptures()
    {
        List<int> captures = new List<int>();
        List<int> possibleCaptures = new List<int>();        

        for (int currentSquare = 0; currentSquare < MAX_SQUARES; ++currentSquare)
        {
            bool couldBeCaptured = true;
            List<int> blankBranches = new List<int>();
            // The color of the first branch found on the square with a player's piece associated with it.
            PlayerColor currentCaptureColor = branches[Reference.branchesOnSquareConnections[currentSquare, 0]].branchState.branchColor;
            // Look for a player's color.
            for (int connectedBranch = 1; currentCaptureColor == PlayerColor.Blank && connectedBranch < 4; ++connectedBranch)
            {
                currentCaptureColor = branches[Reference.branchesOnSquareConnections[currentSquare, connectedBranch]].branchState.branchColor;
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
                BranchState currentBranchState = branches[Reference.branchesOnSquareConnections[currentSquare, connectedBranch]].branchState;
                
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
            if (isCaptured(square, captures, possibleCaptures))
            {
                captureArea(square, possibleCaptures, captures);
            }
            else
            {
                possibleCaptures.Remove(square);
            }
        }

        foreach (int squareId in captures)
        {
            PlayerColor captureColor = getCapturedSquareOwner(squareId);

            squares[squareId].squareState.ownerColor = captureColor;
            squares[squareId].squareState.resourceState = SquareStatus.Captured;
        }
    }

    // Given a square ID from which to start and the lists of possible captures and captures, will remove
    //  all squares connected to the startSquare by a blank branch and add them to captures.
    private void captureArea(int startSquare, List<int> possibleCaptures, List<int> captures)
    {
        List<int> blankBranches = getBlankBranches(startSquare);
        possibleCaptures.Remove(startSquare);
        captures.Add(startSquare);

        // For every blank branch on this captured square...
        foreach (int blankBranch in blankBranches)
        {
            // If the connected square hasn't yet been moved from possible to captured then move it.
            if (possibleCaptures.Contains(getConnectedSquare(blankBranch, startSquare)))
            {
                captureArea(getConnectedSquare(blankBranch, startSquare), possibleCaptures, captures);
            }
        }
    }

    // Given the ID of a square known to be captured, returns the PlayerColor of the owner.
    private PlayerColor getCapturedSquareOwner(int squareId)
    {
        PlayerColor captureColor;
        // Check the surrounding branches for an owner color.
        for (int branch = 0; branch < 4; ++branch)
        {
            captureColor = branches[Reference.branchesOnSquareConnections[squareId, branch]].branchState.branchColor;
            // If found, return it.
            if (captureColor != PlayerColor.Blank)
            {
                return captureColor;
            }
        }
        // If all surrounding branches are blank, go to the square above and check it for an owner color. 
        return getCapturedSquareOwner(Reference.squareOnSquareConnections[squareId, 0]);
    }

    // Checks the given square and any blank branch connecting squares for capture. 
    //  Returns true if captured, having already modifed the given lists to reflect the capture state
    //  of connected squares (but not itself). Ever heard the tragedy of Darth Plagueis the Wise?
    private bool isCaptured(int startingSquare, List<int> captures, List<int> possibleCaptures)
    {
        List<int> blankBranches = getBlankBranches(startingSquare);
        List<int> checkedSquares = new List<int>();
        checkedSquares.Add(startingSquare);

        foreach (int blankBranch in blankBranches)
        {
            int connectedSquareId = getConnectedSquare(blankBranch, startingSquare);
            if (!possibleCaptures.Contains(connectedSquareId) ||
                !isConnectedSquareCaptured(connectedSquareId, checkedSquares, captures, possibleCaptures))
            {
                possibleCaptures.Remove(startingSquare);
                return false;
            }
        }
        return true;
    }

    // Given a square ID, returns a List of the branch IDs that are blank and connected to that square.
    private List<int> getBlankBranches(int squareId)
    {
        List<int> blankBranches = new List<int>();
        for (int branch = 0; branch < 4; ++branch)
        {
            int branchId = Reference.branchesOnSquareConnections[squareId, branch];
            if (branches[branchId].branchState.branchColor == PlayerColor.Blank)
            {
                blankBranches.Add(branchId);
            }
        }
        return blankBranches;
    }

    // Given a branch ID and a connected square ID, returns the ID of the square on the otherside of the branch.
    private int getConnectedSquare(int branchId, int squareId)
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

    // A recursive method that checks for any connected squares that could be captured and checks to see if they're captured. 
    private bool isConnectedSquareCaptured(int square, List<int> checkedSquares, List<int> captures, List<int> possibleCaptures)
    {
        List<int> blankBranches = getBlankBranches(square);
        checkedSquares.Add(square);

        foreach (int blankBranchId in blankBranches)
        {
            int connectedSquareId = getConnectedSquare(blankBranchId, square);
            if (!checkedSquares.Contains(connectedSquareId))
            {
                if (!possibleCaptures.Contains(connectedSquareId) ||
                    !isConnectedSquareCaptured(connectedSquareId, checkedSquares, captures, possibleCaptures))
                {
                    possibleCaptures.Remove(square);
                    return false;
                }
            }
        }
        return true;
    }

    // Given one player's color, returns the other player's color. Not meant to take blank.
    private PlayerColor getOpponentColor(PlayerColor currentPlayer)
    {
        if (currentPlayer == PlayerColor.Blank)
        {
            Debug.Log("getOpponentColor() is not meant to receive PlayerColor.Blank.");
            return PlayerColor.Blank;
        }

        if (currentPlayer == PlayerColor.Orange)
        {
            return PlayerColor.Purple;
        }
        else
        {
            return PlayerColor.Orange;
        }
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
