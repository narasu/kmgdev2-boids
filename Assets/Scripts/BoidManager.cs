using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numBoids = 10;
    List<Boid> boids = new List<Boid>();
    [Range(0.01f, 1)] public float separationWeight;
    [Range(0.01f, 1)] public float alignmentWeight;
    [Range(0.01f, 1)] public float cohesionWeight;
    public Transform preferredTransform;
    private Vector3 preferredPosition;
    [Range(0.01f,1)] public float stayNearPositionWeight;
    
    public float minDistance = 2f;
    public float maxVelocity = 4f;

    private void Start()
    {
        preferredPosition = preferredTransform.position;
        for (int i = 0; i < numBoids; i++)
        {
            GameObject b = Instantiate(boidPrefab);
            b.transform.position = new Vector3(Random.Range(-40f, 40f), Random.Range(-40f, 40f), Random.Range(-40f, 40f));
            boids.Add(b.GetComponent<Boid>());
        }
    }

    private void Update()
    {
        ApplyVelocity();
        
    }

    Vector3 RuleCohesion(Boid _boid)
    {
        Vector3 centerOfMass = new Vector3();
        foreach (Boid b in boids)
        {
            if (b == _boid)
            {
                continue;
            }

            centerOfMass += b.transform.position;
        }
        centerOfMass /= boids.Count - 1;
        return (centerOfMass - _boid.transform.position) * cohesionWeight;
    }
    
    Vector3 RuleSeparation(Boid _boid)
    {
        Vector3 displacement = Vector3.zero;
        foreach (Boid b in boids)
        {
            if (b == _boid)
            {
                continue;
            }
            if (Vector3.Distance(_boid.transform.position, b.transform.position) < minDistance)
            {
                displacement -= b.transform.position - _boid.transform.position;
            }
        }
        return displacement * separationWeight;
    }
    
    Vector3 RuleAlignment(Boid _boid)
    {
        Vector3 totalVelocity = new Vector3();

        foreach (Boid b in boids)
        {
            if (b == _boid)
            {
                continue;
            }
            totalVelocity += b.velocity;
        }
        Vector3 newVelocity = new Vector3(totalVelocity.x / boids.Count-1, totalVelocity.y / boids.Count-1, totalVelocity.z / boids.Count-1);
        return (newVelocity - _boid.velocity) * alignmentWeight;
    }

    Vector3 StayNearPosition(Boid _boid)
    {
        Vector3 boidPosition = _boid.transform.position;
        float dist = Vector3.Distance(boidPosition, preferredPosition);
        Vector3 velocityAdjustment = Vector3.zero;
        if (boidPosition.x < preferredPosition.x)
        {
            velocityAdjustment.x = stayNearPositionWeight;
        }
        else if (boidPosition.x > preferredPosition.x)
        {
            velocityAdjustment.x = -stayNearPositionWeight;
        }
        
        if (boidPosition.y < preferredPosition.y)
        {
            velocityAdjustment.y = stayNearPositionWeight;
        }
        else if (boidPosition.y > preferredPosition.y)
        {
            velocityAdjustment.y = -stayNearPositionWeight;
        }
        
        if (boidPosition.z < preferredPosition.z)
        {
            velocityAdjustment.z = stayNearPositionWeight;
        }
        else if (boidPosition.z > preferredPosition.z)
        {
            velocityAdjustment.z = -stayNearPositionWeight;
        }

        return velocityAdjustment;
    }
    

    void ApplyVelocity()
    {
        foreach(Boid b in boids)
        {
            Vector3 v1 = RuleCohesion(b);
            Vector3 v2 = RuleSeparation(b);
            Vector3 v3 = RuleAlignment(b);
            Vector3 v4 = StayNearPosition(b);

            Vector3 velocity = v1 + v2 + v3 + v4;
            b.velocity += velocity;
            b.velocity = Vector3.ClampMagnitude(b.velocity, maxVelocity);
            b.transform.position += b.velocity * Time.deltaTime;
            b.transform.rotation = Quaternion.Lerp(b.transform.rotation, Quaternion.LookRotation(b.velocity), 0.1f);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(preferredTransform.position, 1.0f);
    }
}
