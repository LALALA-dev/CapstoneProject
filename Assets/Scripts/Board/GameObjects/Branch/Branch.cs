using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class Branch : MonoBehaviour
{
    public int id;
    public BranchState branchState;

    public GameController gameController;

    void Awake()
    {
        branchState.location = id;
        branchState.ownerColor = branchState.branchColor = PlayerColor.Blank;

        gameController = GameController.getInstance();
        gameController.getGameBoard().branches[id] = this;
    }
}
