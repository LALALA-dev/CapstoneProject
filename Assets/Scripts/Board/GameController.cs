using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class GameController
{
    private static GameController gameController;
    private GameBoard gameBoard;

    private PlayerColor currentPlayerColor = PlayerColor.Silver;

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
            gameController.currentPlayerColor = PlayerColor.Silver;
        }
        return gameController;
    }
    
    public static GameController Destroy()
    {
        if(gameController != null)
        {
            gameController = null;
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

    public void FlipColors()
    {
        if (currentPlayerColor == PlayerColor.Silver)
            currentPlayerColor = PlayerColor.Gold;
        else
            currentPlayerColor = PlayerColor.Silver;
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
        if (!GameInformation.HumanNetworkProtocol)
            gameBoard.StringToConfiguration(hostGameBoard);
        else
            gameBoard.SetHNP(hostGameBoard);
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
        foreach (Node node in gameBoard.nodes)
        {
            if(node.nodeState.nodeColor == getCurrentPlayerColor())
                currentNodes.Add(node.nodeState);
        }

        List<SquareState> squares = new List<SquareState>();
        foreach(NodeState node in currentNodes)
        {
            foreach(int squareId in ReferenceScript.nodeConnectToTheseTiles[node.location])
            {
                if(gameBoard.squares[squareId].squareState.resourceState == SquareStatus.Open || (gameBoard.squares[squareId].squareState.ownerColor == getCurrentPlayerColor()))
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

        if (GameInformation.gameType == 'N')
        {
            if ((GameInformation.playerIsHost && GameInformation.currentPlayer == "HOST"))
            {
                GameInformation.playerOneResources[0] += resources[0];
                GameInformation.playerOneResources[1] += resources[1];
                GameInformation.playerOneResources[2] += resources[2];
                GameInformation.playerOneResources[3] += resources[3];
            }
            else
            {
                GameInformation.playerTwoResources[0] += resources[0];
                GameInformation.playerTwoResources[1] += resources[1];
                GameInformation.playerTwoResources[2] += resources[2];
                GameInformation.playerTwoResources[3] += resources[3];
            }
        }
        else
        {
            if ((GameInformation.playerIsHost && GameInformation.currentPlayer == "HUMAN") || (!GameInformation.playerIsHost && GameInformation.currentPlayer == "AI"))
            {
                GameInformation.playerOneResources[0] += resources[0];
                GameInformation.playerOneResources[1] += resources[1];
                GameInformation.playerOneResources[2] += resources[2];
                GameInformation.playerOneResources[3] += resources[3];
            }
            else
            {
                GameInformation.playerTwoResources[0] += resources[0];
                GameInformation.playerTwoResources[1] += resources[1];
                GameInformation.playerTwoResources[2] += resources[2];
                GameInformation.playerTwoResources[3] += resources[3];
            }
        }

        
    }

    public int CalculatePlayerLongestNetwork(PlayerColor playerColor)
    {
        int longestNetwork = 0;

        List<int> playerBranches = gameBoard.GetPlayersBranches(playerColor);
        List<int> checkedBranches = new List<int>();
        Stack<int> branchesToCheck = new Stack<int>();

        branchesToCheck.Push(playerBranches[0]);

        while(branchesToCheck.Count > 0)
        {
            int currentBranch = branchesToCheck.Pop();
            int[] touchingBranches = ReferenceScript.branchConnectsToTheseBranches[currentBranch];

            foreach (int touchedBranch in touchingBranches)
            {
                if (playerBranches.Contains(touchedBranch) && !checkedBranches.Contains(touchedBranch) && !branchesToCheck.Contains(touchedBranch))
                {
                    branchesToCheck.Push(touchedBranch);
                }
            }

            checkedBranches.Add(currentBranch);
        }

        longestNetwork = checkedBranches.Count;

        if(checkedBranches.Count != playerBranches.Count)
        {
            longestNetwork = checkedBranches.Count;

            foreach(int branch in checkedBranches)
            {
                playerBranches.Remove(branch);
            }

            checkedBranches.Clear();

            branchesToCheck.Push(playerBranches[0]);

            while (branchesToCheck.Count > 0)
            {
                int currentBranch = branchesToCheck.Pop();
                int[] touchingBranches = ReferenceScript.branchConnectsToTheseBranches[currentBranch];

                foreach (int touchedBranch in touchingBranches)
                {
                    if (playerBranches.Contains(touchedBranch) && !checkedBranches.Contains(touchedBranch) && !branchesToCheck.Contains(touchedBranch))
                    {
                        branchesToCheck.Push(touchedBranch);
                    }
                }

                checkedBranches.Add(currentBranch);
            }

            if (checkedBranches.Count > longestNetwork)
                return checkedBranches.Count;
        }

        return longestNetwork;
    }

    public void UpdateScores()
    {
        GameInformation.playerOneScore = gameBoard.GetNumberOfPlayerNodes(PlayerColor.Silver);
        GameInformation.playerTwoScore = gameBoard.GetNumberOfPlayerNodes(PlayerColor.Gold);
        GameInformation.playerOneScore += gameBoard.GetNumberOfPlayerCapturedTiles(PlayerColor.Silver);
        GameInformation.playerTwoScore += gameBoard.GetNumberOfPlayerCapturedTiles(PlayerColor.Gold);

        int playerOneNetwork = CalculatePlayerLongestNetwork(PlayerColor.Silver);
        int playerTwoNetwork = CalculatePlayerLongestNetwork(PlayerColor.Gold);

        if (playerOneNetwork > playerTwoNetwork)
            GameInformation.playerOneScore += 2;
        else if (playerOneNetwork < playerTwoNetwork)
            GameInformation.playerTwoScore += 2;
    }

    public void UpdateGameBoard()
    {
        RefreshBlockedTiles();
        RefreshCapturedTiles();
    }

    public void RefreshCapturedTiles()
    {
        gameBoard.DetectMultiTileCaptures();
    }

    public void RefreshBlockedTiles()
    {
        gameBoard.DetectTileOverloads();
    }

}

