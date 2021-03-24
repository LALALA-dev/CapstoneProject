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

    public SceneLoader sl;

    private string[] infoOneMessages = new string[]
    {
        "Nodopoly is a 2 player strategy game, pinning players against one another for real-estate dominance of their city",
        "The cityscape is randomly generate at the start of each game",
        "Each player gets 2 of their avatar game pieces, nodes, and 2 houses at the start of the game",
        "Game pieces can be placed on the corners of the streets throughout city",
        "Houses go on the streets themselves, building your real-estate network",
        "For the first two rounds for each player, the game piece-house combo can go anywhere in the city...",
        "Every piece collects a rent from all non-foreclosed properties it touches, this is done at the start of every turn",
        "This game piece collects rent from 1 green, 1 red, and 1 yellow property",
        "And this game piece collects rent from 1 green, 1 yellow, and 1 blue property",
        "",
        "Players use collected rent to build additional game pieces and houses",
        "Tap the street corner of the property to place a new game piece",
        "Now it's the gold player's turn, they recieve their rent from their properties",
        "Houses are legally connected even if they oass through an opponent's game piece",
        "The gold player now submits their move...",
        "The silver player gets rent from all their placed game pieces",
        "The stars on a property represent the max amount of game pieces that can be touching it",
        "This property supports 1 game piece",
        "Foreclosed properties get marked with the foreclosed stamp only after a player submits a move",
        "Surrounding a property with houses of the same player will monopolize the property for that player",
        "Like foreclosed properties, monopolized properties are marked after a player submits a move",
        "You can even monopolize multiple squares at once if no opponent game pieces or houses inside the attempted area",
        "Once monopolized, opponents cannot build houses through that area",
        "If you don't have the right mix of rent, you can trade for it",
        "Once per turn, you may trade any 3 types of rent for one of another color",
        "Now Gold has the rent required to build another house",
        "The goal of Nodopoly is to be the first to 10 points",
        "1 point is rewared for each game piece placed and monopolized property. 2 points are rewared for having the longest chain of houses",
        "All 7 of the Silver player's houses are connected... While the Gold player has 9 houses placed, but not all connected",
    };

    private string[] infoTwoMessages = new string[]
    {
        "Click the green arrow to proceed, click the red arrow to go back",
        "",
        "",
        "Place the game piece on the street corner",
        "Place the house on the street to connect to your game piece",
        "...but the opening two moves, all game pieces and houses must connect to your already established network via houses",
        "The color of rent matches the color of the property where it came from",
        " ",
        " ",
        "Meaning that you've collected rent in the form of 2 green, 2 yellow, 1 blue, and 1 red for this turn",
        "A house costs 1 red and 1 blue, while a game piece costs 2 yellow, and 2 green",
        " ",
        " ",
        "And you may build additional houses without any game pieces on them",
        "...and any unused rent are saved for future rounds",
        "Including from the new game piece they placed last turn",
        "If more game pieces are on property than it's star amount, it forecloses and stops paying rent out to either player",
        "But the silver player is about to place a 2nd game piece on it, which will foreclose it",
        "Remember, game pieces from both players count towards the total limit!",
        "Monopolizing a property builds a hotel on that property and stop paying rent to the opposing player",
        " ",
        " ",
        "Gold cannot build through the Silver perimeter now that these properties are monopolized",
        "Click here to open the trade menu",
        " ",
        " ",
        "Go to the info screen to see a score rules",
        " ",
        "This makes the Gold player's longest network 5; and therefore, Silver has the longest house network because 7 > 5",
    };


    public Text playerOneScore;
    public Text playerTwoScore;
    public Text currentPlayerMessage;

    public Text infoOne;
    public Text infoTwo;

    public Sprite[] avatars;

    public int turnNumber = 1;

    public int messageNumber = 0;

    public GameObject tutorialPanel;

    public NodeController openingNode;
    public BranchController openingBranch;

    public Sprite tutorialSprite;

    #region Setup
    private void Awake()
    {
        gameController = GameController.getInstance();

        playerOneAvatar.sprite = avatars[0];
        playerTwoAvatar.sprite = avatars[1];

        infoOne.text = infoOneMessages[messageNumber];
        infoTwo.text = infoTwoMessages[messageNumber];
    }

    void Start()
    {
        ToogleTriggers();
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

        GameInformation.HumanNetworkProtocol = true;
        gameController.SetBoardConfiguration("1R1G3R2B2G3Y0L3G1Y3B2R2Y1B");
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

    public void OnForwardClick()
    {
        messageNumber++;
        if (messageNumber <= 28)
        {
            infoOne.text = infoOneMessages[messageNumber];
            infoTwo.text = infoTwoMessages[messageNumber];

            if (messageNumber == 1)
            {
                tutorialPanel.SetActive(false);
                SpriteRenderer sprite = openingNode.GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialSprite;
                openingNode.ToggleTrigger();
            }
        }
        else
        {
            sl.LoadMenuScene();
        }
    }

    public void OnBackwardClick()
    {
        messageNumber--;
        if(messageNumber >= 0)
        {
            infoOne.text = infoOneMessages[messageNumber];
            infoTwo.text = infoTwoMessages[messageNumber];

            if (messageNumber == 0)
                tutorialPanel.SetActive(true);
        }
        else
        {
            sl.LoadMenuScene();
        }
    }
}
