using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveSafe : MonoBehaviour
{
    [SerializeField] private Vector3 openRotation, closedRotation;

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
        iTweenArgs.Add("rotation", openRotation);
        iTweenArgs.Add("time", animationTime);
        iTweenArgs.Add("islocal", true);
        iTweenArgs.Add("easetype", iTween.EaseType.linear);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (isOpen)
            {
                iTweenArgs["position"] = closedRotation;
            }
            else
            {
                iTweenArgs["position"] = openRotation;
            }

            isOpen = !isOpen;

            iTween.MoveTo(gameObject, iTweenArgs);
        }
    }
}
