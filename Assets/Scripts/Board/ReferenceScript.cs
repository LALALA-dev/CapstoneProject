﻿using System.Collections;
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

    public int[][] nodeConnectsToTheseBranches = new int[][] { };

    public int[][] tileConnectsToTheseNodes = new int[][] { };

    public int[][] tileConnectsToTheseBranches = new int[][] { };



}
