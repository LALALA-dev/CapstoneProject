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

    /*  Methods for sending the board state to the console. Demonstrats how board is stored.    */
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

    public void CollectCurrentPlayerResources()
    {

    }

}

