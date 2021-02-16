using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class GameController
{
    private static GameController gameController;
    private GameBoard gameBoard;

    private PlayerColor currentPlayerColor = PlayerColor.Orange;

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
            GameInformation.openingMoveBranchSet = false;
            GameInformation.openingMoveNodeSet = false;
            if (currentPlayerColor == PlayerColor.Orange)
                currentPlayerColor = PlayerColor.Purple;
            else
                currentPlayerColor = PlayerColor.Orange;
            UpdateScores();
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

    public BranchState GetBranchState(int id)
    {
        return gameBoard.GetBranchStates()[id];
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
        {
            GameInformation.playerOneResources[0] += resources[0];
            GameInformation.playerOneResources[1] += resources[1];
            GameInformation.playerOneResources[2] += resources[2];
            GameInformation.playerOneResources[3] += resources[3];
            Debug.Log("RESOURCES- Red: " + GameInformation.playerOneResources[0] + " Blue: " + GameInformation.playerOneResources[1] + 
                " Yellow: " + GameInformation.playerOneResources[2] + " Green: " + GameInformation.playerOneResources[3]);
        }
        else
        {
            GameInformation.playerTwoResources[0] += resources[0];
            GameInformation.playerTwoResources[1] += resources[1];
            GameInformation.playerTwoResources[2] += resources[2];
            GameInformation.playerTwoResources[3] += resources[3];
            Debug.Log("RESOURCES- Red: " + GameInformation.playerTwoResources[0] + " Blue: " + GameInformation.playerTwoResources[1]
                + " Yellow: " + GameInformation.playerTwoResources[2] + " Green: " + GameInformation.playerTwoResources[3]);
        }

        
    }

    public int CalculatePlayerLongestNetwork(PlayerColor playerColor)
    {
        int longestNetwork = 0;
        int currentNetwork = 0;
        List<int> runningNetworkBranches = new List<int>();

        List<int> playerBranches = gameBoard.GetPlayersBranches(playerColor);

        runningNetworkBranches.Add(playerBranches[0]);
        currentNetwork++;
        foreach(int ownedBranch in playerBranches)
        {
            if(!runningNetworkBranches.Contains(ownedBranch))
            {
                longestNetwork = currentNetwork;
                currentNetwork = 0;
                runningNetworkBranches.Clear();
            }

            int[] touchingBranches = ReferenceScript.branchConnectsToTheseBranches[ownedBranch];
            foreach(int touchedBranch in touchingBranches)
            {
                if(!runningNetworkBranches.Contains(touchedBranch) && playerBranches.Contains(touchedBranch))
                {
                    runningNetworkBranches.Add(touchedBranch);
                    currentNetwork++;
                }
            }
        }
        if(currentNetwork > longestNetwork)
            longestNetwork = currentNetwork;

        return longestNetwork;
    }

    public void UpdateScores()
    {
        GameInformation.playerOneScore = gameBoard.GetNumberOfPlayerNodes(PlayerColor.Orange);
        GameInformation.playerTwoScore = gameBoard.GetNumberOfPlayerNodes(PlayerColor.Purple);
        GameInformation.playerOneScore += gameBoard.GetNumberOfPlayerCapturedTiles(PlayerColor.Orange);
        GameInformation.playerTwoScore += gameBoard.GetNumberOfPlayerCapturedTiles(PlayerColor.Purple);

        int playerOneNetwork = CalculatePlayerLongestNetwork(PlayerColor.Orange);
        int playerTwoNetwork = CalculatePlayerLongestNetwork(PlayerColor.Purple);

        if (playerOneNetwork > playerTwoNetwork)
            GameInformation.playerOneScore += 2;
        else if (playerOneNetwork < playerTwoNetwork)
            GameInformation.playerTwoScore += 2;
        Debug.Log("SCORES:\nPLAYER ONE: " + GameInformation.playerOneScore + "\nPLAYER TWO: " + GameInformation.playerTwoScore);
    }
}

