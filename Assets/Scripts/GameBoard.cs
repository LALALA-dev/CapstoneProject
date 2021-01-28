using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard
{
    const int MAX_TILES = 13;
    const int MAX_NODES = 24;
    const int MAX_BRANCHES = 36;

    public PropertyTile[] tiles = new PropertyTile[MAX_TILES];
    public Node[] nodes = new Node[MAX_NODES];
    public Branch[] branches = new Branch[MAX_BRANCHES];

    public GameBoard()
    {
        for(int i = 0; i < MAX_TILES; i++)
        {
            tiles[i] = new PropertyTile();
        }

        for (int i = 0; i < MAX_NODES; i++)
        {
            nodes[i] = new Node();
        }

        for(int i = 0; i < MAX_BRANCHES; i++)
        {
            branches[i] = new Branch();
        }
    }

    public GameBoard(PropertyTile[] tileState)
    {
        for (int i = 0; i < MAX_TILES; i++)
        {
            tiles[i] = tileState[i];
        }
    }

    public GameBoard(PropertyTile[] tileState, Node[] nodeState, Branch[] branchState)
    {
        for(int i = 0; i < MAX_TILES; i++)
        {
            tiles[i] = tileState[i];
        }

        for(int i = 0; i < MAX_NODES; i++)
        {
            nodes[i] = nodeState[i];
        }

        for(int i = 0; i < MAX_BRANCHES; i++)
        {
            branches[i] = branchState[i];
        }
    }

}
