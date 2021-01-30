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
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = unowned;
    }

    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if(Game.playerOneTurn)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.sprite = playerOne; 
        }
        else
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.sprite = playerTwo;
        }
    }
}
