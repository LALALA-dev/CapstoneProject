using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectProperties
{
    public enum PlayerColor
    {
        Blank,
        Silver,
        Gold
    }
    public enum SquareStatus
    {
        Blocked,
        Open,
        Captured
    }
    public enum SquareResourceColor
    {
        Blank,
        Red,
        Yellow,
        Blue,
        Green
    }
    public enum SquareResourceAmount
    {
        Blank,
        One,
        Two,
        Three
    }

    public struct SquareCaptureDetect
    {
        public int square;
        public List<int> blankBranches;
    }

    public struct SquareState
    {
        public int location;
        public PlayerColor ownerColor;
        public SquareStatus resourceState;
        public SquareResourceColor resourceColor;
        public SquareResourceAmount resourceAmount;
    }

    public struct NodeState
    {
        public int location;
        public PlayerColor nodeColor;
    }

    public struct BoardState
    {
        public SquareState[] squareStates;
        public NodeState[] nodeStates;
        public BranchState[] branchStates;
    }

    public struct BranchState
    {
        public int location;
        public PlayerColor branchColor;
        public PlayerColor ownerColor;
    }
}
