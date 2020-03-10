using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    //False = +angle = open clockwise from hinges, True = -angle = open east from hinges
    public bool openReverse = false;

    private CustomEventTrigger eventTriggers;
    private int triggerWithParamsIndex;

    void Start()
    {
        eventTriggers = GetComponent<CustomEventTrigger>(); //Get ref to trigger script on object

        eventTriggers.triggerList.Add(new ce_TriggerStruct(
                "DoorHandler", //Script file name, case sensitive
                "OpenDoor", //Method name, case sensitive
                "KitchenToMainHall", //Tag for Handler
                new object[] { true, 1.0f, "parameter 3", "etc..." }
            ));

        triggerWithParamsIndex = triggerList.Count - 1; //Get the index of the newly added trigger
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            eventTriggers.triggerList[triggerWithParamsIndex].fire(); //Fire the event we added in the Start Method.
        }
    }
}
