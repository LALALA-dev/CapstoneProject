using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class GameBoard
{
    private const int MAX_SQUARES = 13;
    private const int MAX_NODES = 24;
    private const int MAX_BRANCHES = 36;

    public Square[] squares;
    public Node[] nodes;
    public Branch[] branches;

    private BoardState boardState;

    public GameBoard()
    {
        squares = new Square[MAX_SQUARES];
        nodes = new Node[MAX_NODES];
        branches = new Branch[MAX_BRANCHES];

        boardState.squareStates = new SquareState[MAX_SQUARES];
        boardState.nodeStates = new NodeState[MAX_NODES];
        boardState.branchStates = new BranchState[MAX_BRANCHES];
    }

    // Given an array of ResourceState, sets each square on the board to that layout.
    // Used to set up a custom gameboard.
    public void setSquares(SquareState[] squareStates)
    {
        int i = 0;
        foreach (SquareState squareState in squareStates)
        {
            squares[i].squareState = squareState;
            ++i;
        }
    }

    // Sets the board up as per the given arrays of SquareState, NodeState, and BranchState.
    // Used to set up a custom gameboard with pieces in desired locations.
    public void setBoard(SquareState[] resourceStates, NodeState[] nodeStates, BranchState[] branchStates)
    {
        int nodeIndex = 0, branchIndex = 0;

        setSquares(resourceStates);
        foreach (NodeState nodeState in nodeStates)
        {
            nodes[nodeIndex].nodeState = nodeState;
            ++nodeIndex;
        }
        foreach (BranchState branchState in branchStates)
        {
            branches[branchIndex].branchState = branchState;
            ++branchIndex;
        }
    }

    public BoardState getBoardState()
    {
        foreach (Square square in squares)
        {
            boardState.squareStates[square.id] = square.squareState;
        }
        foreach (Node node in nodes)
        {
            boardState.nodeStates[node.id] = node.nodeState;
        }
        foreach (Branch branch in branches)
        {
            boardState.branchStates[branch.id] = branch.branchState;
        }

        return boardState;
    }
}
