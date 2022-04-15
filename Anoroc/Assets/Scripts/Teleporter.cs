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
            Item.transform.position = StartTeleporter.transform.position;
            //Item.transform.position = new Vector3(15.75f, 0.712f, 23.4f);
            
        }

        if (collision.gameObject.CompareTag("Teleporter2"))
        {
            Item.transform.position = TeleportTo.transform.position;
            Debug.Log("TELEPORT 2");
        }
    }
}