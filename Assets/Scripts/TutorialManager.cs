using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static GameObjectProperties;
using System;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PlayerResourcesManager playerResourcesManager;
    private GameController gameController;
    private BeginnerAI beginnerAI;

    [SerializeField] private Image playerOneAvatar;
    [SerializeField] private Image playerTwoAvatar;

    public Text playerOneScore;
    public Text playerTwoScore;
    public Text currentPlayerMessage;

    public Sprite[] avatars;

    public int turnNumber = 1;

    #region Setup
    private void Awake()
    {
        gameController = GameController.getInstance();

        playerOneAvatar.sprite = avatars[0];
        playerTwoAvatar.sprite = avatars[1];
    }

    void Start()
    {
        BeginBeginnerAIGame();
    }
    #endregion

    private void Update()
    {

        if (GameInformation.tradeHasBeenMade)
        {
            GameInformation.tradeHasBeenMade = false;
            playerResourcesManager.UpdateBothPlayersResources();
        }

    }

    #region AI Game
    public void BeginBeginnerAIGame()
    {
        PlayerColor aiColor;
        if (GameInformation.playerIsHost)
            aiColor = PlayerColor.Gold;
        else
            aiColor = PlayerColor.Silver;

        boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        beginnerAI = new BeginnerAI(aiColor, gameController.getGameBoard().getBoardState());

        if (!GameInformation.playerIsHost)
        {
            currentPlayerMessage.text = "AI's Move";
            BoardState AIMove = beginnerAI.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
            gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
            EndCurrentAIPlayersTurn();
        }
        else
        {
            currentPlayerMessage.text = "Your Move";
        }
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
            if (GameInformation.playerIsHost)
            {
                if (turnNumber == 1)
                {
                    currentPlayerMessage.text = "AI's Move";
                    turnNumber++;
                    RandomAIOpeningMove();

                    GameInformation.humanMoveFinished = false;
                    EndCurrentAIPlayersTurn();
                }
                else if (turnNumber == 4)
                {
                    currentPlayerMessage.text = "AI's Move";
                    GameInformation.openingSequence = false;
                    turnNumber++;
                    GameInformation.currentPlayer = "AI";
                    gameController.FlipColors();

                    gameController.CollectCurrentPlayerResources();
                    gameController.UpdateScores();
                    playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                    playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

                    int[] AIResources;
                    if (!GameInformation.playerIsHost)
                        AIResources = GameInformation.playerOneResources;
                    else
                        AIResources = GameInformation.playerTwoResources;

                    BoardState AIMove = beginnerAI.RandomMove(gameController.getGameBoard().getBoardState(), AIResources);
                    gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
                    boardManager.RefreshBoardGUI();
                    EndCurrentAIPlayersTurn();
                }
            }
            else
            {
                if (turnNumber == 2)
                {
                    currentPlayerMessage.text = "Your Move";
                    turnNumber++;
                    gameController.FlipColors();
                    BeginHumanOpeningMove();
                }
                else if (turnNumber == 3)
                {
                    currentPlayerMessage.text = "AI's Move";
                    turnNumber++;
                    RandomAIOpeningMove();
                    EndCurrentAIPlayersTurn();
                }
            }
        }
        else if (GameInformation.openingSequence && GameInformation.currentPlayer == "AI")
        {
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if (!GameInformation.playerIsHost)
            {
                if (turnNumber == 1)
                {
                    boardManager.RefreshBoardGUI();
                    turnNumber++;
                    currentPlayerMessage.text = "Your Move";
                    BeginHumanOpeningMove();
                }
                else if (turnNumber == 4)
                {
                    currentPlayerMessage.text = "Your Move";
                    GameInformation.openingSequence = false;
                    turnNumber++;
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
                if (turnNumber == 2)
                {
                    currentPlayerMessage.text = "AI's Move";
                    turnNumber++;
                    RandomAIOpeningMove();
                    GameInformation.humanMoveFinished = false;
                    EndCurrentAIPlayersTurn();
                }
                else if (turnNumber == 3)
                {
                    currentPlayerMessage.text = "Your Move";
                    turnNumber++;
                    GameInformation.currentPlayer = "HUMAN";
                    gameController.FlipColors();

                    GameInformation.humanMoveFinished = false;
                    BeginHumanOpeningMove();
                }
            }
        }
        else if (!GameInformation.openingSequence)
        {
            turnNumber++;
            gameController.UpdateGameBoard();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
            GameInformation.currentRoundPlacedNodes.Clear();
            GameInformation.currentRoundPlacedBranches.Clear();

            GameInformation.resourceTrade = false;
            if (GameInformation.currentPlayer == "HUMAN")
            {
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "AI's Move";
            }
            else
            {
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
            }
            gameController.FlipColors();
            gameController.CollectCurrentPlayerResources();
            playerResourcesManager.UpdateBothPlayersResources();
            gameController.UpdateScores();

            playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
            playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

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
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
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
        boardManager.RefreshBoardGUI();
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
        boardManager.RefreshBoardGUI();
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

    public bool OpponentHasSentNewMoveToProcess()
    {
        return GameInformation.gameType == 'N' && GameInformation.newNetworkMoveSet;
    }

    public bool NeedToSyncNetworkGameVariables()
    {
        return GameInformation.gameType == 'N' && GameInformation.needToSyncGameVariables;
    }

    public bool IsTheCurrentPlayerYourself()
    {
        return (GameInformation.currentPlayer == "HOST" && GameInformation.playerIsHost) || (GameInformation.currentPlayer == "CLIENT" && !GameInformation.playerIsHost);
    }

    public bool IsCorrectHostOpeningMove()
    {
        return GameInformation.openingSequence && GameInformation.currentPlayer == "HOST" && OpeningMoveSatisfied();
    }

    public bool IsCorrectClientOpeningMove()
    {
        return GameInformation.openingSequence && GameInformation.currentPlayer == "CLIENT" && OpeningMoveSatisfied();
    }
    #endregion


    public void ToogleTriggers()
    {
        BroadcastMessage("ToggleNodeBranchTriggers");
    }

    public void EndTurnButtonClick()
    {
        EndCurrentAIPlayersTurn();
    }
}
