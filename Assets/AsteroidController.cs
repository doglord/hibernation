using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Asteroid;

public class AsteroidController : MonoBehaviour
{
    public GameObject smallAsteroid;
    public GameObject medAsteroid;
    public GameObject largeAsteroid;

    public Transform ship;

    bool allowFiring = true;
    void Start()
    {
        StartCoroutine(SpawnAsteroidCR());
    }
    void Update(){
        if(Input.GetKeyDown(KeyCode.J)) SpawnAsteroid(AsteroidType.Small);
        if(Input.GetKeyDown(KeyCode.K)) SpawnAsteroid(AsteroidType.Medium);
        if(Input.GetKeyDown(KeyCode.L)) SpawnAsteroid(AsteroidType.Large);
    }

    void SpawnAsteroid(AsteroidType type)
    {
        GameObject asteroid;
        switch(type)
        {
            case AsteroidType.Small:
                asteroid = Instantiate(smallAsteroid);
                break;
            case AsteroidType.Medium:
                asteroid = Instantiate(medAsteroid);
                break;
            case AsteroidType.Large:
                asteroid = Instantiate(largeAsteroid);
                break;
            default:
                asteroid = new GameObject();
                break;
        }

        // place & set target
        asteroid.transform.position = ship.transform.position + (Vector3.up * 10) + (Vector3.right * Random.Range(-10f, 10f));
        asteroid.GetComponent<Asteroid>().target = ship.transform.position;
    }

    float minSpawnWait = 10f;
    float maxSpawnWait = 45f;
    IEnumerator SpawnAsteroidCR()
    {
        while(true)
        {
            if(!allowFiring)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnWait, maxSpawnWait));
                continue;
            }
            yield return new WaitForSeconds(Random.Range(minSpawnWait, maxSpawnWait));
            SpawnAsteroid((AsteroidType)Random.Range(0, 3));
        }
    }
}
