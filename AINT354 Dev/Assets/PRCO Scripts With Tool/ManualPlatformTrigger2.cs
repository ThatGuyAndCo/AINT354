using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualPlatformTrigger2 : MonoBehaviour
{
    private bool inTrigger = false;
    //public MovingPlatform platformToMove;
    CustomEventMaster eventSys;

    // Start is called before the first frame update
    void Start()
    {
        eventSys = Object.FindObjectOfType<CustomEventMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && Input.GetButtonDown("Interact"))
        {
            //Move platform
            //platformToMove.triggerMovement(false, true);
            eventSys.sendEvent("triggerMovement", "secondPlatform", false, "MovingPlatform2", new object[] { false, true });
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
