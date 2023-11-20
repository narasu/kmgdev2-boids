using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float rotationalInterpolation;
    public Vector3 velocity;
    Vector3 lastVelocity;

    private void LateUpdate()
    {
        transform.position += velocity * Time.deltaTime;
    }
}