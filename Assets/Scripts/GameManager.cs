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

    #region Setup
    private void Awake()
    {
        gameController = GameController.getInstance();

        if(!GameInformation.HumanNetworkProtocol)
            HNPInput.gameObject.SetActive(false);
    }

    void Start()
    {
        if (GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            GameInformation.currentPlayer = "HOST";
            networkController.SendOpeningBoardConfiguration(gameController.getGameBoard().ToString());
            BeginNetworkGame();
        }
        else if(GameInformation.gameType == 'A')
        {
            BeginBeginnerAIGame();
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
        if(GameInformation.renderClientBoard && GameInformation.gameType == 'N' && !GameInformation.playerIsHost)
        {
            RenderHostBoard();
            GameInformation.renderClientBoard = false;
        }

        if(GameInformation.tradeHasBeenMade)
        {
            GameInformation.tradeHasBeenMade = false;
            playerResourcesManager.UpdateBothPlayersResources();
        }

        if(GameInformation.gameType == 'N' && GameInformation.enableTriggers)
        {
            GameInformation.enableTriggers = false;
            ToogleTriggers();
        }

        if (GameInformation.gameType == 'N' && GameInformation.newNetworkMoveSet)
        {
            GameInformation.newNetworkMoveSet = false;
            string hostBoard = networkController.GetMove();
            gameController.SetBoardConfiguration(hostBoard);
            gameController.RefreshBlockedTiles();
            boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        }
    }

    #region Network Game
    public void BeginNetworkGame()
    {
        boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        networkController.InvokeClientsRenderHost();
    }

    public void RenderHostBoard()
    {
        if (GameInformation.gameType == 'N' && !GameInformation.playerIsHost)
        {
            string hostBoard = networkController.GetMove();
            gameController.SetBoardConfiguration(hostBoard);
            boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
            gameController.FlipColors();
            ToogleTriggers();
        }
    }

    private void EndCurrentNetworkPlayersTurn()
    {
        if (GameInformation.openingSequence && GameInformation.currentPlayer == "HOST" && OpeningMoveSatisfied())
        {
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if (GameInformation.playerIsHost)
            {
                if (GameInformation.turnNumber == 1)
                {
                    GameInformation.turnNumber++;
                    // TODO: SETUP CLIENT AS CURRENT PLAYER FOR THEIR FIRST OPENING MOVE
                    GameInformation.currentPlayer = "CLIENT";
                    // DISABLE INTERACTION WITH GUI
                    ToogleTriggers();
                    // TELL THE CLIENT TO ENABLE THEIR GUI
                    networkController.EnableOpponentsTriggers();
                    // SEND CLIENT MY BOARD STATE
                    networkController.SendMove(gameController.getGameBoard().ToString());
                }
                else if (GameInformation.turnNumber == 4)
                {
                    GameInformation.openingSequence = false;
                    GameInformation.turnNumber++;

                    GameInformation.currentPlayer = "CLIENT";
                    // DISABLE INTERACTION WITH GUI
                    ToogleTriggers();
                    // TELL THE CLIENT TO ENABLE THEIR GUI
                    networkController.EnableOpponentsTriggers();
                    // SEND CLIENT MY BOARD STATE
                    networkController.SendMove(gameController.getGameBoard().ToString());

                    // TODO: COLLECT CLIENT'S RESOURCES AND BEGIN ACTUAL GAME
                    gameController.CollectCurrentPlayerResources();
                    gameController.UpdateScores();
                }
            }
        }
        else if (GameInformation.openingSequence && GameInformation.currentPlayer == "ClIENT" && OpeningMoveSatisfied())
        {
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if (!GameInformation.playerIsHost)
            {
                if (GameInformation.turnNumber == 2)
                {
                    GameInformation.turnNumber++;
                    // TODO: CLIENT IS STILL CURRENT PLAYER FOR SECOND OPENING MOVE
                    // SEND CLIENT MY BOARD STATE
                    networkController.SendMove(gameController.getGameBoard().ToString());
                    GameInformation.openingMoveBranchSet = false;
                    GameInformation.openingMoveNodeSet = false;

                }
                else if (GameInformation.turnNumber == 3)
                {
                    GameInformation.turnNumber++;
                    // TODO: SETUP HOST AS CURRENT PLAYER FOR THEIR SECOND OPENING MOVE

                    GameInformation.currentPlayer = "HOST";
                    // DISABLE INTERACTION WITH GUI
                    ToogleTriggers();
                    // TELL THE CLIENT TO ENABLE THEIR GUI
                    networkController.EnableOpponentsTriggers();
                    // SEND CLIENT MY BOARD STATE
                    networkController.SendMove(gameController.getGameBoard().ToString());
                }
            }
        }
        else
        {
            // NORMAL GAMEPLAY
            GameInformation.turnNumber++;
            gameController.UpdateGameBoard();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().GetSquareStates());
            GameInformation.currentRoundPlacedNodes.Clear();
            GameInformation.currentRoundPlacedBranches.Clear();

            GameInformation.resourceTrade = false;
            if (GameInformation.currentPlayer == "HOST" && GameInformation.playerIsHost)
            {
                GameInformation.currentPlayer = "CLIENT";
                ToogleTriggers();
                networkController.EnableOpponentsTriggers();
                networkController.SendMove(gameController.getGameBoard().ToString());
            }
            else
            {
                GameInformation.currentPlayer = "HOST";
                ToogleTriggers();
                networkController.EnableOpponentsTriggers();
                networkController.SendMove(gameController.getGameBoard().ToString());
            }

            gameController.FlipColors();
            gameController.CollectCurrentPlayerResources();
            playerResourcesManager.UpdateBothPlayersResources();
            gameController.UpdateScores();

            if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
            {
                GameInformation.gameOver = true;
                return;
            }
        }
    }

    #endregion

    #region AI Game
    public void BeginBeginnerAIGame()
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
            EndCurrentAIPlayersTurn();
        }
    }

    public void ExpertAIGame()
    {

    }

    public void BeginHumanOpeningMove()
    {
        GameInformation.openingMoveBranchSet = false;
        GameInformation.openingMoveNodeSet = false;
        GameInformation.currentPlayer = "HUMAN";
        gameController.FlipColors();
        GameInformation.humanMoveFinished = false;
    }

    private void EndCurrentAIPlayersTurn()
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
                    RandomAIOpeningMove();

                    GameInformation.humanMoveFinished = false;
                    EndCurrentAIPlayersTurn();
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
                    EndCurrentAIPlayersTurn();
                }
            }
            else
            {
                if(GameInformation.turnNumber == 2)
                {
                    GameInformation.turnNumber++;
                    gameController.FlipColors();
                    BeginHumanOpeningMove();
                }
                else if(GameInformation.turnNumber == 3)
                {
                    GameInformation.turnNumber++;
                    RandomAIOpeningMove();
                    EndCurrentAIPlayersTurn();
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
                    BeginHumanOpeningMove();
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
                    RandomAIOpeningMove();
                    GameInformation.humanMoveFinished = false;
                    EndCurrentAIPlayersTurn();
                }
                else if (GameInformation.turnNumber == 3)
                {
                    GameInformation.turnNumber++;
                    GameInformation.currentPlayer = "HUMAN";
                    gameController.FlipColors();

                    GameInformation.humanMoveFinished = false;
                    BeginHumanOpeningMove();
                }
            }
        }
        else
        {
            GameInformation.turnNumber++;
            gameController.UpdateGameBoard();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().GetSquareStates());
            GameInformation.currentRoundPlacedNodes.Clear();
            GameInformation.currentRoundPlacedBranches.Clear();

            GameInformation.resourceTrade = false;
            if (GameInformation.currentPlayer == "HUMAN")
            {
                GameInformation.currentPlayer = "AI";
            }
            else
            {
                GameInformation.currentPlayer = "HUMAN";
            }
            gameController.FlipColors();
            gameController.CollectCurrentPlayerResources();
            playerResourcesManager.UpdateBothPlayersResources();
            gameController.UpdateScores();

            if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
            {
                GameInformation.gameOver = true;
                return;
            }

            if (GameInformation.currentPlayer == "AI")
            {
                RandomAIMove();
                    
                gameController.UpdateGameBoard();
                boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().GetSquareStates());
                if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
                {
                    GameInformation.gameOver = true;
                    return;
                }

                EndCurrentAIPlayersTurn();
            }
        }
    }

    public void RandomAIOpeningMove()
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
        BeginBeginnerAIGame();
    }

    public void ToogleTriggers()
    {
        BroadcastMessage("ToggleNodeBranchTriggers");
    }

    public void EndTurnButtonClick()
    {
        if (GameInformation.gameType == 'N')
            EndCurrentNetworkPlayersTurn();
        else
            EndCurrentAIPlayersTurn();
    }
}
