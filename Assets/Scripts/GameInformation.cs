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
    public static string username;

    public static string roomName = "StandardRoom";

    public static NetworkGameType networkGameType = NetworkGameType.Public;

    public static bool playerIsHost = true;

    public static char gameType = 'A';

    public static bool openingSequence = true;

    public static int turnNumber = 1;

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

    public static bool gameOver = false;

    public static string currentPlayer = "HUMAN";

    public static bool humanMoveFinished = false;

    public static bool tradeHasBeenMade = false;

    public static bool HumanNetworkProtocol = false;

    public static List<int> currentRoundPlacedNodes = new List<int>();

    public static List<int> currentRoundPlacedBranches = new List<int>();

    public static bool renderClientBoard = false;

    public static bool enableTriggers = false;

    public static bool newNetworkMoveSet = false;
}
