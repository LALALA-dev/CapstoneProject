using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ExpertAI;

public class TreeNode
{
    public MyBoard localBoard;
    public List<TreeNode> child;
    public TreeNode parent;
    public int level;
    public int N; // visited times
    public int W; // number of wins
    public TreeNode(MyBoard myBoard)
    {
        this.localBoard = myBoard;
        this.child = new List<TreeNode>();
        this.level = 0;
        this.N = 0;
        this.W = 0;
    }

    public TreeNode()
    {
        this.child = new List<TreeNode>();
        this.level = 0;
        this.N = 0;
        this.W = 0;
    }

    public TreeNode Copy()
    {
        TreeNode result = new TreeNode();
        result.W = this.W;
        result.N = this.N;
        result.level = this.level;
        result.localBoard = this.localBoard.Clone();
        result.parent = this.parent;
        result.child = this.child;
        return result;
    }

    public void AddChild(MyBoard myBoard)
    {
        TreeNode temp = new TreeNode(myBoard);
        this.child.Add(temp);
        temp.parent = this;
        temp.level = this.level + 1;
    }

}
