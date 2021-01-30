using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Sprite[] images;
    public SpriteRenderer[] spriteRenderers;

    private void Start()
    {
        ReshuffleBoard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReshuffleBoard();
        }
    }

    public void ReshuffleBoard()
    {
        for (int i = 0; i < images.Length; i++)
        {
            Sprite tmp = images[i];
            int r = Random.Range(i, images.Length);
            images[i] = images[r];
            images[r] = tmp;
        }

        for(int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = images[i];
        }
    }

    public void EndCurrentPlayersTurn()
    {
        Game.playerOneTurn = !Game.playerOneTurn;
    }

}
