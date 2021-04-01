using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static GameObjectProperties;

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
        "Each player gets 2 game pieces, and 2 roads at the start of the game",
        "Game pieces can be placed on the corners of the streets throughout city",
        "Houses go on the streets themselves, building your real-estate network",
        "Now it's player 2's turn, they get to place their two opening moves consecutively", // 5
        "For the first two rounds for each player, the game piece-road combo can go anywhere in the city...",
        "It's now player 2's turn",
        "Every piece collects a rent from all non-foreclosed properties it touches, this is done at the start of every turn",
        "This game piece collects rent from 1 green, 1 red, and 1 yellow property",
        "And this game piece collects rent from 1 green, 1 yellow, and 1 blue property", // 10
        "Meaning that you've collected rent in the form of 2 green, 2 yellow, 1 blue, and 1 red for this turn",
        "Players use collected rent to build additional game pieces and roads",
        "Tap the street corner of the property to place a new game piece",
        "Now it's the gold player's turn, they recieve their rent from their properties",
        "Houses are legally connected even if they pass through an opponent's game piece", // 15
        "The gold player now submits their move...",
        "The silver player gets rent from all their placed game pieces",
        "Build the highlighted roads and game pieces",
        "The stars on a property represent the max amount of game pieces that can be touching it",
        "This property supports 1 game piece", // 20
        "Foreclosed properties get marked with the foreclosed stamp only after a player submits a move",
        "Select GO to end your turn",
        "Surrounding a property with roads of the same player will monopolize the property for that player",
        "Like foreclosed properties, monopolized properties are marked after a player submits a move",
        "You can even monopolize multiple squares at once if no opponent game pieces or roads inside the attempted area", // 25
        "Once monopolized, opponents cannot build roads through that area",
        "If you don't have the right mix of rent, you can trade for it",
        "Once per turn, you may trade any 3 types of rent for one of another color",
        "Now you have the rent required to build 1 more game piece",
        "Tap the highlighted street corner to build a game piece", // 30
        "The goal of Nodopoly is to be the first to 10 points",
        "1 point is rewarded for each game piece placed and monopolized property. 2 points are rewarded for having the longest chain of roads",
        "While the gold player has 7 roads placed, but not all connected. This means the silver player has the longest network because 6 > 5",
        "Tap the highlighted corner to build a game piece",
        "", // 35
        "Click the Police Man to quit any game, click the Question for in-game rule summary and score breakdown"
    };
    private string[] infoTwoMessages = new string[]
    {
        "Click the green arrow to proceed, click the red arrow to go back",
        "",
        "The first two rounds goes player 1 then player 2 twice, and then player 1 and it alternates",
        "Tap the highlighted street corner to build a game piece",
        "Tap the highlighted street to build a road to connect to your game piece. Press GO to submit your move",
        "", // 5
        "...but after the first two moves, new placements must connect to an already established network",
        "They collect their rent and make their move, they complete the turn to pass it back to you",
        "The color of rent matches the color of the property where it came from",
        " ",
        " ", // 10
        "",
        "A road costs 1 red and 1 blue, while a game piece costs 2 yellow, and 2 green",
        "Press Go to submit your move",
        " ",
        "And you may build additional roads without any game pieces on them", // 15
        "...and any unused rent are saved for future rounds",
        "Including from the new game piece they placed last turn",
        "",
        "If more game pieces are on property than it's star amount, it forecloses and stops paying rent out to either player",
        "But the silver player is about to place a 2nd game piece on it, which will foreclose it", // 20
        "Remember, game pieces from both players count towards the total limit!",
        "",
        "Monopolizing a property builds a hotel on that property and stop paying rent to the opposing player",
        " ",
        " ", // 25
        "The gold player cannot build through the silver perimeter now that these properties are monopolized",
        "Click the chest to open the trading menu",
        "Click the 2 yellow dollars and a red dollar to trade for a needed green dollar",
        " ",
        "", // 30
        "",
        "All 6 of player 1's roads are connected",
        "The player with the longest road network will recieve this card",
        "Finishing this turn will get you a score a 10",
        "", // 35
        "Click the green arrow to exit"
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
    public Button exitBtn;
    public Button[] tradingButtons;

    public NodeController[] tutorialNodes;
    public BranchController[] tutorialBranches;
    public Square[] tutorialTiles;

    public ArrowController[] arrows;

    public GameObject topBG;
    public GameObject bottomBG;
    public GameObject winPanel;
    public GameObject tradePanel;
    public GameObject longestNetworkCard;

    public Sprite tutorialSprite;
    public Sprite tutorialBranchSprite;
    public Sprite highlightSilver;
    public Sprite highlightGold;

    public SpriteRenderer[] blockSquares;
    public Sprite[] undoBlockSprites;

    private float[] arrowXLocations = new float[14];
    private float[] arrowYLocations = new float[14];

    #region Setup
    private void Awake()
    {
        gameController = GameController.getInstance();
        GameInformation.gameType = 'T';
        playerOneAvatar.sprite = avatars[0];
        playerTwoAvatar.sprite = avatars[1];

        infoOne.text = infoOneMessages[messageNumber];
        infoTwo.text = infoTwoMessages[messageNumber];

        goBtn.interactable = false;
        tradeBtn.interactable = false;
        infoBtn.interactable = false;
        exitBtn.interactable = false;
        longestNetworkCard.SetActive(false);

        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].gameObject.SetActive(false);
            arrowXLocations[i] = arrows[i].gameObject.transform.position.x;
            arrowYLocations[i] = arrows[i].gameObject.transform.position.y;
        }
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
        Debug.Log(messageNumber);
        if (messageNumber < infoOneMessages.Length)
        {
            infoOne.text = infoOneMessages[messageNumber];
            infoTwo.text = infoTwoMessages[messageNumber];

            if (messageNumber == 3)
            {
                tutorialPanel.SetActive(false);
                HighlightNode(0);
                arrows[0].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[0]));
            }
            else if (messageNumber == 4)
            {
                StopAllCoroutines();
                arrows[0].gameObject.SetActive(false);
                arrows[2].gameObject.SetActive(true);
                arrows[3].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[2]));
                StartCoroutine(MoveForward(arrows[3]));
                ClaimNode(0, PlayerColor.Silver, avatars[0]);
                GameInformation.openingMoveNodeSet = true;
                HighlightBranch(0);
                goBtn.interactable = true;
                forwardBtn.interactable = false;
            }
            else if (messageNumber == 5)
            {
                StopAllCoroutines();
                arrows[3].gameObject.SetActive(false);
                arrows[2].gameObject.SetActive(false);
                goBtn.interactable = false;
                forwardBtn.interactable = true;

                currentPlayerMessage.text = "Opponent's Move";
                goBtn.interactable = false;
                ClaimBranch(0, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);

                ClaimNode(1, PlayerColor.Gold, avatars[1]);
                ClaimNode(2, PlayerColor.Gold, avatars[1]);
                ClaimBranch(1, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(2, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
            }
            else if (messageNumber == 6)
            {
                currentPlayerMessage.text = "Your Move";
                GameInformation.openingMoveNodeSet = false;
                GameInformation.openingMoveBranchSet = false;
                forwardBtn.interactable = false;
                goBtn.interactable = true;
                HighlightNode(3);
                HighlightBranch(3);
            }
            else if (messageNumber == 7)
            {
                ClaimNode(3, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                ClaimBranch(3, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                GameInformation.openingSequence = false;
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                int[] one = new int[] { 0, 0, 0, 0 };
                int[] two = new int[] { 2, 2, 1, 1 };
                SetResources(one, two);
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
                ;
                int[] one = new int[] { 1, 1, 2, 2 };
                int[] two = new int[] { 1, 1, 1, 1 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: 2";
                playerTwoScore.text = "Score: 2";
            }
            else if (messageNumber == 9)
            {
                arrows[0].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[0]));
            }
            else if(messageNumber == 10)
            {
                StopAllCoroutines();
                arrows[0].gameObject.SetActive(false);
                arrows[1].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[1]));
            }
            else if (messageNumber == 11)
            {
                StopAllCoroutines();
                arrows[1].gameObject.SetActive(false);
            }
            else if (messageNumber == 13)
            {
                forwardBtn.interactable = false;
                goBtn.interactable = true;
                HighlightNode(4);
            }
            else if (messageNumber == 14)
            {
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                ClaimNode(4, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 1, 1, 1, 1 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if (messageNumber == 15)
            {
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ; 
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 3, 3, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
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

                arrows[10].gameObject.SetActive(true);
                StartCoroutine(MoveNorthWest(arrows[10]));
            }
            else if(messageNumber == 16)
            {
                arrows[10].gameObject.SetActive(false);
                StopAllCoroutines();
            }
            else if(messageNumber == 17)
            {
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                int[] one = new int[] { 3, 3, 3, 2 };
                int[] two = new int[] { 0, 0, 2, 2 };
                SetResources(one, two);
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
                int[] one = new int[] { 0, 0, 1, 0 };
                int[] two = new int[] { 0, 0, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                ClaimNode(5, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                ClaimBranch(7, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(8, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(9, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                GameInformation.currentRoundPlacedBranches.Clear();
                GameInformation.currentRoundPlacedNodes.Clear();
            }
            else if(messageNumber == 20)
            {
                arrows[4].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[4]));
            }
            else if(messageNumber == 21)
            {
                StopAllCoroutines();
                arrows[4].gameObject.SetActive(false);
            }
            else if(messageNumber == 22)
            {
                goBtn.interactable = true;
                forwardBtn.interactable = false;
            }
            else if(messageNumber == 23)
            {
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                tutorialTiles[0].squareState.resourceState = SquareStatus.Blocked;
                gameController.UpdateGameBoard();
                boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ; 
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 2, 2, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 24)
            {
                arrows[5].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[5]));
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
                StopAllCoroutines();
                arrows[5].gameObject.SetActive(false);
                arrows[6].gameObject.SetActive(true);
                StartCoroutine(MoveUp(arrows[6]));
                tutorialTiles[1].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[1].squareState.ownerColor = PlayerColor.Gold;
                gameController.UpdateGameBoard();
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
                goBtn.interactable = true;
                forwardBtn.interactable = false;
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                int[] one = new int[] { 4, 3, 3, 1 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

                HighlightBranch(12);
            }
            else if (messageNumber == 26)
            {
                int[] one = new int[] { 3, 2, 3, 1 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                StopAllCoroutines();
                arrows[6].gameObject.SetActive(false);
                arrows[11].gameObject.SetActive(true);
                StartCoroutine(MoveNorthEast(arrows[11]));
                goBtn.interactable = false;
                forwardBtn.interactable = true;
                ClaimBranch(12, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                tutorialTiles[0].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[0].squareState.ownerColor = PlayerColor.Silver;
                tutorialTiles[2].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[2].squareState.ownerColor = PlayerColor.Silver;
                gameController.UpdateGameBoard();
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
            }
            else if (messageNumber == 27)
            {
                StopAllCoroutines();
                arrows[11].gameObject.SetActive(false);
                tradeBtn.interactable = true;
                forwardBtn.interactable = false;
                arrows[7].gameObject.SetActive(true);
                StartCoroutine(MoveDown(arrows[7]));
            }
            else if (messageNumber == 28)
            {
                StopAllCoroutines();
                arrows[7].gameObject.SetActive(false);
                for (int i = 0; i < tradingButtons.Length; i++)
                    tradingButtons[i].interactable = false;
            }
            else if(messageNumber == 29)
            {
                int[] one = new int[] { 1, 2, 2, 2 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                tradeBtn.interactable = false;
                forwardBtn.interactable = true;
            }
            else if (messageNumber == 30)
            {
                HighlightNode(6);
                forwardBtn.interactable = false;
                goBtn.interactable = true;
            }
            else if (messageNumber == 31)
            {
                ClaimNode(6, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                arrows[8].gameObject.SetActive(true);
                arrows[9].gameObject.SetActive(true);

                int[] one = new int[] { 1, 2, 0, 0 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();

                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 32)
            {
                arrows[8].gameObject.SetActive(false);
                arrows[9].gameObject.SetActive(false);
                ClaimBranch(0, PlayerColor.Silver, highlightSilver);
                ClaimBranch(3, PlayerColor.Silver, highlightSilver);
                ClaimBranch(7, PlayerColor.Silver, highlightSilver);
                ClaimBranch(8, PlayerColor.Silver, highlightSilver);
                ClaimBranch(9, PlayerColor.Silver, highlightSilver);
                ClaimBranch(12, PlayerColor.Silver, highlightSilver);
            }
            else if (messageNumber == 33)
            {
                ClaimBranch(0, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(3, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(7, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(8, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(9, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(12, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(1, PlayerColor.Gold, highlightGold);
                ClaimBranch(2, PlayerColor.Gold, highlightGold);
                ClaimBranch(4, PlayerColor.Gold, highlightGold);
                ClaimBranch(5, PlayerColor.Gold, highlightGold);
                ClaimBranch(6, PlayerColor.Gold, highlightGold);
                ClaimBranch(10, PlayerColor.Gold, highlightGold);
                ClaimBranch(11, PlayerColor.Gold, highlightGold);
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                longestNetworkCard.SetActive(true);
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 34)
            {

                longestNetworkCard.SetActive(false);
                ClaimBranch(1, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(2, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(4, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(5, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(6, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(10, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(11, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                forwardBtn.interactable = false;
                goBtn.interactable = true;
                HighlightNode(7);
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;

                int[] one = new int[] { 4, 4, 3, 4 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if(messageNumber == 35)
            {
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                topBG.SetActive(false);
                bottomBG.SetActive(false);

                ClaimNode(7, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                int[] one = new int[] { 4, 4, 1, 2 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();

                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
                GameInformation.gameOver = true;
                winPanel.SetActive(true);
            }
            else if (messageNumber == 36)
            {
                winPanel.SetActive(false);
                topBG.SetActive(true);
                bottomBG.SetActive(true);
                GameInformation.gameOver = false;

                arrows[12].gameObject.SetActive(true);
                arrows[13].gameObject.SetActive(true);

                StartCoroutine(MoveNorthWest(arrows[12]));
                StartCoroutine(MoveNorthEast(arrows[13]));
            }
        }
        else
        {
            GameInformation.tutorialNeeded = true;
            sl.LoadMenuScene();
        }
    }

    public void OnBackwardClick()
    {
        messageNumber--;
        Debug.Log(messageNumber);
        if (messageNumber >= 0)
        {
            infoOne.text = infoOneMessages[messageNumber];
            infoTwo.text = infoTwoMessages[messageNumber];

            if (messageNumber == 2)
            {
                StopAllCoroutines();
                tutorialPanel.SetActive(true);
                arrows[0].gameObject.SetActive(false);
                MoveArrow(0);
                UndoNode(0, PlayerColor.Blank, tutorialNodes[0].blankSprite);
            }
            else if (messageNumber == 3)
            {
                StopAllCoroutines();
                MoveArrow(0);
                arrows[0].gameObject.SetActive(true);
                arrows[2].gameObject.SetActive(false);
                arrows[3].gameObject.SetActive(false);
                MoveArrow(2);
                MoveArrow(3);
                UndoNode(0, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                GameInformation.openingMoveNodeSet = false;
                UndoBranch(0, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                goBtn.interactable = false;
                forwardBtn.interactable = true;
                tutorialPanel.SetActive(false);
                HighlightNode(0);
                arrows[0].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[0]));
            }
            else if (messageNumber == 4)
            {
                StopAllCoroutines();
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                UndoBranch(0, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                GameInformation.openingMoveNodeSet = true;
                GameInformation.openingMoveBranchSet = false;
                GameInformation.openingNodeId = 8;
                UndoNode(1, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                UndoNode(2, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                UndoBranch(1, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                UndoBranch(2, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                MoveArrow(0);
                arrows[0].gameObject.SetActive(false);
                arrows[2].gameObject.SetActive(true);
                arrows[3].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[2]));
                StartCoroutine(MoveForward(arrows[3]));
                ClaimNode(0, PlayerColor.Silver, avatars[0]);
                GameInformation.openingMoveNodeSet = true;
                HighlightBranch(0);
                goBtn.interactable = true;
                forwardBtn.interactable = false;
            }
            else if (messageNumber == 5)
            {
                StopAllCoroutines();

                // undo 6
                GameInformation.openingMoveNodeSet = false;
                GameInformation.openingMoveBranchSet = false;
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                UndoNode(3, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                UndoBranch(3, PlayerColor.Blank, tutorialBranches[0].blankSprite);

                // redo 5
                MoveArrow(2);
                MoveArrow(3);
                arrows[3].gameObject.SetActive(false);
                arrows[2].gameObject.SetActive(false);
                forwardBtn.interactable = true;
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                ClaimBranch(0, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimNode(1, PlayerColor.Gold, avatars[1]);
                ClaimNode(2, PlayerColor.Gold, avatars[1]);
                ClaimBranch(1, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(2, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
            }
            else if (messageNumber == 6)
            {
                StopAllCoroutines();
                // undo 7
                UndoNode(3, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                UndoBranch(3, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                GameInformation.openingSequence = true;
                int[] one = new int[] { 0, 0, 0, 0 };
                int[] two = new int[] { 0, 0, 0, 0 };
                SetResources(one, two);
                playerOneScore.text = "Score: 0";
                playerTwoScore.text = "Score: 0";
                UndoBranch(4, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                playerResourcesManager.UpdateBothPlayersResources();

                // redo 6
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                GameInformation.openingMoveNodeSet = false;
                GameInformation.openingMoveBranchSet = false;
                forwardBtn.interactable = false;
                goBtn.interactable = true;
                HighlightNode(3);
                HighlightBranch(3);
            }
            else if (messageNumber == 7)
            {
                // Redo 7
                ClaimNode(3, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                ClaimBranch(3, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                GameInformation.openingSequence = false;
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                int[] one = new int[] { 0, 0, 0, 0 };
                int[] two = new int[] { 2, 2, 1, 1 };
                SetResources(one, two);
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
                // undo 9
                StopAllCoroutines();
                MoveArrow(0);
                arrows[0].gameObject.SetActive(false);

                // redo 8
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                int[] one = new int[] { 1, 1, 2, 2 };
                int[] two = new int[] { 1, 1, 1, 1 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: 2";
                playerTwoScore.text = "Score: 2";
            }
            else if (messageNumber == 9)
            {
                // undo 10
                StopAllCoroutines();
                MoveArrow(1);
                arrows[1].gameObject.SetActive(false);

                // redo 9
                arrows[0].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[0]));
            }
            else if (messageNumber == 10)
            {
                StopAllCoroutines();
                MoveArrow(0);
                arrows[0].gameObject.SetActive(false);
                arrows[1].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[1]));
            }
            else if (messageNumber == 11)
            {
                StopAllCoroutines();
                MoveArrow(1);
                arrows[1].gameObject.SetActive(false);
            }
            else if (messageNumber == 12)
            {
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                UndoNode(4, PlayerColor.Blank, tutorialNodes[0].blankSprite);
            }
            else if (messageNumber == 13)
            {
                UndoNode(4, PlayerColor.Blank, tutorialSprite);
                int[] one = new int[] { 1, 1, 2, 2 };
                int[] two = new int[] { 1, 1, 1, 1 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();

                forwardBtn.interactable = false;
                goBtn.interactable = true;
                HighlightNode(4);
            }
            else if (messageNumber == 14)
            {
                // undo 15
                StopAllCoroutines();
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                UndoBranch(5, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                UndoBranch(6, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                ;
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
                MoveArrow(10);
                arrows[10].gameObject.SetActive(false);

                // redo 14
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                ClaimNode(4, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 1, 1, 1, 1 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if (messageNumber == 15)
            {
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 3, 3, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
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

                arrows[10].gameObject.SetActive(true);
                StartCoroutine(MoveNorthWest(arrows[10]));
            }
            else if (messageNumber == 16)
            {
                // undo 17
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                int[] one = new int[] { 3, 3, 3, 2 };
                int[] two = new int[] { 0, 0, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
            }
            else if (messageNumber == 17)
            {
                UndoNode(5, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                UndoBranch(7, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                UndoBranch(8, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                UndoBranch(9, PlayerColor.Blank, tutorialBranches[0].blankSprite);

                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                int[] one = new int[] { 3, 3, 3, 2 };
                int[] two = new int[] { 0, 0, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 18)
            {
                int[] one = new int[] { 3, 3, 3, 2 };
                int[] two = new int[] { 0, 0, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                UndoNode(5, PlayerColor.Blank, tutorialNodes[0].playerOneSprite);
                UndoBranch(7, PlayerColor.Blank, tutorialBranches[0].playerOneSprite);
                UndoBranch(8, PlayerColor.Blank, tutorialBranches[0].playerOneSprite);
                UndoBranch(9, PlayerColor.Blank, tutorialBranches[0].playerOneSprite);
                GameInformation.currentRoundPlacedBranches.Clear();
                GameInformation.currentRoundPlacedNodes.Clear();

                HighlightNode(5);
                HighlightBranch(7);
                HighlightBranch(8);
                HighlightBranch(9);
            }
            else if (messageNumber == 19)
            {
                StopAllCoroutines();
                MoveArrow(4);
                arrows[4].gameObject.SetActive(false);

                int[] one = new int[] { 0, 0, 1, 0 };
                int[] two = new int[] { 0, 0, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                ClaimNode(5, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                ClaimBranch(7, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(8, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                ClaimBranch(9, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                GameInformation.currentRoundPlacedBranches.Clear();
                GameInformation.currentRoundPlacedNodes.Clear();
            }
            else if (messageNumber == 20)
            {
                StopAllCoroutines();
                arrows[4].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[4]));
            }
            else if (messageNumber == 21)
            {
                StopAllCoroutines();
                goBtn.interactable = false;
                forwardBtn.interactable = true;
                MoveArrow(4);
                arrows[4].gameObject.SetActive(false);
            }
            else if (messageNumber == 22)
            {
                tutorialTiles[0].squareState.resourceState = SquareStatus.Open;
                blockSquares[0].sprite = undoBlockSprites[0];
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 0, 0, 2, 2 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
                goBtn.interactable = true;
                forwardBtn.interactable = false;
            }
            else if (messageNumber == 23)
            {
                StopAllCoroutines();
                MoveArrow(5);
                arrows[5].gameObject.SetActive(false);
                UndoBranch(10, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                UndoBranch(11, PlayerColor.Blank, tutorialBranches[0].blankSprite);

                forwardBtn.interactable = true;
                goBtn.interactable = false;
                tutorialTiles[0].squareState.resourceState = SquareStatus.Blocked;
                gameController.UpdateGameBoard();
                boardManager.DetectNewTileBlocks(gameController.getGameBoard().squares);
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 2, 2, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 24)
            {
                StopAllCoroutines();
                UndoBranch(12, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                MoveArrow(6);
                arrows[6].gameObject.SetActive(false);
                tutorialTiles[1].squareState.resourceState = SquareStatus.Open;
                tutorialTiles[1].squareState.ownerColor = PlayerColor.Blank;
                blockSquares[1].sprite = undoBlockSprites[1];
                goBtn.interactable = false;
                forwardBtn.interactable = true;
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                int[] one = new int[] { 1, 1, 0, 0 };
                int[] two = new int[] { 2, 2, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

                arrows[5].gameObject.SetActive(true);
                StartCoroutine(MoveForward(arrows[5]));
                ClaimBranch(10, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(11, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;
                GameInformation.playerTwoResources[1]--;
                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if(messageNumber == 25)
            {
                StopAllCoroutines();
                MoveArrow(11);
                arrows[11].gameObject.SetActive(false);
                UndoBranch(12, PlayerColor.Blank, tutorialBranches[0].blankSprite);
                tutorialTiles[0].squareState.resourceState = SquareStatus.Open;
                tutorialTiles[0].squareState.ownerColor = PlayerColor.Blank;
                tutorialTiles[3].squareState.resourceState = SquareStatus.Open;
                tutorialTiles[3].squareState.ownerColor = PlayerColor.Blank;
                blockSquares[0].sprite = undoBlockSprites[3];
                blockSquares[2].sprite = undoBlockSprites[2];

                MoveArrow(5);
                arrows[5].gameObject.SetActive(false);
                arrows[6].gameObject.SetActive(true);
                StartCoroutine(MoveUp(arrows[6]));
                tutorialTiles[1].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[1].squareState.ownerColor = PlayerColor.Gold;
                gameController.UpdateGameBoard();
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
                goBtn.interactable = true;
                forwardBtn.interactable = false;
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                int[] one = new int[] { 4, 3, 3, 1 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
                HighlightBranch(12);
            }
            else if (messageNumber == 26)
            {
                StopAllCoroutines();
                MoveArrow(7);
                arrows[7].gameObject.SetActive(false);
                int[] one = new int[] { 3, 2, 3, 1 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                MoveArrow(6);
                arrows[6].gameObject.SetActive(false);
                arrows[11].gameObject.SetActive(true);
                StartCoroutine(MoveNorthEast(arrows[11]));
                goBtn.interactable = false;
                forwardBtn.interactable = true;
                ClaimBranch(12, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                tutorialTiles[0].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[0].squareState.ownerColor = PlayerColor.Silver;
                tutorialTiles[2].squareState.resourceState = SquareStatus.Captured;
                tutorialTiles[2].squareState.ownerColor = PlayerColor.Silver;
                gameController.UpdateGameBoard();
                boardManager.DetectNewBlockCaptures(gameController.getGameBoard().squares);
            }
            else if (messageNumber == 27)
            {
                StopAllCoroutines();
                tradePanel.SetActive(false);
                MoveArrow(11);
                arrows[11].gameObject.SetActive(false);
                tradeBtn.interactable = true;
                forwardBtn.interactable = false;
                arrows[7].gameObject.SetActive(true);
                StartCoroutine(MoveDown(arrows[7]));
            }
            else if (messageNumber == 28)
            {
                int[] one = new int[] { 3, 2, 3, 1 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                tradeBtn.interactable = false;
                forwardBtn.interactable = false;
                GameInformation.resourceTrade = false;
                tradePanel.SetActive(true);
                StopAllCoroutines();
                MoveArrow(7);
                arrows[7].gameObject.SetActive(false);
                for (int i = 0; i < tradingButtons.Length; i++)
                    tradingButtons[i].interactable = false;
            }
            else if (messageNumber == 29)
            {
                int[] one = new int[] { 1, 2, 2, 2 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                UndoNode(6, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                forwardBtn.interactable = true;
                goBtn.interactable = false;
            }
            else if(messageNumber == 30)
            {
                UndoNode(6, PlayerColor.Blank, tutorialSprite);
                MoveArrow(8);
                MoveArrow(9);
                arrows[8].gameObject.SetActive(false);
                arrows[9].gameObject.SetActive(false);
                int[] one = new int[] { 1, 2, 2, 2 };
                int[] two = new int[] { 0, 0, 3, 3 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

                HighlightNode(6);
                forwardBtn.interactable = false;
                goBtn.interactable = true;
            }
            else if(messageNumber == 31)
            {
                UndoBranch(0, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                UndoBranch(3, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                UndoBranch(7, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                UndoBranch(8, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                UndoBranch(9, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);
                UndoBranch(12, PlayerColor.Silver, tutorialBranches[0].playerOneSprite);

                ClaimNode(6, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                arrows[8].gameObject.SetActive(true);
                arrows[9].gameObject.SetActive(true);
                int[] one = new int[] { 1, 2, 0, 0 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 32)
            {
                ClaimBranch(1, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(2, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(4, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(5, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(6, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(10, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(11, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                longestNetworkCard.SetActive(false);
                int[] one = new int[] { 1, 2, 0, 0 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();

                MoveArrow(8);
                MoveArrow(9);
                arrows[8].gameObject.SetActive(false);
                arrows[9].gameObject.SetActive(false);
                ClaimBranch(0, PlayerColor.Silver, highlightSilver);
                ClaimBranch(3, PlayerColor.Silver, highlightSilver);
                ClaimBranch(7, PlayerColor.Silver, highlightSilver);
                ClaimBranch(8, PlayerColor.Silver, highlightSilver);
                ClaimBranch(9, PlayerColor.Silver, highlightSilver);
                ClaimBranch(12, PlayerColor.Silver, highlightSilver);
            }
            else if (messageNumber == 33)
            {
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                UndoNode(7, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                longestNetworkCard.SetActive(true);
                int[] one = new int[] { 4, 4, 3, 4 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();

                ClaimBranch(1, PlayerColor.Gold, highlightGold);
                ClaimBranch(2, PlayerColor.Gold, highlightGold);
                ClaimBranch(4, PlayerColor.Gold, highlightGold);
                ClaimBranch(5, PlayerColor.Gold, highlightGold);
                ClaimBranch(6, PlayerColor.Gold, highlightGold);
                ClaimBranch(10, PlayerColor.Gold, highlightGold);
                ClaimBranch(11, PlayerColor.Gold, highlightGold);
                GameInformation.currentPlayer = "AI";
                currentPlayerMessage.text = "Opponent's Move";
                ;
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 34)
            {
                topBG.SetActive(true);
                bottomBG.SetActive(true);
                UndoNode(7, PlayerColor.Blank, tutorialNodes[0].blankSprite);
                GameInformation.gameOver = false;
                winPanel.SetActive(false);

                longestNetworkCard.SetActive(false);
                ClaimBranch(1, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(2, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(4, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(5, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(6, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(10, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                ClaimBranch(11, PlayerColor.Gold, tutorialBranches[0].playerTwoSprite);
                forwardBtn.interactable = false;
                goBtn.interactable = true;
                HighlightNode(7);
                GameInformation.currentPlayer = "HUMAN";
                currentPlayerMessage.text = "Your Move";
                ;
                int[] one = new int[] { 4, 4, 3, 4 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
            }
            else if (messageNumber == 35)
            {
                StopAllCoroutines();
                winPanel.SetActive(true);
                topBG.SetActive(false);
                bottomBG.SetActive(false);
                MoveArrow(12);
                MoveArrow(13);
                arrows[12].gameObject.SetActive(false);
                arrows[13].gameObject.SetActive(false);
                forwardBtn.interactable = true;
                goBtn.interactable = false;
                ClaimNode(7, PlayerColor.Silver, tutorialNodes[0].playerOneSprite);
                int[] one = new int[] { 4, 4, 1, 2 };
                int[] two = new int[] { 2, 2, 4, 4 };
                SetResources(one, two);
                playerResourcesManager.UpdateBothPlayersResources();
                gameController.UpdateScores();
                playerOneScore.text = "Score: " + GameInformation.playerOneScore.ToString();
                playerTwoScore.text = "Score: " + GameInformation.playerTwoScore.ToString();
                GameInformation.gameOver = true;
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
        BoxCollider2D collider = tutorialBranches[branchIndex].GetComponent<BoxCollider2D>();
        collider.enabled = true;
    }

    public void HighlightNode(int nodeIndex)
    {
        SpriteRenderer sprite = tutorialNodes[nodeIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = tutorialSprite;
        BoxCollider2D collider = tutorialNodes[nodeIndex].GetComponent<BoxCollider2D>();
        collider.enabled = true;
    }

    public void ClaimBranch(int branchIndex, PlayerColor playerColor, Sprite playerImage)
    {
        SpriteRenderer sprite = tutorialBranches[branchIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = playerImage;
        tutorialBranches[branchIndex].branchEntity.branchState.branchColor = playerColor;
        tutorialBranches[branchIndex].branchEntity.branchState.ownerColor = playerColor;
        BoxCollider2D collider = tutorialBranches[branchIndex].GetComponent<BoxCollider2D>();
        collider.enabled = false;
    }

    public void ClaimNode(int nodeIndex, PlayerColor playerColor, Sprite playerAvatar)
    {
        SpriteRenderer sprite = tutorialNodes[nodeIndex].GetComponent<SpriteRenderer>();
        sprite.sprite = playerAvatar;
        tutorialNodes[nodeIndex].nodeEntity.nodeState.nodeColor = playerColor;
        BoxCollider2D collider = tutorialNodes[nodeIndex].GetComponent<BoxCollider2D>();
        collider.enabled = false;
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

    IEnumerator MoveForward(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x + .1f, arrow.gameObject.transform.position.y);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveBackward(arrow));
    }

    IEnumerator MoveBackward(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x - .1f, arrow.gameObject.transform.position.y);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveForward(arrow));
    }

    IEnumerator MoveUp(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x, arrow.gameObject.transform.position.y + .1f);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveDown(arrow));
    }

    IEnumerator MoveDown(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x, arrow.gameObject.transform.position.y- .1f);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveUp(arrow));
    }

    IEnumerator MoveNorthWest(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x - .1f, arrow.gameObject.transform.position.y + .1f);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveSouthEast(arrow));
    }

    IEnumerator MoveSouthEast(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x + .1f, arrow.gameObject.transform.position.y - .1f);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveNorthWest(arrow));
    }

    IEnumerator MoveNorthEast(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x + .1f, arrow.gameObject.transform.position.y + .1f);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveSouthWest(arrow));
    }

    IEnumerator MoveSouthWest(ArrowController arrow)
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            arrow.gameObject.transform.position = new Vector3(arrow.gameObject.transform.position.x - .1f, arrow.gameObject.transform.position.y - .1f);
            yield return new WaitForSeconds(.1f);
        }

        StartCoroutine(MoveNorthEast(arrow));
    }

    public void SetResources(int[] one, int[] two)
    {
        GameInformation.playerOneResources[0] = one[0];
        GameInformation.playerOneResources[1] = one[1];
        GameInformation.playerOneResources[2] = one[2];
        GameInformation.playerOneResources[3] = one[3];
        GameInformation.playerTwoResources[0] = two[0];
        GameInformation.playerTwoResources[1] = two[1];
        GameInformation.playerTwoResources[2] = two[2];
        GameInformation.playerTwoResources[3] = two[3];
    }

    public void MoveArrow(int arrowNum)
    {
        arrows[arrowNum].gameObject.transform.position = new Vector3(arrowXLocations[arrowNum], arrowYLocations[arrowNum]);
    }
}
