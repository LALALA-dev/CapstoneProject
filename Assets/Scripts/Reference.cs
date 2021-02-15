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
}
