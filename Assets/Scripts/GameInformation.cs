using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NetworkGameType
{
    Public,
    Host,
    Private
}

public class GameInformation
{
    public static string roomName = "StandardRoom";

    public static NetworkGameType networkGameType = NetworkGameType.Host;

    public static bool playerIsHost = true;

    public static char gameType = 'A';

    public static bool openingSequence = true;

    public static bool openingMoveNodeSet = false;

    public static bool openingMoveBranchSet = false;

    public static int openingNodeId = 0;

    public static int openingBranchId = 0;

    public static int[] playerOneResources = new int[4] { 0, 0, 0, 0 };

    public static int[] playerTwoResources = new int[4] { 0, 0, 0, 0 };

    public static int[] maxTradeResources = new int[4] { 0, 0, 0, 0 };

    public static bool resourceTrade = false;

    public static int playerOneScore = 0;

    public static int playerTwoScore = 0;

    public static int playerOneNetwork = 0;

    public static int playerTwoNetwork = 0;

    public static bool gameOver = false;

    public static string currentPlayer = "HUMAN";

    public static bool humanMoveFinished = false;

    public static bool tradeHasBeenMade = false;

    public static bool HumanNetworkProtocol = false;

    public static List<int> currentRoundPlacedNodes = new List<int>();

    public static List<int> currentRoundPlacedBranches = new List<int>();

    public static bool renderClientBoard = false;

    public static bool newNetworkMoveSet = false;

    public static string ownAvatar = "CAR";

    public static string playerOneAvatar = "CAR";

    public static string playerTwoAvatar = "WHEELBARREL";

    public static bool needToSyncGameVariables = false;

    public static bool needToUpdateOpponentsResources = false;

    public static bool needToSyncAvatars = false;

    public static bool tutorialNeeded = false;
}
