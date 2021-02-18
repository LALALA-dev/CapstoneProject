using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceScript
{
    public static int[][] branchConnectsToTheseBranches = new int[][]
    {
        new int[2] {1, 2},
        new int[4] {0, 3, 4, 7},
        new int[4] {0, 4, 5, 8},
        new int[4] {1, 4, 6, 7},
        new int[6] {1, 2, 3, 5, 7, 8}, // 4
        new int[4] {2, 4, 8, 9},
        new int[4] {3, 10, 11, 16},
        new int[6] {1, 3, 4, 11, 12, 17},
        new int[6] {2, 4, 5, 12, 13, 18},
        new int[4] {5, 13, 14, 19}, // 9
        new int[4] {6, 11, 15, 16},
        new int[6] {6, 7, 10 , 12, 16, 17},
        new int[6] {7, 8, 11, 13, 17, 18},
        new int[6] {8, 9, 12, 14, 18, 19},
        new int[4] {9, 13, 19, 20}, // 14
        new int[2] {10, 21},
        new int[6] {6, 10, 11, 21, 22, 26},
        new int[6] {7, 11, 12, 22, 23, 27},
        new int[6] {8, 12, 13, 23, 24, 28},
        new int[6] {9, 13, 14, 24, 25, 29}, // 19
        new int[2] {14, 25},
        new int[4] {15, 16, 22, 26},
        new int[6] {16, 17, 21, 23, 26, 27},
        new int[6] {17, 18, 22, 24, 27, 28},
        new int[6] {18, 19, 23, 25, 28, 29}, // 24
        new int[4] {19, 20, 24, 29},
        new int[4] {16, 21, 22, 30},
        new int[6] {17, 22, 23, 30, 31, 33},
        new int[6] {18, 23, 24, 31, 32, 34},
        new int[4] {19, 24, 25, 32}, // 29
        new int[4] {26, 27, 31, 33},
        new int[6] {27, 28, 30, 32, 33, 34},
        new int[4] {28, 29, 31, 34},
        new int[4] {27, 30, 31, 35},
        new int[4] {28, 31, 32, 35}, // 34
        new int[2] {33, 34}
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
        new int[2] {34, 35}
    };

    public static int[][] nodeConnectToTheseTiles = new int[][]
    {
        new int[1] {0},
        new int[1] {0},
        new int[1] {1},
        new int[3] {0, 1, 2},
        new int[3] {0, 2, 3},
        new int[1] {3},
        new int[1] {4},
        new int[3] {1, 4, 5},
        new int[4] {1, 2, 5, 6},
        new int[4] {2, 3, 6, 7},
        new int[3] {3, 7, 8},
        new int[1] {8},
        new int[1] {4},
        new int[3] {4, 5, 9},
        new int[4] {5, 6, 9, 10},
        new int[4] {6, 7, 10, 11},
        new int[3] {7, 8, 11},
        new int[1] {8},
        new int[1] {9},
        new int[3] {9, 10, 12},
        new int[3] {10, 11, 12},
        new int[1] {11},
        new int[1] {12},
        new int[1] {12}
    };

    public static int[][] tileConnectsToTheseNodes = new int[][]
    {
        new int[4] {0, 1, 3, 4},
        new int[4] {2, 3, 7, 8},
        new int[4] {3, 4, 8, 9},
        new int[4] {4, 5, 9, 10},
        new int[4] {6, 7, 12, 13},
        new int[4] {7, 8, 13, 14},
        new int[4] {8, 9, 14, 15},
        new int[4] {9, 10, 15, 16},
        new int[4] {10, 11, 16, 17},
        new int[4] {13, 14, 18, 19},
        new int[4] {14, 15, 19, 20},
        new int[4] {15, 16, 20, 21},
        new int[4] {19, 20, 22, 23}
    };

    public static int[][] tileConnectsToTheseBranches = new int[][] 
    {
        new int[4] {0, 1, 3, 4},
        new int[4] {3, 6, 7, 11},
        new int[4] {4, 7, 8, 12},
        new int[4] {5, 8, 9, 13},
        new int[4] {10, 15, 16, 21},
        new int[4] {11, 16, 17, 22},
        new int[4] {12, 17, 18, 23},
        new int[4] {13, 18, 19, 24},
        new int[4] {14, 19, 20, 25},
        new int[4] {22, 26, 27, 30},
        new int[4] {23, 27, 28, 31},
        new int[4] {24, 28, 29, 32},
        new int[4] {31, 33, 34, 35}
    };

}
