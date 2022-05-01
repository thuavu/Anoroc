using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveDrawer : MonoBehaviour
{

    [SerializeField] private Vector3 openPosition, closedPosition;

    [SerializeField] private float animationTime;

    [SerializeField] private bool isOpen = false;

    [SerializeField] private MovementType movementType;

    private enum MovementType { Slide, Rotate };

    // used to set up local positions
    private Hashtable iTweenArgs;

    void Start()
    {

        // this makes it so that it doesn't move globally
        iTweenArgs = iTween.Hash();
        iTweenArgs.Add("position", openPosition);
        iTweenArgs.Add("time", animationTime);
        iTweenArgs.Add("islocal", true);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (isOpen)
            {
                iTweenArgs["position"] = closedPosition;
            }
            else
            {
                iTweenArgs["position"] = openPosition;
            }

            isOpen = !isOpen;

            iTween.MoveTo(gameObject, iTweenArgs);
        }
    }
}
