using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class GameController
{
    private static GameController gameController;
    private GameBoard gameBoard;

    private PlayerColor currentPlayerColor;

    private GameController()
    {
        gameBoard = new GameBoard();
        gameBoard.ShuffleSquares();
    }

    public static GameController getInstance()
    {
        if (gameController == null)
        {
            gameController = new GameController();
            gameController.currentPlayerColor = PlayerColor.Orange;
        }
        return gameController;
    }

    public GameBoard getGameBoard()
    {
        return gameBoard;
    }

    public PlayerColor getCurrentPlayerColor()
    {
        return currentPlayerColor;
    }

    public void endTurn()
    {
        GameInformation.turnNumber++;

        if (!GameInformation.openingSequence || GameInformation.turnNumber == 5)
        {
            GameInformation.openingSequence = false;
            if (currentPlayerColor == PlayerColor.Orange)
                currentPlayerColor = PlayerColor.Purple;
            else
                currentPlayerColor = PlayerColor.Orange;
            CollectCurrentPlayerResources();
        }
        else if (GameInformation.turnNumber == 2)
        {
            currentPlayerColor = PlayerColor.Purple;
            GameInformation.openingMoveBranchSet = false;
            GameInformation.openingMoveNodeSet = false;
        }
        else if (GameInformation.turnNumber == 3)
        {
            currentPlayerColor = PlayerColor.Purple;
            GameInformation.openingMoveBranchSet = false;
            GameInformation.openingMoveNodeSet = false;
        }
        else if (GameInformation.turnNumber == 4)
        {
            currentPlayerColor = PlayerColor.Orange;
            GameInformation.openingMoveBranchSet = false;
            GameInformation.openingMoveNodeSet = false;
        }


        Debug.Log("BoardState: \n\t" + getCurrentSquareConfig() + "\n\t" + getCurrentNodeConfig() + "\n\t" + getCurrentBranchConfig());
    }

    private string getCurrentSquareConfig()
    {
        SquareState[] squareStates = gameBoard.getBoardState().squareStates;
        string squareConfigString = "Squares:";

        foreach (SquareState squareState in squareStates)
        {
            squareConfigString += "\n\t\tLocation: " + squareState.location + "\tOwner Color: " + squareState.ownerColor + "\tResource State: " + squareState.resourceState +
                "\tResource Color: " + squareState.resourceColor + "\tResource Amount: " + squareState.resourceAmount;
        }

        return squareConfigString;
    }

    private string getCurrentNodeConfig()
    {
        NodeState[] nodeStates = gameBoard.getBoardState().nodeStates;
        string nodeConfigString = "Nodes:";

        foreach (NodeState nodeState in nodeStates)
        {
            nodeConfigString += "\n\t\tLocation: " + nodeState.location + "\tColor: " + nodeState.nodeColor;
        }

        return nodeConfigString;
    }

    private string getCurrentBranchConfig()
    {
        BranchState[] branchStates = gameBoard.getBoardState().branchStates;
        string branchConfigString = "Branches:";

        foreach (BranchState branchState in branchStates)
        {
            branchConfigString += "\n\t\tLocation: " + branchState.location + "\tBranch Color: " + branchState.branchColor + "\tOwner Color: " + branchState.ownerColor;
        }

        return branchConfigString;
    }

    public void SetBoardConfiguration(string hostGameBoard)
    {
        gameBoard.StringToConfiguration(hostGameBoard);
        Debug.Log("BoardState: \n\t" + getCurrentSquareConfig() + "\n\t" + getCurrentNodeConfig() + "\n\t" + getCurrentBranchConfig());
    }

    public SquareState[] NewGame()
    {
        return gameBoard.ShuffleSquares();
    }

    public SquareState[] GetSquareStates()
    {
        return gameBoard.GetSquareStates();
    }

    public NodeState[] GetNodeStates()
    {
        return gameBoard.GetNodeStates();
    }

    public void CollectCurrentPlayerResources()
    {
        List<NodeState> currentNodes = new List<NodeState>();
        foreach (NodeState node in GetNodeStates())
        {
            if(node.nodeColor == getCurrentPlayerColor())
                currentNodes.Add(node);
        }

        List<SquareState> squares = new List<SquareState>();
        foreach(NodeState node in currentNodes)
        {
            foreach(int squareId in ReferenceScript.nodeConnectToTheseTiles[node.location])
            {
                if(GetSquareStates()[squareId].resourceState == SquareStatus.Open)
                    squares.Add(GetSquareStates()[squareId]);
            }
        }

        int[] resources = new int[4] { 0, 0, 0, 0 };
        foreach(SquareState square in squares)
        {
            switch (square.resourceColor)
            {
                case SquareResourceColor.Red:
                    resources[0]++;
                    break;
                case SquareResourceColor.Blue:
                    resources[1]++;
                    break;
                case SquareResourceColor.Yellow:
                    resources[2]++;
                    break;
                case SquareResourceColor.Green:
                    resources[3]++;
                    break;
                default:
                    break;
            }
        }

        if (getCurrentPlayerColor() == PlayerColor.Orange)
            GameInformation.playerOneResources = resources;
        else
            GameInformation.playerTwoResources = resources;
        Debug.Log("RESOURCES- Red: " + resources[0] + " Blue: " + resources[1] + " Yellow: " + resources[2] + " Green: " + resources[3]);
    }

}

