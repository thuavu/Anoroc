using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Teleporter : MonoBehaviour
{
    public GameObject Item;
    public GameObject StartTeleporter;
    public GameObject TeleportTo;
 
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Teleporter"))
        {
            Debug.Log("TELEPORT 1");
            Item.transform.position = new Vector3(TeleportTo.transform.position.x + 3f, TeleportTo.transform.position.y - 0.2f, TeleportTo.transform.position.z);  
        }

        if (collision.gameObject.CompareTag("Teleporter2"))
        {
            Debug.Log("TELEPORT 2");
            Item.transform.position = new Vector3(StartTeleporter.transform.position.x + 3f, StartTeleporter.transform.position.y + 0.2f, StartTeleporter.transform.position.z);
        }
    }
}