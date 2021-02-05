using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class NodeController : MonoBehaviour
{
    public Sprite playerOneSprite;
    public Sprite playerTwoSprite;
    public Sprite blankSprite;

    public Node nodeEntity;

    void Start()
    {
        // Mark Branch Unowned at start
        ClaimNode(blankSprite);
    }

    private void OnMouseDown()
    {
        // Determine if current player can place here. (As of now simply meaning you can't override
        if (isNodeBlank())
        {
            nodeEntity.nodeState.ownerColor = nodeEntity.gameController.getCurrentPlayerColor();
            nodeEntity.nodeState.nodeColor = nodeEntity.gameController.getCurrentPlayerColor();

            // Change color
            if (Game.playerOneTurn)
                ClaimNode(playerOneSprite);
            else
                ClaimNode(playerTwoSprite);
        }
        // Are you trying to undo a selection?
        else if (isNodeColorOfCurrentPlayer())
        {
            ClaimNode(blankSprite);
        }

        // Print
        print("You clicked on:\n" + this.GetComponent<Renderer>().name + ", ID: " + nodeEntity.id + ", Color: ");
        if (GetComponent<SpriteRenderer>().sprite == playerOneSprite)
            print("Orange\n");
        else if (GetComponent<SpriteRenderer>().sprite == playerTwoSprite)
            print("Purple\n");
        else
            print("Blank\n");
    }

    private void ClaimNode(Sprite playerColor)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = playerColor;
    }

    private bool isNodeBlank()
    {
        return nodeEntity.nodeState.ownerColor == PlayerColor.Blank;
    }


    private bool isNodeColorOfCurrentPlayer()
    {
        return nodeEntity.nodeState.nodeColor == nodeEntity.gameController.getCurrentPlayerColor();
    }
}
