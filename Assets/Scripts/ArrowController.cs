using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private float StartingCoordinateX = 0.0f;
    private float StartingCoordinareY = 0.0f;
    private float MaxPostitionY = 0.0f;
    private float MaxPostitionZ = 0.0f;
    private float RotationX = 0.0f;
    private float RotationY = 0.0f;
    private float RotationZ = 0.0f;

    public float speedX = .01f;
    public float speedY = .01f;

    void Start()
    {
        gameObject.SetActive(true);
        EnableArrow();
    }

    public void EnableArrow()
    {
        gameObject.SetActive(true);
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + speedX, gameObject.transform.position.y + speedY);
            yield return new WaitForSeconds(.5f);
        }
    }
}
