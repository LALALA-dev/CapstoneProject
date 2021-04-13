using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class Reference
{
    public static readonly SquareState[] defaultSquareState = new[]
    {
       new SquareState(){location=0, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.One, resourceColor=SquareResourceColor.Red, resourceState=SquareStatus.Open},
       new SquareState(){location=1, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Two, resourceColor=SquareResourceColor.Red, resourceState=SquareStatus.Open},
       new SquareState(){location=2, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Three, resourceColor=SquareResourceColor.Red, resourceState=SquareStatus.Open},

       new SquareState(){location=3, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.One, resourceColor=SquareResourceColor.Blue, resourceState=SquareStatus.Open},
       new SquareState(){location=4, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Two, resourceColor=SquareResourceColor.Blue, resourceState=SquareStatus.Open},
       new SquareState(){location=5, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Three, resourceColor=SquareResourceColor.Blue, resourceState=SquareStatus.Open},

       new SquareState(){location=6, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.One, resourceColor=SquareResourceColor.Yellow, resourceState=SquareStatus.Open},
       new SquareState(){location=7, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Two, resourceColor=SquareResourceColor.Yellow, resourceState=SquareStatus.Open},
       new SquareState(){location=8, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Three, resourceColor=SquareResourceColor.Yellow, resourceState=SquareStatus.Open},

       new SquareState(){location=9, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.One, resourceColor=SquareResourceColor.Green, resourceState=SquareStatus.Open},
       new SquareState(){location=10, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Two, resourceColor=SquareResourceColor.Green, resourceState=SquareStatus.Open},
       new SquareState(){location=11, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.Three, resourceColor=SquareResourceColor.Green, resourceState=SquareStatus.Open},

       new SquareState(){location=12, ownerColor=PlayerColor.Blank, resourceAmount=SquareResourceAmount.One, resourceColor=SquareResourceColor.Blank, resourceState=SquareStatus.Open},
    };

    // Connections are ordered by position: { up, left, right, down }
    public static readonly int[,] branchesOnSquareConnections = new int[,] {
                                                 { 0, 1, 2, 4 },
                             { 3, 6, 7, 11 },    { 4, 7, 8, 12 },    { 5, 8, 9, 13 },
        { 10, 15, 16, 21 }, { 11, 16, 17, 22 }, { 12, 17, 18, 23 }, { 13, 18, 19, 24 }, { 14, 19, 20, 25 },
                            { 22, 26, 27, 30 }, { 23, 27, 28, 31 }, { 24, 28, 29, 32 },
                                                { 31, 33, 34, 35 }
    };

    // Connections are ordered by position, as above. -1 means no object.
    public static readonly int[,] squareOnSquareConnections = new int[,] {
                                                { -1, -1, -1, 2 },
                            { -1, -1, 2, 5 },    { 0, 1, 3, 6 },    { -1, 2, -1, 7 },
        { -1, -1, 5, -1 },    { 1, 4, 6, 9 },     { 2, 5, 7, 10 },    { 3, 6, 8, 11 }, { -1, 7, -1, -1 },
                            { 5, -1, 10, -1 },  { 6, 9, 11, 12 },   { 7, 10, -1, -1 },
                                                { 10, -1, -1, -1 }
    };
}
