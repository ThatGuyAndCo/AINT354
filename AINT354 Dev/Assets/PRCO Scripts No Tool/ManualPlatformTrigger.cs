using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualPlatformTrigger : MonoBehaviour
{
    private bool inTrigger = false;
    public MovingPlatform platformToMove;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && Input.GetButtonDown("Interact"))
        {
            //Move platform
            platformToMove.triggerMovement(false, true);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            inTrigger = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            inTrigger = false;
        }
    }
}
