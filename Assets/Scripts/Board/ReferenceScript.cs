using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceScript
{
    public int[][] branchConnectsToTheseBranches = new int[][]
    {
        new int[2] {1, 2},
        new int[4] {0, 3, 4, 7},
        new int[4] {0, 4, 5, 8},
        new int[4] {1, 4, 6, 7},
        new int[6] {1, 2, 3, 5, 6, 7},
        new int[4] {2, 4, 8, 9},

    };

    public static int[][] nodeConnectsToTheseBranches = new int[][]
    {
        new int[2] {0, 1},
        new int[2] {0, 2},
        new int[2] {3, 6},
        new int[4] {1, 3, 4, 7},
        new int[4] {2, 4, 5, 8},
        new int[2] {5, 9},
        new int[2] {10, 15},
        new int[4] {6, 10, 11, 16},
        new int[4] {7, 11, 12, 17},
        new int[4] {8, 12, 13, 18},
        new int[4] {9, 13, 14, 19},
        new int[2] {4, 20 },
        new int[2] {15, 21},
        new int[4] {16, 21, 22, 26},
        new int[4] {17, 22, 23, 27},
        new int[4] {18, 23, 24, 28},
        new int[4] {19, 24, 25, 29},
        new int[2] {20, 25},
        new int[2] {26, 30},
        new int[4] {27, 30, 31, 33},
        new int[4] {28, 31, 32, 34},
        new int[2] {29, 32},
        new int[2] {33, 35},
        new int[2] {34, 35},
    };

    public int[][] tileConnectsToTheseNodes = new int[][] { };

    public int[][] tileConnectsToTheseBranches = new int[][] { };



}
