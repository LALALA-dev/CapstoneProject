using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
    public Sprite playerOne;
    public Sprite playerTwo;
    public Sprite unowned;

    void Start()
    {
        ClaimNode(unowned);
    }

    private void OnMouseDown()
    {
        if (Game.playerOneTurn)
        {
            ClaimNode(playerOne);
        }
        else
        {
            ClaimNode(playerTwo);
        }
    }

    private void ClaimNode(Sprite playerColor)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = playerColor;
    }
}
