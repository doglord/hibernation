using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public Transform WaypointA;
    public Transform WaypointB;

    void Start()
    {
        transform.position = WaypointA.position;
    }

    void Update()
    {
        var t = GameController.Inst.seconds / GameController.Inst.totalTime.TotalSeconds;
        transform.position = Vector3.Lerp(WaypointA.position, WaypointB.position, (float)t);
    }
}
