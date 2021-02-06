using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameObjectProperties;

public class BranchController : MonoBehaviour
{
    public Sprite playerOneSprite;
    public Sprite playerTwoSprite;
    public Sprite blankSprite;

    public Branch branchEntity;

    void Start()
    {
        // Mark Branch Unowned at start
        ClaimBranch(blankSprite);
    }

    private void OnMouseDown()
    {
        // Determine if current player can place here. (As of now simply meaning you can't override
        if(isBranchBlank() || isBranchSurroundedByCurrentPlayer())
        {
            branchEntity.branchState.ownerColor = branchEntity.gameController.getCurrentPlayerColor();
            branchEntity.branchState.branchColor = branchEntity.gameController.getCurrentPlayerColor();

            // Change color
            if (Game.playerOneTurn)
                ClaimBranch(playerOneSprite);
            else
                ClaimBranch(playerTwoSprite);
        }
        // Are you trying to undo a selection?
        else if (isBranchColorOfCurrentPlayer())
        {
            branchEntity.branchState.ownerColor = PlayerColor.Blank;
            branchEntity.branchState.branchColor = PlayerColor.Blank;

            ClaimBranch(blankSprite);
        }

        // Print
        print("You clicked on:\n" + this.GetComponent<Renderer>().name + ", ID: " + branchEntity.id + ", Color: ");
        if (GetComponent<SpriteRenderer>().sprite == playerOneSprite)
            print("Orange\n");
        else if (GetComponent<SpriteRenderer>().sprite == playerTwoSprite)
            print("Purple\n");
        else
            print("Blank\n");
    }

    private void ClaimBranch(Sprite playerColor)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = playerColor;
    }

    private bool isBranchBlank()
    {
        return branchEntity.branchState.ownerColor == PlayerColor.Blank;
    }

    private bool isBranchSurroundedByCurrentPlayer()
    {
        return ((branchEntity.branchState.ownerColor == branchEntity.gameController.getCurrentPlayerColor()) && (branchEntity.branchState.branchColor == PlayerColor.Blank));
    }

    private bool isBranchColorOfCurrentPlayer()
    {
        return branchEntity.branchState.branchColor == branchEntity.gameController.getCurrentPlayerColor();
    }
}
