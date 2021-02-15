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

    public static bool playerIsHost = false;

    public static char gameType = 'L';

    public static bool openingSequence = true;

    public static int turnNumber = 1;
}
