using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipCollider : MonoBehaviour
{
    public delegate void OnShipCollisionEvent(Asteroid asteroid);
    public static OnShipCollisionEvent onShipCollision;
    public AudioSource alarm;
    public AudioSource DINK;
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "Asteroid")
        {
            // check hit
            Debug.Log("HIT");
            if (alarm.isPlaying)
                alarm.Stop();
            DINK.Play();
            onShipCollision?.Invoke(coll.gameObject.GetComponent<Asteroid>());
        }
    }
}
