using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject: MonoBehaviour
{
    private void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log(collisionInfo.collider.name);
    }
}
