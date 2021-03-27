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

    public void DisableArrow()
    {
        gameObject.SetActive(false);
    }

}
