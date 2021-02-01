using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchController : MonoBehaviour
{
    public Sprite playerOne;
    public Sprite playerTwo;
    public Sprite unowned;

    void Start()
    {
        ClaimBranch(unowned);
    }

    private void OnMouseDown()
    {
        if (Game.playerOneTurn)
        {
            ClaimBranch(playerOne);
        }
        else
        {
            ClaimBranch(playerTwo);
        }
    }

    private void ClaimBranch(Sprite playerColor)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = playerColor;
    }
}
