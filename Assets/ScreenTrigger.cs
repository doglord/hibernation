using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Asteroid asteroid;
    public AudioSource alarm;
    public List<AudioClip> clips; 
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(GameController.Inst.GAME_OVER)
            return;
            
        // check for asteroid in screen
        // Debug.Log("ASTEROID APPROACHING");  
        if(coll.tag == "Asteroid")
        {
            asteroid = coll.gameObject.GetComponent<Asteroid>();
            switch (asteroid.type)
            {
                case Asteroid.AsteroidType.Small:
                    alarm.PlayOneShot(clips[0]);
                    break;
                case Asteroid.AsteroidType.Medium:
                    alarm.PlayOneShot(clips[1]);
                    break;
                case Asteroid.AsteroidType.Large:
                    alarm.PlayOneShot(clips[2]);
                    break;
                default:
                    alarm.PlayOneShot(clips[0]);
                    break;
            }
        }
    }
}
