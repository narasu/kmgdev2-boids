using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int numBoids = 10;
    List<Boid> boids = new List<Boid>();
    [Range(0.01f,10)]
    public float separationWeight;
    [Range(1,10)]
    public float alignmentWeight;
    [Range(1,100)]
    public float cohesionWeight;
    public float minDistance = 2f;
    public float speed = 4f;
    public float rotationalInterpolation = 0.4f;

    private void Start()
    {
        for (int i = 0; i < numBoids; i++)
        {
            GameObject b = Instantiate(boidPrefab);
            b.transform.position = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f));
            b.GetComponent<Boid>().rotationalInterpolation = rotationalInterpolation;
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
        return (centerOfMass - _boid.transform.position) / cohesionWeight;
    }
    
    Vector3 RuleSeparation(Boid _boid)
    {
        Vector3 displacement = new Vector3();
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
        return newVelocity / alignmentWeight;
    }
    

    void ApplyVelocity()
    {
        foreach(Boid b in boids)
        {
            Vector3 v1 = RuleCohesion(b);
            Vector3 v2 = RuleSeparation(b);
            Vector3 v3 = RuleAlignment(b);

            Vector3 velocity = (v1 + v2 + v3).normalized;
            b.velocity = Vector3.ClampMagnitude(velocity * speed, speed * 2);
            //b.transform.Translate(b.velocity);
            //b.transform.Rotate(...);
        }
        
    }
}
