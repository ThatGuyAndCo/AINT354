using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualPlatformTrigger2 : MonoBehaviour
{
    private bool inTrigger = false;
    //public MovingPlatform platformToMove;
    CustomEventTrigger eventTriggers;

    // Start is called before the first frame update
    void Start()
    {
        eventTriggers = gameObject.GetComponent<CustomEventTrigger>();

        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call platform", //Trigger Name
            "MovingPlatform2", //Script Name
            "triggerMovement", //Method Name
            "secondPlatform", //Tag
            new object[] { false, true } //Parameters for method being called
        ));
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && Input.GetButtonDown("Interact"))
        {
            //Move platform
            //platformToMove.triggerMovement(false, true);
            eventTriggers.triggerList[0].fire();
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
