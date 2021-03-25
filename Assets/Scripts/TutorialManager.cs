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
        "Nodopoly is a 2 player strategy game, pitting players against one another for real-estate dominance of their city",
        "The cityscape is randomly generated at the start of each game",
        "Each player gets 2 of their avatar game pieces, and 2 houses at the start of the game",
        "Game pieces can be placed on the corners of the streets throughout city",
        "Houses go on the streets themselves, building your real-estate network",
        "Now it's player 2's turn, they get to place two in a round",
        "For the first two rounds for each player, the game piece-house combo can go anywhere in the city...",
        "It's now player 2's turn",
        "Every piece collects a rent from all non-foreclosed properties it touches, this is done at the start of every turn",
        "This game piece collects rent from 1 green, 1 red, and 1 yellow property",
        "And this game piece collects rent from 1 green, 1 yellow, and 1 blue property",
        "",
        "Players use collected rent to build additional game pieces and houses",
        "Tap the street corner of the property to place a new game piece",
        "Now it's the gold player's turn, they recieve their rent from their properties",
        "Houses are legally connected even if they pass through an opponent's game piece",
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
        "The first two rounds goes player 1 then player 2 twice, and then player 1 and it alternates",
        "Place the game piece on the street corner",
        "Place the house on the street to connect to your game piece",
        "",
        "...but after the first two moves, all game pieces and houses must connect to your already established network",
        "They collect their rent and make their move, they complete the turn to pass it back to you",
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

    public NodeController[] tutorialNodes;
    public BranchController[] tutorialBranches;

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


    public void BeginTutorial()
    {

        GameInformation.HumanNetworkProtocol = true;
        gameController.SetBoardConfiguration("1R1G3R2B2G3Y0L3G1Y3B2R2Y1B");
        boardManager.SetSquareUI(gameController.getGameBoard().GetSquareStates());
        currentPlayerMessage.text = "Your Move";
    }

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

    public void OnForwardClick()
    {
        messageNumber++;
        if (messageNumber <= 29)
        {
            infoOne.text = infoOneMessages[messageNumber];
            infoTwo.text = infoTwoMessages[messageNumber];

            if (messageNumber == 3)
            {
                tutorialPanel.SetActive(false);
                SpriteRenderer sprite = tutorialNodes[0].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialSprite;
                tutorialNodes[0].ToggleTrigger();
            }
            else if(messageNumber == 4)
            {
                SpriteRenderer sprite = tutorialNodes[0].GetComponent<SpriteRenderer>();
                sprite.sprite = avatars[0];
                tutorialNodes[0].nodeEntity.nodeState.nodeColor = PlayerColor.Silver;

                sprite = tutorialBranches[0].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranchSprite;

                tutorialNodes[0].ToggleTrigger();
                tutorialBranches[0].ToggleTrigger();
            }
            else if(messageNumber == 5)
            {
                SpriteRenderer sprite = tutorialBranches[0].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranches[0].playerOneSprite;
                tutorialBranches[0].branchEntity.branchState.branchColor = PlayerColor.Silver;
                tutorialBranches[0].branchEntity.branchState.ownerColor = PlayerColor.Silver;

                
                sprite = tutorialNodes[1].GetComponent<SpriteRenderer>();
                sprite.sprite = avatars[1];
                tutorialNodes[1].nodeEntity.nodeState.nodeColor = PlayerColor.Gold;

                sprite = tutorialBranches[1].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranches[1].playerTwoSprite;
                tutorialBranches[1].branchEntity.branchState.branchColor = PlayerColor.Gold;
                tutorialBranches[1].branchEntity.branchState.ownerColor = PlayerColor.Gold;

                sprite = tutorialNodes[2].GetComponent<SpriteRenderer>();
                sprite.sprite = avatars[1];
                tutorialNodes[2].nodeEntity.nodeState.nodeColor = PlayerColor.Gold;

                sprite = tutorialBranches[2].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranches[2].playerTwoSprite;
                tutorialBranches[2].branchEntity.branchState.branchColor = PlayerColor.Gold;
                tutorialBranches[2].branchEntity.branchState.ownerColor = PlayerColor.Gold;
            }
            else if(messageNumber == 6)
            {
                GameInformation.openingMoveNodeSet = false;
                GameInformation.openingMoveBranchSet = false;

                SpriteRenderer sprite = tutorialNodes[3].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialSprite;

                sprite = tutorialBranches[3].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranchSprite;

                tutorialNodes[3].ToggleTrigger();
                tutorialBranches[3].ToggleTrigger();
            }
            else if (messageNumber == 7)
            {
                GameInformation.openingSequence = false;
                GameInformation.currentPlayer = "AI";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;

                SpriteRenderer sprite = tutorialBranches[4].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranches[4].playerTwoSprite;
                tutorialBranches[4].branchEntity.branchState.branchColor = PlayerColor.Gold;
                tutorialBranches[4].branchEntity.branchState.ownerColor = PlayerColor.Gold;

                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if (messageNumber == 8)
            {
                GameInformation.currentPlayer = "HUMAN";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if (messageNumber == 13)
            {
                SpriteRenderer sprite = tutorialNodes[4].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialSprite;
                tutorialNodes[4].ToggleTrigger();
            }
            else if (messageNumber == 15)
            {
                GameInformation.currentPlayer = "AI";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;

                SpriteRenderer sprite = tutorialBranches[5].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranches[4].playerTwoSprite;
                tutorialBranches[5].branchEntity.branchState.branchColor = PlayerColor.Gold;
                tutorialBranches[5].branchEntity.branchState.ownerColor = PlayerColor.Gold;
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;

                sprite = tutorialBranches[6].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranches[4].playerTwoSprite;
                tutorialBranches[6].branchEntity.branchState.branchColor = PlayerColor.Gold;
                tutorialBranches[6].branchEntity.branchState.ownerColor = PlayerColor.Gold;
                GameInformation.playerTwoResources[0]--;
                GameInformation.playerTwoResources[1]--;

                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if(messageNumber == 17)
            {
                GameInformation.currentPlayer = "HUMAN";
                gameController.FlipColors();
                gameController.CollectCurrentPlayerResources();
                playerResourcesManager.UpdateBothPlayersResources();
            }
            else if (messageNumber == 18)
            {
                SpriteRenderer sprite = tutorialNodes[5].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialSprite;
                tutorialNodes[5].ToggleTrigger();

                sprite = tutorialBranches[7].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranchSprite;
                tutorialBranches[7].ToggleTrigger();
                sprite = tutorialBranches[8].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialBranchSprite;
                tutorialBranches[8].ToggleTrigger();
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
                SpriteRenderer sprite = tutorialNodes[0].GetComponent<SpriteRenderer>();
                sprite.sprite = tutorialNodes[0].blankSprite;
                tutorialNodes[0].ToggleTrigger();
            }
        }
        else
        {
            sl.LoadMenuScene();
        }
    }
}
