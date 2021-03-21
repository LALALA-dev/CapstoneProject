using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static GameObjectProperties;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NetworkController networkController;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PlayerResourcesManager playerResourcesManager;
    private GameController gameController;
    private BeginnerAI beginnerAI;

    public TMP_InputField HNPInput;

    public Image playerOneAvatar;
    public Image playerTwoAvatar;
    public Text playerOneScore;
    public Text playerTwoScore;
    public Text currentPlayerMessage;

    public Sprite[] avatars;

    public int turnNumber = 1;

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

        Image humanAvatar, aiAvatar;
        if(GameInformation.playerIsHost)
        {
            humanAvatar = playerOneAvatar;
            aiAvatar = playerTwoAvatar;
        }
        else
        {
            humanAvatar = playerTwoAvatar;
            aiAvatar = playerOneAvatar;
        }

        switch(GameInformation.playerAvatar)
        {
            case "HAT":
                humanAvatar.sprite = avatars[0];
                break;
            case "BATTLESHIP":
                humanAvatar.sprite = avatars[1];
                break;
            case "CAR":
                humanAvatar.sprite = avatars[2];
                break;
            case "THIMBLE":
                humanAvatar.sprite = avatars[3];
                break;
            case "WHEELBARREL":
                humanAvatar.sprite = avatars[4];
                break;
            default:
                humanAvatar.sprite = avatars[2];
                break;
        }

        switch (GameInformation.opponentAvatar)
        {
            case "HAT":
                aiAvatar.sprite = avatars[0];
                break;
            case "BATTLESHIP":
                aiAvatar.sprite = avatars[1];
                break;
            case "CAR":
                aiAvatar.sprite = avatars[2];
                break;
            case "THIMBLE":
                aiAvatar.sprite = avatars[3];
                break;
            case "WHEELBARREL":
                aiAvatar.sprite = avatars[4];
                break;
            default:
                aiAvatar.sprite = avatars[2];
                break;
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

        if (NeedToSyncNetworkGameVariables())
        {
            GameInformation.needToSyncGameVariables = false;

            GameInformation.currentPlayer = networkController.GetCurrentPlayer();
            turnNumber = networkController.GetTurnNumber();
            string incomingPlayersResources = networkController.GetOpponentResources();

            if(!IsTheCurrentPlayerYourself())
            {
                // parse resource string and update local opponent's resources
            }
            
            // Determine the new currentPlayer
            if(GameInformation.openingSequence)
            {
                GameInformation.openingMoveNodeSet = false;
                GameInformation.openingMoveBranchSet = false;
                switch (turnNumber)
                {
                    case 1:
                        ToogleTriggers();
                        GameInformation.currentPlayer = "CLIENT";
                        if(GameInformation.playerIsHost)
                            currentPlayerMessage.text = "Opponent's Move";
                        else
                            currentPlayerMessage.text = "Your Move";
                        break;
                    case 2:
                        GameInformation.currentPlayer = "CLIENT";
                        if (GameInformation.playerIsHost)
                            currentPlayerMessage.text = "Opponent's Move";
                        else
                            currentPlayerMessage.text = "Your Move";
                        break;
                    case 3:
                        ToogleTriggers();
                        GameInformation.currentPlayer = "HOST";
                        if (GameInformation.playerIsHost)
                            currentPlayerMessage.text = "Your Move";
                        else
                            currentPlayerMessage.text = "Opponent's Move";
                        break;
                    case 4:
                        ToogleTriggers();
                        GameInformation.currentPlayer = "CLIENT";
                        if (GameInformation.playerIsHost)
                            currentPlayerMessage.text = "Opponent's Move";
                        else
                            currentPlayerMessage.text = "Your Move";
                        GameInformation.openingSequence = false;
                        gameController.CollectCurrentPlayerResources();
                        playerResourcesManager.UpdateBothPlayersResources();
                        break;
                }
            }
            else
            {
                // Normal Gameplay
                if (GameInformation.currentPlayer == "HOST")
                    GameInformation.currentPlayer = "CLIENT";
                else
                    GameInformation.currentPlayer = "HOST";

                ToogleTriggers();
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
            }

            

            if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
            {
                GameInformation.gameOver = true;
                return;
            }

            // Increment the turnNumber and begin turn if your turn
            turnNumber++;
        }

        if (OpponentHasSentNewMoveToProcess())
        {
            GameInformation.newNetworkMoveSet = false;

            string opponentBoard = networkController.GetMove();
            gameController.SetBoardConfiguration(opponentBoard);
            gameController.RefreshBlockedTiles();
            boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
            boardManager.RefreshBoardGUI();
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
        if (IsCorrectHostOpeningMove())
        {
            gameController.UpdateGameBoard();
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if (GameInformation.playerIsHost)
            {
                networkController.SendMove(gameController.getGameBoard().ToString());
                networkController.SyncPlayerVariables(turnNumber, GameInformation.currentPlayer);
            }
        }
        else if (IsCorrectClientOpeningMove())
        {
            gameController.UpdateGameBoard();
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if (!GameInformation.playerIsHost)
            {
                networkController.SendMove(gameController.getGameBoard().ToString());
                networkController.SyncPlayerVariables(turnNumber, GameInformation.currentPlayer);
            }
        }
        else if(!GameInformation.openingSequence)
        {
            // NORMAL GAMEPLAY
            gameController.UpdateGameBoard();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().GetSquareStates());
            GameInformation.currentRoundPlacedNodes.Clear();
            GameInformation.currentRoundPlacedBranches.Clear();
            GameInformation.resourceTrade = false;

            networkController.SendMove(gameController.getGameBoard().ToString());
            networkController.SyncPlayerVariables(turnNumber, GameInformation.currentPlayer);
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
                if(turnNumber == 1)
                {
                    currentPlayerMessage.text = "AI's Move";
                    turnNumber++;
                    RandomAIOpeningMove();

                    GameInformation.humanMoveFinished = false;
                    EndCurrentAIPlayersTurn();
                }
                else if(turnNumber == 4)
                {
                    currentPlayerMessage.text = "AI's Move";
                    GameInformation.openingSequence = false;
                    turnNumber++;
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
                    boardManager.RefreshBoardGUI();
                    EndCurrentAIPlayersTurn();
                }
            }
            else
            {
                if(turnNumber == 2)
                {
                    currentPlayerMessage.text = "Your Move";
                    turnNumber++;
                    gameController.FlipColors();
                    BeginHumanOpeningMove();
                }
                else if(turnNumber == 3)
                {
                    currentPlayerMessage.text = "AI's Move";
                    turnNumber++;
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
        else
        {
            turnNumber++;
            gameController.UpdateGameBoard();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().GetSquareStates());
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

    public string ToStringResources(int[] resources)
    {
        return ("R" + resources[0].ToString() + "B" + resources[1].ToString() + "Y" + resources[2].ToString() + "G" + resources[3].ToString());
    }
}
