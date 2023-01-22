using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Asteroid;

public class AsteroidController : MonoBehaviour
{
    public delegate void OnAsteroidSpawnedEvent(Asteroid asteroid);
    public static OnAsteroidSpawnedEvent onAsteroidSpawned;

    public GameObject smallAsteroid;
    public GameObject medAsteroid;
    public GameObject largeAsteroid;

    public Transform ship;

    bool allowFiring = true;

    public AudioSource alertSource;

    void Start()
    {
        StartCoroutine(SpawnAsteroidCR());
    }
    void Update(){
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
        asteroid.transform.position = ship.transform.position + (Vector3.up * SpawnHeight) + (Vector3.right * Random.Range(-10f, 10f));
        asteroid.GetComponent<Asteroid>().target = ship.transform.position;

        onAsteroidSpawned?.Invoke(asteroid.GetComponent<Asteroid>());
        
        GetComponent<AudioSource>().Play();
    }

    public float SpawnHeight = 7f;
    float minSpawnWait = 10f;
    float maxSpawnWait = 25f;
    IEnumerator SpawnAsteroidCR()
    {
        while(true)
        {
            if(GameController.Inst.GAME_OVER)
                yield break;

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
