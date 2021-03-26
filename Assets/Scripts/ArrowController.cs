using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private float StartingCoordinateX = 0.0f;
    private float StartingCoordinareY = 0.0f;
    private float MaxPostitionY = 0.0f;
    private float MaxPostitionX = 0.0f;
    private float RotationX = 0.0f;
    private float RotationY = 0.0f;
    private float RotationZ = 0.0f;

    public float Speed = 5f;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // gameObject.transform.position = new Vector3(gameObject.transform.position.x + Speed, gameObject.transform.position.y + Speed);
    }

    public void EnableArrow(float sX, float sY, float mY, float mX, float rX, float rY, float rZ, float speed)
    {
        gameObject.SetActive(true);
        // MaxPostitionX = mX;
        // MaxPostitionY = mY;
        transform.position = new Vector3(sX, sY);
        transform.eulerAngles = new Vector3(0, 0, rZ);
        // Speed = speed;
    }

    public void DisableArrow()
    {
        gameObject.SetActive(false);
    }

}
