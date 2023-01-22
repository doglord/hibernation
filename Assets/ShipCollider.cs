using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipCollider : MonoBehaviour
{
    public delegate void OnShipCollisionEvent(Asteroid asteroid);
    public static OnShipCollisionEvent onShipCollision;
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.tag == "Asteroid")
        {
            // check hit
            Debug.Log("HIT");
            onShipCollision?.Invoke(coll.gameObject.GetComponent<Asteroid>());
        }
    }
}
