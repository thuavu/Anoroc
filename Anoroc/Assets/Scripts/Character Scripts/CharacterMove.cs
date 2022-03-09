using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class CharacterMove : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;

    private Rigidbody rb;
    private float moveSpeed;
    private float dirX, dirZ;


    // Start is called before the first frame update
    void Start() 
    {
        moveSpeed = 3f;
        rb = GetComponent<Rigidbody>();
    }

    /*public void Move(Vector3 point) {
        agent.SetDestination(point);
    }*/

    // Update is called once per frame
    void Update() 
    {
        dirX = Input.GetAxis("Horizontal") * moveSpeed;
        dirZ = Input.GetAxis("Vertical") * moveSpeed;
    }

    private void FixedUpdate() 
    {
        rb.velocity = new Vector3(dirX, rb.velocity.y, dirZ);
    }
}
