using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class Node : MonoBehaviour
{
    public int id;
    public NodeState nodeState;

    public GameController gameController;

    void Start()
    {
        nodeState.location = id;
        nodeState.nodeColor = PlayerColor.Blank;

        gameController = GameController.getInstance();
        gameController.getGameBoard().nodes[id] = this;
    }
}
