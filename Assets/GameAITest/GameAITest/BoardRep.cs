using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameAITest
{
    public enum PlayerColor
    {
        Blank,
        Orange,
        Purple
    }
    public struct BranchState
    {
        public int location;
        public PlayerColor branchColor; // The color of the branch.
        public PlayerColor ownerColor;  // The color of the branch's owner (if branch in capture area, can be owned while actual color is blank).
    }
    public struct NodeState
    {
        public int location;
        public PlayerColor nodeColor;
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
    public struct SquareState
    {
        public int location;
        public PlayerColor ownerColor;
        public SquareStatus resourceState;
        public SquareResourceColor resourceColor;
        public SquareResourceAmount resourceAmount;
    }

    public struct BoardState
    {
        public SquareState[] squareStates;
        public NodeState[] nodeStates;
        public BranchState[] branchStates;
        
    }

    public static class ReferenceScript // should it be static?
    {
        public static int[][] branchConnectsToTheseBranches = new int[][]
        {
        new int[2] {1, 2},
        new int[4] {0, 3, 4, 7},
        new int[4] {0, 4, 5, 8},
        new int[4] {1, 4, 6, 7},
        new int[6] {1, 2, 3, 5, 6, 7},
        new int[4] {2, 4, 8, 9},

        };

        public static int[][] nodeConnectsToTheseBranches = new int[][] { };

        public static int[][] tileConnectsToTheseNodes = new int[][] {};

        public static int[][] tileConnectsToTheseBranches = new int[][] { };



    }

}
