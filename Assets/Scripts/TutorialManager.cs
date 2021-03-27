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

    [SerializeField] private Image playerOneAvatar;
    [SerializeField] private Image playerTwoAvatar;

    public SceneLoader sl;

    private string[] infoOneMessages = new string[]
    {
        "Nodopoly is a 2 player strategy game, pitting players against one another for real-estate dominance of their city",
        "The cityscape is randomly generated at the start of each game",
        "Each player gets 2 of their avatar game pieces, and 2 roads at the start of the game",
        "Game pieces can be placed on the corners of the streets throughout city",
        "Houses go on the streets themselves, building your real-estate network",
        "Now it's player 2's turn, they get to place two in a round",
        "For the first two rounds for each player, the game piece-road combo can go anywhere in the city...",
        "It's now player 2's turn",
        "Every piece collects a rent from all non-foreclosed properties it touches, this is done at the start of every turn",
        "This game piece collects rent from 1 green, 1 red, and 1 yellow property",
        "And this game piece collects rent from 1 green, 1 yellow, and 1 blue property",
        "",
        "Players use collected rent to build additional game pieces and roads",
        "Tap the street corner of the property to place a new game piece",
        "Now it's the gold player's turn, they recieve their rent from their properties",
        "Houses are legally connected even if they pass through an opponent's game piece",
        "The gold player now submits their move...",
        "The silver player gets rent from all their placed game pieces",
        "Build the highlighted roads and game pieces",
        "The stars on a property represent the max amount of game pieces that can be touching it",
        "This property supports 1 game piece",
        "Foreclosed properties get marked with the foreclosed stamp only after a player submits a move",
        "Select GO to end your turn",
        "Surrounding a property with roads of the same player will monopolize the property for that player",
        "Like foreclosed properties, monopolized properties are marked after a player submits a move",
        "You can even monopolize multiple squares at once if no opponent game pieces or roads inside the attempted area",
        "Once monopolized, opponents cannot build roads through that area",
        "If you don't have the right mix of rent, you can trade for it",
        "Once per turn, you may trade any 3 types of rent for one of another color",
        "Now you have the rent required to build 2 more game pieces",
        "Place game pieces on the highlighted street corners",
        "The goal of Nodopoly is to be the first to 10 points",
        "1 point is rewared for each game piece placed and monopolized property. 2 points are rewared for having the longest chain of roads",
        "All 7 of the Silver player's roads are connected... While the Gold player has 9 roads placed, but not all connected",
    };
    private string[] infoTwoMessages = new string[]
    {
        "Click the green arrow to proceed, click the red arrow to go back",
        "",
        "The first two rounds goes player 1 then player 2 twice, and then player 1 and it alternates",
        "Place the game piece on the street corner",
        "Place the road on the street to connect to your game piece, press GO to submit your move",
        "",
        "...but after the first two moves, all game pieces and roads must connect to your already established network",
        "They collect their rent and make their move, they complete the turn to pass it back to you",
        "The color of rent matches the color of the property where it came from",
        " ",
        " ",
        "Meaning that you've collected rent in the form of 2 green, 2 yellow, 1 blue, and 1 red for this turn",
        "A road costs 1 red and 1 blue, while a game piece costs 2 yellow, and 2 green",
        " ",
        " ",
        "And you may build additional roads without any game pieces on them",
        "...and any unused rent are saved for future rounds",
        "Including from the new game piece they placed last turn",
        "",
        "If more game pieces are on property than it's star amount, it forecloses and stops paying rent out to either player",
        "But the silver player is about to place a 2nd game piece on it, which will foreclose it",
        "Remember, game pieces from both players count towards the total limit!",
        "",
        "Monopolizing a property builds a hotel on that property and stop paying rent to the opposing player",
        " ",
        " ",
        "Gold cannot build through the Silver perimeter now that these properties are monopolized",
        "Click here to open the trade menu",
        " ",
        " ",
        "",
        "Go to the info screen to see a score rules",
        " ",
        "This makes the Gold player's longest network 5; and therefore, Silver has the longest road network because 7 > 5",
    };


    public Text playerOneScore;
    public Text playerTwoScore;
    public Text currentPlayerMessage;
    public Text infoOne;
    public Text infoTwo;

    public Sprite[] avatars;
    private int messageNumber = 0;

    public GameObject tutorialPanel;
    public Button goBtn;
    public Button tradeBtn;
    public Button infoBtn;
    public Button forwardBtn;

    public NodeController[] tutorialNodes;
    public BranchController[] tutorialBranches;
    public Square[] tutorialTiles;

    public ArrowController arrowOne;
    public ArrowController arrowTwo;

    public Sprite tutorialSprite;
    public Sprite tutorialBranchSprite;

    #region Setup
    private void Awake()
    {
        gameController = GameController.getInstance();

        playerOneAvatar.sprite = avatars[0];
        playerTwoAvatar.sprite = avatars[1];

        infoOne.text = infoOneMessages[messageNumber];
        infoTwo.text = infoTwoMessages[messageNumber];

        goBtn.interactable = false;
        tradeBtn.interactable = false;
        infoBtn.interactable = false;
    }

    void Start()
    {
        ToogleTriggers();
        BeginTutorial();
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

    public void UpdateResourcesUI()
    {
        playerResourcesManager.UpdateBothPlayersResources();
    }


    public void BeginTutorial()
    {

        GameInformation.HumanNetworkProtocol = true;
        gameController.SetBoardConfiguration("1R1G3R2B2G3Y0L3G1Y3B2R2Y1B");
        boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        currentPlayerMessage.text = "Your Move";
    }

    public void ToogleTriggers()
    {
        BroadcastMessage("ToggleNodeBranchTriggers");
    }

    public void OnForwardClick()
    {
        messageNumber++;
        if (messageNumber < infoOneMessages.Length)
        {
            infoOne.text = infoOneMessages[messageNumber];
            infoTwo.text = infoTwoMessages[messageNumber];

            if (messageNumber == 3)
            {
                tutorialPanel.SetActive(false);
                HighlightNode(0);
            }
            else if(messageNumber == 4)
            {
                ClaimNode(0, PlayerColor.Silver, avatars[0]);
                HighlightBranch(0);
                goBtn.interactable = true;
            }
            else if(messageNumber == 5)
            {
                currentPlayerMessage.text = "Opponent's Move";
                goBtn.interactable = false;
                ClaimBranch(0, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);

                ClaimNode(1, PlayerColor.Gold, avatars[1]);
                ClaimNode(2, PlayerColor.Gold, avatars[1]);
                ClaimBranch(1, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(2, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
            }
            else if(messageNumber == 6)
            {
                currentPlayerMessage.text = "Your Move";
                GameInformation.openingMoveNodeSet = false;
                GameInformation.openingMoveBranchSet = false;

                HighlightNode(3);
                HighlightBranch(3);
            }
            else if (messageNumber == 7)
            {
                ClaimNode(3, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                ClaimBranch(3, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);

                GameInformation.openingSequence = false;
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                gameController.UpdateScores();

                playerOneScore.text = "Score: 2";
                playerTwoScore.text = "Score: 2";

                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;
                ClaimBranch(4, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);

                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if (messageNumber == 8)
            {
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: 2";
                playerTwoScore.text = "Score: 2";
            }
            else if (messageNumber == 9)
            {
                arrowOne.EnableArrow(3.0f, 2.0f, 2.7f, -4.35f, 0, 0, -25.5f, .001f);
            }
            else if(messageNumber == 10)
            {
                arrowOne.DisableArrow();
            }
            else if (messageNumber == 13)
            {
                HighlightNode(4);
            }
            else if (messageNumber == 14)
            {
                ClaimNode(4, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
            }
            else if (messageNumber == 15)
            {
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;

                ClaimBranch(5, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;

                ClaimBranch(6, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;

                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if(messageNumber == 17)
            {
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 18)
            {
                HighlightNode(5);

                HighlightBranch(7);
                HighlightBranch(8);
                HighlightBranch(9);
            }
            else if(messageNumber == 19)
            {
                ClaimNode(5, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                ClaimBranch(7, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(8, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(9, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                GameInformation.currentRoundPlacedBranches.Clear();
                GameInformation.currentRoundPlacedNodes.Clear();
            }
            else if(messageNumber == 22)
            {
                goBtn.interactable = true;
            }
            else if(messageNumber == 23)
            {
                goBtn.interactable = false;
                tutorialTiles[0].squareState.resourceState = SquareStatus.Blocked;
                gameController.UpdateGameBoard();
                boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 24)
            {
                ClaimBranch(10, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(11, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;
                GameInformation.playerTwoResources[1]--;
                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if (messageNumber == 25)
            {
                tutorialTiles[1].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[1].squareState.ownerColor = PlayerColor.Gold;
                gameController.UpdateGameBoard();
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

                HighlightBranch(12);
            }
            else if (messageNumber == 26)
            {
                tutorialTiles[0].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[0].squareState.ownerColor = PlayerColor.Silver;
                tutorialTiles[2].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[2].squareState.ownerColor = PlayerColor.Silver;
                gameController.UpdateGameBoard();
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
            }
            else if (messageNumber == 27)
            {
                tradeBtn.interactable = true;
                forwardBtn.interactable = false;
            }
            else if (messageNumber == 28)
            {
                
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

            if (messageNumber == 2)
            {
                tutorialPanel.SetActive(true);
                UndoNode(0, PlayerColor.Blank, tutorialNodes[0].blankSprite);
            }
            else if (messageNumber == 3)
            {
                UndoNode(0, PlayerColor.Blank, tutorialSprite);
                UndoBranch(0, PlayerColor.Blank, tutorialBranches[0].blankSprite);
            }
            else if(messageNumber == 4)
            {

            }
            else if (messageNumber == 5)
            {

            }
            else if (messageNumber == 6)
            {

            }
            else if (messageNumber == 7)
            {

            }
            else if (messageNumber == 8)
            {

            }
            else if (messageNumber == 12)
            {

            }
            else if (messageNumber == 13)
            {

            }
            else if (messageNumber == 14)
            {

            }
            else if (messageNumber == 16)
            {

            }
            else if (messageNumber == 17)
            {

            }
            else if (messageNumber == 18)
            {

            }
            else if (messageNumber == 22)
            {
                
            }
        }
        else
        {
            sl.LoadMenuScene();
        }
    }

    public void HighlightBranch(int branchIndex)
    {
        SpriteRenderer sprite = tutorialBranches[branchIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = tutorialBranchSprite;
        tutorialBranches[branchIndex].ToggleTrigger();
    }

    public void HighlightNode(int nodeIndex)
    {
        SpriteRenderer sprite = tutorialNodes[nodeIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = tutorialSprite;
        tutorialNodes[nodeIndex].ToggleTrigger();
    }

    public void ClaimBranch(int branchIndex, PlayerColor playerColor, Sprite playerImage)
    {
        SpriteRenderer sprite = tutorialBranches[branchIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = playerImage;
        tutorialBranches[branchIndex].branchEntity.branchState.branchColor = playerColor;
        tutorialBranches[branchIndex].branchEntity.branchState.ownerColor = playerColor;
        tutorialBranches[branchIndex].ToggleTrigger();
    }

    public void ClaimNode(int nodeIndex, PlayerColor playerColor, Sprite playerAvatar)
    {
        SpriteRenderer sprite = tutorialNodes[nodeIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = playerAvatar;
        tutorialNodes[nodeIndex].nodeEntity.nodeState.nodeColor = playerColor;
        tutorialNodes[nodeIndex].ToggleTrigger();
    }

    public void UndoNode(int nodeIndex, PlayerColor color, Sprite Sprite)
    {
        SpriteRenderer sprite = tutorialNodes[nodeIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = Sprite;
        tutorialNodes[nodeIndex].nodeEntity.nodeState.nodeColor = color;
        tutorialNodes[nodeIndex].ToggleTrigger();
    }

    public void UndoBranch(int branchIndex, PlayerColor color, Sprite Sprite)
    {
        SpriteRenderer sprite = tutorialBranches[branchIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = Sprite;
        tutorialBranches[branchIndex].branchEntity.branchState.branchColor = color;
        tutorialBranches[branchIndex].branchEntity.branchState.ownerColor = color;
         tutorialBranches[branchIndex].ToggleTrigger();
    }
}
