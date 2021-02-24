using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class GameController
{
    private static GameController gameController;
    private GameBoard gameBoard;
    private BeginnerAI beginnerAI;

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

    public void endTurn()
    {
        GameInformation.turnNumber++;
        gameBoard.DetectTileOverloads();
        gameBoard.DetectMultiTileCaptures();
        if (!GameInformation.openingSequence || GameInformation.turnNumber == 5)
        {
            GameInformation.openingSequence = false;
            if (currentPlayerColor == PlayerColor.Orange)
            {
                if (GameInformation.gameType == 'L')
                {
                    currentPlayerColor = PlayerColor.Purple;
                }
                else
                {
                    currentPlayerColor = PlayerColor.Purple;
                    gameBoard.DetectTileOverloads();
                    UpdateScores();
                    CollectCurrentPlayerResources();
                    BoardState aiMove = beginnerAI.RandomMove(gameBoard.getBoardState(), GameInformation.playerTwoResources);
                    gameBoard.setBoard(aiMove.squareStates, aiMove.nodeStates, aiMove.branchStates);
                    GameInformation.isAIMoveFinished = true;
                    endTurn();
                }
            }
            else
            {
                currentPlayerColor = PlayerColor.Orange;
                GameInformation.resourceTrade = false;
                gameBoard.DetectTileOverloads();
                UpdateScores();
                CollectCurrentPlayerResources();
            }   

        }
        else if (GameInformation.turnNumber == 2)
        {
            // AI OPENING TEST 
            if (GameInformation.gameType == 'A')
            {
                gameController.beginnerAI = new BeginnerAI(PlayerColor.Purple, gameController.getGameBoard().getBoardState());
                BoardState aiMove = beginnerAI.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
                gameBoard.setBoard(aiMove.squareStates, aiMove.nodeStates, aiMove.branchStates);
                GameInformation.isAIMoveFinished = true;
                endTurn();
            }
            else
            {
                currentPlayerColor = PlayerColor.Purple;
                GameInformation.openingMoveBranchSet = false;
                GameInformation.openingMoveNodeSet = false;
            }

            gameBoard.DetectTileOverloads();
        }
        else if (GameInformation.turnNumber == 3)
        {
            // AI OPENING TEST 
            if (GameInformation.gameType == 'A')
            {
                System.Threading.Thread.Sleep(2000);
                BoardState aiMove = beginnerAI.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
                gameBoard.setBoard(aiMove.squareStates, aiMove.nodeStates, aiMove.branchStates);
                GameInformation.isAIMoveFinished = true;
                endTurn();
            }
            else
            {
                currentPlayerColor = PlayerColor.Purple;
                GameInformation.openingMoveBranchSet = false;
                GameInformation.openingMoveNodeSet = false;
            }
            gameBoard.DetectTileOverloads();
        }
        else if (GameInformation.turnNumber == 4)
        {
            currentPlayerColor = PlayerColor.Orange;
            GameInformation.openingMoveBranchSet = false;
            GameInformation.openingMoveNodeSet = false;
            gameBoard.DetectTileOverloads();
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

        if (getCurrentPlayerColor() == PlayerColor.Orange)
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

