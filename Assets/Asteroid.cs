using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public enum AsteroidType
    {
        Small,
        Medium,
        Large
    }
    public AsteroidType type;
    public Vector3 target;
    public float MoveSpeed = 5f;
    public float RotateSpeed = 5f;
    public bool started = false;

    void Update()
    {
        if(!started) return;
        
        transform.position += (target - transform.position).normalized * MoveSpeed * Time.deltaTime;

        transform.RotateAround(transform.position, Vector3.forward, RotateSpeed * Time.deltaTime);
    }
}
