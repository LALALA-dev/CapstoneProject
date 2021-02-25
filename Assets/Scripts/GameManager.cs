using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GameObjectProperties;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NetworkController networkController;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PlayerResourcesManager playerResourcesManager;
    private GameController gameController;
    private BeginnerAI beginnerAI;

    public TMP_InputField HNPInput;
    public GameObject RenderBtn;

    #region Setup
    private void Awake()
    {
        gameController = GameController.getInstance();

        if (GameInformation.playerIsHost)
            RenderBtn.gameObject.SetActive(false);
        if(!GameInformation.HumanNetworkProtocol)
            HNPInput.gameObject.SetActive(false);
    }

    void Start()
    {
        if (GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            networkController.SendOpeningBoardConfiguration(gameController.getGameBoard().ToString());
            NetworkGame();
        }
        else if(GameInformation.gameType == 'A')
        {
            BeginnerAIGame();
        }
        else if(GameInformation.gameType == 'E')
        {
            ExpertAIGame();
        }
        else if (GameInformation.HumanNetworkProtocol)
        {
            HNPInput.gameObject.SetActive(true);
        }
    }
    #endregion

    private void Update()
    {
        if(GameInformation.tradeHasBeenMade)
        {
            GameInformation.tradeHasBeenMade = false;
            playerResourcesManager.UpdateBothPlayersResources();
        }
        gameController.UpdateGameBoard();
    }

    #region Network Game
    public void NetworkGame()
    {
        RenderHostBoard();
        NetworkOpeningSequence();

        while(!GameInformation.gameOver)
        {
            NetworkPlayerMove();
        }
    }

    public void RenderHostBoard()
    {
        if (!GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            string hostBoard = networkController.GetMove();
            gameController.SetBoardConfiguration(hostBoard);
            RenderBtn.gameObject.SetActive(false);
            boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());

        }
    }

    public void NetworkOpeningSequence()
    {

    }

    public void NetworkPlayerMove()
    {

    }
    #endregion

    #region AI Game
    public void BeginnerAIGame()
    {
        PlayerColor aiColor;
        if (GameInformation.playerIsHost)
            aiColor = PlayerColor.Purple;
        else
            aiColor = PlayerColor.Orange;

        boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        beginnerAI = new BeginnerAI(aiColor, gameController.getGameBoard().getBoardState());

        if(!GameInformation.playerIsHost)
        {
            BoardState AIMove = beginnerAI.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
            gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
            EndCurrentPlayersTurn();
        }
    }

    public void ExpertAIGame()
    {

    }

    public void BeginHumanOpenningMove()
    {
        GameInformation.openingMoveBranchSet = false;
        GameInformation.openingMoveNodeSet = false;
        GameInformation.currentPlayer = "HUMAN";
        gameController.FlipColors();
        GameInformation.humanMoveFinished = false;
    }

    public void EndCurrentPlayersTurn()
    {
        if (GameInformation.openingSequence && GameInformation.currentPlayer == "HUMAN" && OpeningMoveSatisfied())
        {
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if(GameInformation.playerIsHost)
            {
                if(GameInformation.turnNumber == 1)
                {
                    GameInformation.turnNumber++;
                    RandomAIOpenningMove();

                    GameInformation.humanMoveFinished = false;
                    EndCurrentPlayersTurn();
                }
                else if(GameInformation.turnNumber == 4)
                {
                    GameInformation.openingSequence = false;
                    GameInformation.turnNumber++;
                    GameInformation.currentPlayer = "AI"; 
                    gameController.FlipColors();

                    gameController.CollectCurrentPlayerResources();
                    gameController.UpdateScores();

                    int[] AIResources;
                    if (!GameInformation.playerIsHost)
                        AIResources = GameInformation.playerOneResources;
                    else
                        AIResources = GameInformation.playerTwoResources;

                    BoardState AIMove = beginnerAI.RandomMove(gameController.getGameBoard().getBoardState(), AIResources);
                    gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
                    boardManager.RefreshForAIMoves();
                    EndCurrentPlayersTurn();
                }
            }
            else
            {
                if(GameInformation.turnNumber == 2)
                {
                    GameInformation.turnNumber++;
                    gameController.FlipColors();
                    BeginHumanOpenningMove();
                }
                else if(GameInformation.turnNumber == 3)
                {
                    GameInformation.turnNumber++;
                    RandomAIOpenningMove();
                    EndCurrentPlayersTurn();
                }
            }
        }
        else if(GameInformation.openingSequence && GameInformation.currentPlayer == "AI")
        {
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if (!GameInformation.playerIsHost)
            {
                if (GameInformation.turnNumber == 1)
                {
                    boardManager.RefreshForAIMoves();
                    GameInformation.turnNumber++;
                    BeginHumanOpenningMove();
                }
                else if (GameInformation.turnNumber == 4)
                {
                    GameInformation.openingSequence = false;
                    GameInformation.turnNumber++;
                    GameInformation.currentPlayer = "HUMAN";
                    GameInformation.humanMoveFinished = false;
                    gameController.FlipColors();
                    gameController.CollectCurrentPlayerResources();
                    gameController.UpdateScores();
                    playerResourcesManager.UpdateBothPlayersResources();
                }
            }
            else
            {
                if (GameInformation.turnNumber == 2)
                {
                    GameInformation.turnNumber++;
                    RandomAIOpenningMove();
                    GameInformation.humanMoveFinished = false;
                    EndCurrentPlayersTurn();
                }
                else if (GameInformation.turnNumber == 3)
                {
                    GameInformation.turnNumber++;
                    GameInformation.currentPlayer = "HUMAN";
                    gameController.FlipColors();

                    GameInformation.humanMoveFinished = false;
                    BeginHumanOpenningMove();
                }
            }
        }
        else
        {
            GameInformation.turnNumber++;
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            gameController.RefreshCapturedTiles();
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().GetSquareStates());
            GameInformation.currentRoundPlacedNodes.Clear();
            GameInformation.currentRoundPlacedBranches.Clear();

            GameInformation.resourceTrade = false;
            if (GameInformation.currentPlayer == "HUMAN")
            {
                GameInformation.currentPlayer = "AI";
                gameController.FlipColors();
            }
            else
            {
                GameInformation.currentPlayer = "HUMAN";
                gameController.FlipColors();
            }
            gameController.CollectCurrentPlayerResources();
            playerResourcesManager.UpdateBothPlayersResources();
            gameController.UpdateScores();

            if (GameInformation.currentPlayer == "AI")
            {
                RandomAIMove();
                EndCurrentPlayersTurn();
            }

            if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
            {
                GameInformation.gameOver = true;
                return;
            }
        }
    }

    public void RandomAIOpenningMove()
    {
        GameInformation.currentPlayer = "AI";
        gameController.FlipColors();
        BoardState AIMove = beginnerAI.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
        gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
        boardManager.RefreshForAIMoves();
    }

    public void RandomAIMove()
    {
        int[] AIResources;
        if (!GameInformation.playerIsHost)
            AIResources = GameInformation.playerOneResources;
        else
            AIResources = GameInformation.playerTwoResources;

        BoardState AIMove = beginnerAI.RandomMove(gameController.getGameBoard().getBoardState(), AIResources);
        gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
        boardManager.RefreshForAIMoves();
    }

    #endregion

    public void UpdateResourcesUI()
    {
        playerResourcesManager.UpdateBothPlayersResources();
    }

    #region Logic Checks
    public bool OpeningMoveSatisfied()
    {
        return OpeningMovePlacedNode() && OpeningMovePlacedConnectingBranch();
    }

    public bool OpeningMovePlacedNode()
    {
        return GameInformation.openingMoveNodeSet;
    }

    public bool OpeningMovePlacedConnectingBranch()
    {
        return GameInformation.openingMoveBranchSet;
    }
    #endregion

    public void SetUpHNPGame()
    {
        gameController.SetBoardConfiguration(HNPInput.text.Trim());
        boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        HNPInput.gameObject.SetActive(false);
        BeginnerAIGame();
    }
}
