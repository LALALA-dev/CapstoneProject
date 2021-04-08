using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static GameObjectProperties;
using System;
using static ExpertAI;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NetworkController networkController;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PlayerResourcesManager playerResourcesManager;
    private GameController gameController;
    private BeginnerAI beginnerAI;
    private ExpertAI expertAI;
    public TextMeshProUGUI playerLeftMessage;

    public TMP_InputField HNPInput;
    public TMP_Text longestNetworkPlayerText;
    public TMP_Text longestNetworkLengthText;
    public GameObject CompleteTurnBtn;
    public GameObject TradeBtn;
    public GameObject longestNetworkMessage;
    public GameObject playerLeftErrorMessage;
    public GameObject generalErrorMessage;
    public GameObject badOpeningMove;
    public Image playerOneAvatar;
    public Image playerTwoAvatar;
    public Text playerOneName;
    public Text playerTwoName;
    public Text playerOneScore;
    public Text playerTwoScore;
    public Text currentPlayerMessage;

    public Sprite[] avatars;
    public GameObject waitingAnimation;

    public int turnNumber = 1;

    public AudioSource resoureAllocation;
    public AudioSource whistle;
    public AudioSource button;

    #region Setup
    private void Awake()
    {
        gameController = GameController.getInstance();

        longestNetworkMessage.SetActive(false);
        generalErrorMessage.SetActive(false);
        playerLeftErrorMessage.SetActive(false);
        badOpeningMove.SetActive(false);

        if (!GameInformation.HumanNetworkProtocol)
            HNPInput.gameObject.SetActive(false);

        if(!GameInformation.playerIsHost) {
            currentPlayerMessage.text = "Opponent's Move";
            playerOneName.text = "Opponent";
            playerTwoName.text = "You";
        }

        if (GameInformation.playerIsHost) {
            waitingAnimation.SetActive(false);
            playerOneName.text = "You";
            playerTwoName.text = "Opponent";
        }
    }

    void Start()
    {
        if (GameInformation.playerIsHost && GameInformation.gameType == 'N')
        {
            GameInformation.currentPlayer = "HOST";
            networkController.SendOpeningBoardConfiguration(gameController.getGameBoard().ToString());
            networkController.SendAvatar(GameInformation.playerOneAvatar);
            BeginNetworkGame();
        }
        else if(GameInformation.gameType == 'A')
        {
            BeginBeginnerAIGame();
        }
        else if(GameInformation.gameType == 'E')
        {
            BeginExpertAIGame();
        }
        else if (GameInformation.HumanNetworkProtocol)
        {
            HNPInput.gameObject.SetActive(true);
        }

        if(GameInformation.playerIsHost)
        {
            GameInformation.playerOneAvatar = GameInformation.ownAvatar;
            GameInformation.playerTwoAvatar = GameInformation.aiAvatar;
        }
        else
        {
            GameInformation.playerTwoAvatar = GameInformation.ownAvatar;
            GameInformation.playerOneAvatar = GameInformation.aiAvatar;
        }

        if(GameInformation.gameType != 'N')
            SetAvatars();

    }
    #endregion

    public void ButtonClick()
    {
        button.Play();
    }

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
            resoureAllocation.Play();
        }

        if (GameInformation.gameType == 'N' && PhotonNetwork.CurrentRoom.PlayerCount < 2 && !GameInformation.gameOver)
        {
            whistle.Play();
            playerLeftErrorMessage.SetActive(true);
            CompleteTurnBtn.SetActive(false);
            TradeBtn.SetActive(false);
        }

        if (NeedToSyncNetworkGameVariables())
        {
            GameInformation.needToSyncGameVariables = false;

            GameInformation.currentPlayer = networkController.GetCurrentPlayer();
            turnNumber = networkController.GetTurnNumber();

            if(!IsTheCurrentPlayerYourself())
            {
                // parse resource string and update local opponent's resources
                string incomingPlayersResources = networkController.GetOpponentResources();
                int[] playersResources = DeStringResources(incomingPlayersResources);

                if (GameInformation.currentPlayer == "CLIENT")
                    GameInformation.playerTwoResources = playersResources;
                else
                    GameInformation.playerOneResources = playersResources;

                playerResourcesManager.UpdateBothPlayersResources();
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
                        {
                            currentPlayerMessage.text = "Opponent's Move";
                            waitingAnimation.SetActive(true);
                        } 
                        else
                        {
                            currentPlayerMessage.text = "Your Move";
                            waitingAnimation.SetActive(false);
                        }
                        break;
                    case 2:
                        GameInformation.currentPlayer = "CLIENT";
                        if (GameInformation.playerIsHost)
                        {
                            currentPlayerMessage.text = "Opponent's Move";
                            waitingAnimation.SetActive(true);
                        }
                        else
                        {
                            currentPlayerMessage.text = "Your Move";
                            waitingAnimation.SetActive(false);
                        }
                        break;
                    case 3:
                        ToogleTriggers();
                        GameInformation.currentPlayer = "HOST";
                        if (GameInformation.playerIsHost)
                        {
                            currentPlayerMessage.text = "Your Move";
                            waitingAnimation.SetActive(false);
                        }
                        else
                        {
                            currentPlayerMessage.text = "Opponent's Move";
                            waitingAnimation.SetActive(true);
                        }
                        break;
                    case 4:
                        ToogleTriggers();
                        GameInformation.currentPlayer = "CLIENT";
                        if (GameInformation.playerIsHost)
                        {
                            currentPlayerMessage.text = "Opponent's Move";
                            waitingAnimation.SetActive(true);
                        }
                        else
                        {
                            currentPlayerMessage.text = "Your Move";
                            waitingAnimation.SetActive(false);
                            resoureAllocation.Play();
                        }
                        GameInformation.openingSequence = false;
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

                if (!IsTheCurrentPlayerYourself())
                {
                    currentPlayerMessage.text = "Opponent's Move";
                    waitingAnimation.SetActive(true);
                }   
                else
                {
                    resoureAllocation.Play();
                    currentPlayerMessage.text = "Your Move";
                    waitingAnimation.SetActive(false);
                }
                ToogleTriggers();
            }

            turnNumber++;
        }

        if (OpponentHasSentNewMoveToProcess())
        {
            GameInformation.newNetworkMoveSet = false;
            string opponentBoard = networkController.GetMove();
            gameController.SetBoardConfiguration(opponentBoard);
            gameController.UpdateGameBoard();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
            boardManager.RefreshBoardGUI();

            if(!GameInformation.openingSequence)
            {
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();

                if(GameInformation.playerIsHost)
                    networkController.SendCurrentPlayersResources(ToStringResources(GameInformation.playerOneResources));
                else
                    networkController.SendCurrentPlayersResources(ToStringResources(GameInformation.playerTwoResources));

                gameController.UpdateScores();
                UpdateScoresUI();

                if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
                {
                    GameInformation.gameOver = true;
                    return;
                }
            }
            else if(GameInformation.playerIsHost && turnNumber == 5)
            {
                gameController.UpdateScores();

                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
        }

        if(GameInformation.needToUpdateOpponentsResources)
        {
            GameInformation.needToUpdateOpponentsResources = false;
            string resources = networkController.GetOpponentResources();
            int[] parsedResources = DeStringResources(resources);

            if (GameInformation.playerIsHost)
            {
                GameInformation.playerTwoResources = parsedResources;
            }
            else
            {
                GameInformation.playerOneResources = parsedResources;
            }
            playerResourcesManager.UpdateBothPlayersResources();
        }

        if(GameInformation.needToSyncAvatars)
        {
            GameInformation.needToSyncAvatars = false;
            if (GameInformation.playerIsHost)
                GameInformation.playerTwoAvatar = networkController.GetOpponentInfo();
            else
                GameInformation.playerOneAvatar = networkController.GetOpponentInfo();
            SetAvatars();
        }

        // Update in-game help popup ordering.
        if (turnNumber > 4 && turnNumber < 9)
        {
            BroadcastMessage("SetPanelOrderTurnFive");
        }
        else if (turnNumber > 8)
        {
            BroadcastMessage("SetPanelOrderTurnNine");
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
            networkController.SendAvatar(GameInformation.playerTwoAvatar);
        }
    }

    private void EndCurrentNetworkPlayersTurn()
    {
        if (IsTheCurrentPlayerYourself())
        {
            if (IsCorrectHostOpeningMove())
            {
                boardManager.SolidifyNodeSelections(GameInformation.openingNodeId);
                boardManager.SolidifyBranchSelection(GameInformation.openingBranchId);
                gameController.UpdateGameBoard();
                gameController.RefreshBlockedTiles();
                boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);

                if (turnNumber == 4)
                {
                    gameController.UpdateScores();
                    UpdateScoresUI();
                }

                if (GameInformation.playerIsHost)
                {
                    networkController.SendMove(gameController.getGameBoard().ToString());
                    networkController.SyncPlayerVariables(turnNumber, GameInformation.currentPlayer, "0 0 0 0");
                }
            }
            else if (IsCorrectClientOpeningMove())
            {
                boardManager.SolidifyNodeSelections(GameInformation.openingNodeId);
                boardManager.SolidifyBranchSelection(GameInformation.openingBranchId);
                gameController.UpdateGameBoard();
                gameController.RefreshBlockedTiles();
                boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
                if (!GameInformation.playerIsHost)
                {
                    networkController.SendMove(gameController.getGameBoard().ToString());
                    networkController.SyncPlayerVariables(turnNumber, GameInformation.currentPlayer, "0 0 0 0");
                }
            }
            else if (!GameInformation.openingSequence)
            {
                // NORMAL GAMEPLAY
                gameController.UpdateGameBoard();
                boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);

                for (int i = 0; i < GameInformation.currentRoundPlacedNodes.Count; i++)
                {
                    boardManager.SolidifyNodeSelections(GameInformation.currentRoundPlacedNodes[i]);
                }
                for (int i = 0; i < GameInformation.currentRoundPlacedBranches.Count; i++)
                {
                    boardManager.SolidifyBranchSelection(GameInformation.currentRoundPlacedBranches[i]);
                }
                GameInformation.currentRoundPlacedNodes.Clear();
                GameInformation.currentRoundPlacedBranches.Clear();
                GameInformation.resourceTrade = false;

                int[] resources = new int[4];
                if (GameInformation.playerIsHost)
                    resources = GameInformation.playerOneResources;
                else
                    resources = GameInformation.playerTwoResources;

                gameController.UpdateScores();
                UpdateScoresUI();

                if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
                {
                    GameInformation.gameOver = true;
                }

                networkController.SendMove(gameController.getGameBoard().ToString());
                networkController.SyncPlayerVariables(turnNumber, GameInformation.currentPlayer, ToStringResources(resources));
            }
            else if (GameInformation.openingSequence)
            {
                whistle.Play();
                badOpeningMove.SetActive(true);
            }
        }
    }

    #endregion

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

        if(!GameInformation.playerIsHost)
        {
            currentPlayerMessage.text = "AI's Move";
            waitingAnimation.SetActive(true);
            BoardState AIMove = beginnerAI.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
            gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
            EndCurrentAIPlayersTurn();
        }
        else
        {
            currentPlayerMessage.text = "Your Move";
            waitingAnimation.SetActive(false);
        }
    }

    public void BeginExpertAIGame()
    {
        PlayerColor aiColor;
        int[] resources = new int[] { 0, 0, 0, 0 };
        if (GameInformation.playerIsHost)
        {
            aiColor = PlayerColor.Gold;
        }
        else
        {
            aiColor = PlayerColor.Silver;
        }
        boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        expertAI = new ExpertAI();

        if (!GameInformation.playerIsHost)
        {
            currentPlayerMessage.text = "AI's Move";
            waitingAnimation.SetActive(true);
            AI move = new AI(aiColor, gameController.getGameBoard().getBoardState(), resources, resources);
            BoardState AIMove = move.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
            gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
            EndCurrentAIPlayersTurn();
        }
        else
        {
            currentPlayerMessage.text = "Your Move";
            waitingAnimation.SetActive(false);
        }
    }

    public void BeginHumanOpeningMove()
    {
        GameInformation.openingMoveBranchSet = false;
        GameInformation.openingMoveNodeSet = false;
        GameInformation.currentPlayer = "HUMAN";
        CompleteTurnBtn.GetComponent<Button>().interactable = true;
        TradeBtn.GetComponent<Button>().interactable = true;
        gameController.FlipColors();
        GameInformation.humanMoveFinished = false;
    }

    private async void EndCurrentAIPlayersTurn()
    {
        if (GameInformation.openingSequence && GameInformation.currentPlayer == "HUMAN" && OpeningMoveSatisfied())
        {
            boardManager.SolidifyNodeSelections(GameInformation.openingNodeId);
            boardManager.SolidifyBranchSelection(GameInformation.openingBranchId);
            gameController.RefreshBlockedTiles();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            if(GameInformation.playerIsHost)
            {
                if(turnNumber == 1)
                {
                    currentPlayerMessage.text = "AI's Move";
                    waitingAnimation.SetActive(true);
                    turnNumber++;
                    AIOpeningMove();

                    GameInformation.humanMoveFinished = false;
                    EndCurrentAIPlayersTurn();
                }
                else if(turnNumber == 4)
                {
                    currentPlayerMessage.text = "AI's Move";
                    waitingAnimation.SetActive(true);
                    GameInformation.openingSequence = false;
                    turnNumber++;
                    GameInformation.currentPlayer = "AI";
                    CompleteTurnBtn.GetComponent<Button>().interactable = false;
                    TradeBtn.GetComponent<Button>().interactable = false;
                    gameController.FlipColors();

                    gameController.CollectCurrentPlayerResources();
                    gameController.UpdateScores();
                    UpdateScoresUI();

                    BoardState AIMove;
                    PlayerColor aiColor = PlayerColor.Gold;
                    int[] PlayerResources;
                    int[] AIResources;
                    if (!GameInformation.playerIsHost)
                    {
                        aiColor = PlayerColor.Silver;
                        AIResources = GameInformation.playerOneResources;
                        PlayerResources = GameInformation.playerTwoResources;
                    }
                    else
                    {
                        AIResources = GameInformation.playerTwoResources;
                        PlayerResources = GameInformation.playerOneResources;
                    }

                    if (GameInformation.gameType == 'A')
                    {
                        AIMove = await beginnerAI.RandomMove(gameController.getGameBoard().getBoardState(), AIResources);
                    }
                    else
                    {
                        AI expertMove = new AI(aiColor, gameController.getGameBoard().getBoardState(), AIResources, PlayerResources);
                        AIMove = await expertMove.findNextMove(5.5);
                    }

                    playerResourcesManager.UpdateBothPlayersResources();
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
                    waitingAnimation.SetActive(false);
                    turnNumber++;
                    gameController.FlipColors();
                    BeginHumanOpeningMove();
                }
                else if(turnNumber == 3)
                {
                    currentPlayerMessage.text = "AI's Move";
                    waitingAnimation.SetActive(true);
                    turnNumber++;
                    AIOpeningMove();
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
                    waitingAnimation.SetActive(false);
                    BeginHumanOpeningMove();
                }
                else if (turnNumber == 4)
                {
                    currentPlayerMessage.text = "Your Move";
                    waitingAnimation.SetActive(false);
                    GameInformation.openingSequence = false;
                    turnNumber++;
                    GameInformation.currentPlayer = "HUMAN";
                    CompleteTurnBtn.GetComponent<Button>().interactable = true;
                    TradeBtn.GetComponent<Button>().interactable = true;
                    GameInformation.humanMoveFinished = false;
                    gameController.FlipColors();
                    gameController.CollectCurrentPlayerResources();
                    resoureAllocation.Play();
                    gameController.UpdateScores();
                    playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                    playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
                    playerResourcesManager.UpdateBothPlayersResources();
                }
            }
            else
            {
                if (turnNumber == 2)
                {
                    currentPlayerMessage.text = "AI's Move";
                    waitingAnimation.SetActive(true);
                    turnNumber++;
                    AIOpeningMove();
                    GameInformation.humanMoveFinished = false;
                    EndCurrentAIPlayersTurn();
                }
                else if (turnNumber == 3)
                {
                    currentPlayerMessage.text = "Your Move";
                    waitingAnimation.SetActive(false);
                    turnNumber++;
                    GameInformation.currentPlayer = "HUMAN";
                    CompleteTurnBtn.GetComponent<Button>().interactable = true;
                    TradeBtn.GetComponent<Button>().interactable = true;
                    gameController.FlipColors();

                    GameInformation.humanMoveFinished = false;
                    BeginHumanOpeningMove();
                }
            }
        }
        else if(!GameInformation.openingSequence)
        {
            turnNumber++;
            gameController.UpdateGameBoard();
            boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
            boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);

            GameInformation.resourceTrade = false;
            if (GameInformation.currentPlayer == "HUMAN")
            {
                for(int i = 0; i <  GameInformation.currentRoundPlacedNodes.Count; i++)
                {
                    boardManager.SolidifyNodeSelections(GameInformation.currentRoundPlacedNodes[i]);
                }
                for (int i = 0; i < GameInformation.currentRoundPlacedBranches.Count; i++)
                {
                    boardManager.SolidifyBranchSelection(GameInformation.currentRoundPlacedBranches[i]);
                }
                GameInformation.currentRoundPlacedNodes.Clear();
                GameInformation.currentRoundPlacedBranches.Clear();
                GameInformation.currentPlayer = "AI";
                CompleteTurnBtn.GetComponent<Button>().interactable = false;
                TradeBtn.GetComponent<Button>().interactable = false;
                currentPlayerMessage.text = "AI's Move";
                waitingAnimation.SetActive(true);
            }
            else
            {
                GameInformation.currentPlayer = "HUMAN";
                CompleteTurnBtn.GetComponent<Button>().interactable = true;
                TradeBtn.GetComponent<Button>().interactable = true;
                currentPlayerMessage.text = "Your Move";
                waitingAnimation.SetActive(false);
                resoureAllocation.Play();
            }
            gameController.FlipColors();
            gameController.CollectCurrentPlayerResources();
            playerResourcesManager.UpdateBothPlayersResources();
            gameController.UpdateScores();
            UpdateScoresUI();

            if (GameInformation.playerOneScore >= 10 || GameInformation.playerTwoScore >= 10)
            {
                GameInformation.gameOver = true;
                return;
            }

            if (GameInformation.currentPlayer == "AI")
            {
                if (GameInformation.gameType == 'E')
                {
                    PlayerColor aiColor = PlayerColor.Gold;
                    int[] PlayerResources;
                    int[] AIResources;
                    if (!GameInformation.playerIsHost)
                    {
                        aiColor = PlayerColor.Silver;
                        AIResources = GameInformation.playerOneResources;
                        PlayerResources = GameInformation.playerTwoResources;
                    }
                    else
                    {
                        AIResources = GameInformation.playerTwoResources;
                        PlayerResources = GameInformation.playerOneResources;
                    }

                    AI expertMove = new AI(aiColor, gameController.getGameBoard().getBoardState(), AIResources, PlayerResources);
                    BoardState AIMove = await expertMove.findNextMove(5.5);
                    gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
                    boardManager.RefreshBoardGUI();
                }
                else
                {
                    await RandomAIMove();
                }
                    
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
        else if (GameInformation.openingSequence)
        {
            whistle.Play();
            badOpeningMove.SetActive(true);
        }
    }

    public void AIOpeningMove()
    {
        GameInformation.currentPlayer = "AI";
        CompleteTurnBtn.GetComponent<Button>().interactable = false;
        TradeBtn.GetComponent<Button>().interactable = false;
        gameController.FlipColors();
        BoardState AIMove;
        if(GameInformation.gameType == 'A')
            AIMove = beginnerAI.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
        else
        {
            PlayerColor aiColor = PlayerColor.Gold;
            int[] PlayerResources;
            int[] AIResources;
            if (!GameInformation.playerIsHost)
            {
                aiColor = PlayerColor.Silver;
                AIResources = GameInformation.playerOneResources;
                PlayerResources = GameInformation.playerTwoResources;
            }
            else
            {
                AIResources = GameInformation.playerTwoResources;
                PlayerResources = GameInformation.playerOneResources;
            }

            AI expertMove = new AI(aiColor, gameController.getGameBoard().getBoardState(), AIResources, PlayerResources);
            AIMove = expertMove.MakeRandomOpeningMove(gameController.getGameBoard().getBoardState());
        }
            
        gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
        boardManager.RefreshBoardGUI();
    }

    public async Task RandomAIMove()
    {
        int[] AIResources;
        if (!GameInformation.playerIsHost)
            AIResources = GameInformation.playerOneResources;
        else
            AIResources = GameInformation.playerTwoResources;

        BoardState AIMove = await beginnerAI.RandomMove(gameController.getGameBoard().getBoardState(), AIResources);
        gameController.getGameBoard().setBoard(AIMove.squareStates, AIMove.nodeStates, AIMove.branchStates);
        boardManager.RefreshBoardGUI();
    }

    #endregion

    public void UpdateResourcesUI()
    {
        playerResourcesManager.UpdateBothPlayersResources();
    }

    public void UpdateScoresUI()
    {
        playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
        playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

        if (GameInformation.playerOneNetwork > GameInformation.playerTwoNetwork)
        {
            longestNetworkPlayerText.text = "Player One";
            longestNetworkLengthText.text = GameInformation.playerOneNetwork.ToString() + " Roads";
            longestNetworkMessage.SetActive(true);
            longestNetworkMessage.transform.position = new Vector3(620f, 885f, 0f);
        }
        else if (GameInformation.playerTwoNetwork > GameInformation.playerOneNetwork)
        {
            longestNetworkPlayerText.text = "Player Two";
            longestNetworkLengthText.text = GameInformation.playerTwoNetwork.ToString() + " Roads";
            longestNetworkMessage.SetActive(true);
            longestNetworkMessage.transform.position = new Vector3(1250f, 885f, 0f);
        }
        else
        {
            longestNetworkMessage.SetActive(false);
        }

        BroadcastMessage("UpdateHelpPopupScores");
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
        FlipTradeAndCompleteTurnActive();
        BroadcastMessage("ToggleNodeBranchTriggers");
    }

    public void FlipTradeAndCompleteTurnActive()
    {
        if (TradeBtn.GetComponent<Button>().interactable)
            TradeBtn.GetComponent<Button>().interactable = false;
        else
            TradeBtn.GetComponent<Button>().interactable = true;
        
        if (CompleteTurnBtn.GetComponent<Button>().interactable)
            CompleteTurnBtn.GetComponent<Button>().interactable = false;
        else
            CompleteTurnBtn.GetComponent<Button>().interactable = true;
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
        return (resources[0].ToString() + " " + resources[1].ToString() + " " + resources[2].ToString() + " " + resources[3].ToString());
    }

    public int[] DeStringResources(string resources)
    {
        string[] parsedResources = resources.Split(' ');

        int[] opponentsResources = new int[4];

        opponentsResources[0] = int.Parse(parsedResources[0]);
        opponentsResources[1] = int.Parse(parsedResources[1]);
        opponentsResources[2] = int.Parse(parsedResources[2]);
        opponentsResources[3] = int.Parse(parsedResources[3]);

        return opponentsResources;
    }

    public void SetAvatars()
    {
        switch (GameInformation.playerOneAvatar)
        {
            case "HAT":
                playerOneAvatar.sprite = avatars[0];
                break;
            case "BATTLESHIP":
                playerOneAvatar.sprite = avatars[1];
                break;
            case "CAR":
                playerOneAvatar.sprite = avatars[2];
                break;
            case "THIMBLE":
                playerOneAvatar.sprite = avatars[3];
                break;
            case "WHEELBARREL":
                playerOneAvatar.sprite = avatars[4];
                break;
            default:
                playerOneAvatar.sprite = avatars[2];
                break;
        }

        switch (GameInformation.playerTwoAvatar)
        {
            case "HAT":
                playerTwoAvatar.sprite = avatars[5];
                break;
            case "BATTLESHIP":
                playerTwoAvatar.sprite = avatars[6];
                break;
            case "CAR":
                playerTwoAvatar.sprite = avatars[7];
                break;
            case "THIMBLE":
                playerTwoAvatar.sprite = avatars[8];
                break;
            case "WHEELBARREL":
                playerTwoAvatar.sprite = avatars[9];
                break;
            default:
                playerTwoAvatar.sprite = avatars[9];
                break;
        }
    }

    public void OnCancelButtonClick(GameObject gb)
    {
        gb.SetActive(false);
    }
}
