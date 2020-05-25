using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovingPlatform2 : CustomEventHandler
{
    private Vector3 startPosition;
    public Vector3 endPosition;
    public float smoothing = 5f;

    private float count = 0.0f;
    private bool movePlatform = false;
    private bool reverse = false;
    private bool moveNextPlatformAtEnd = false;

    //public MovingPlatform nextPlatformToMove;
    CustomEventTrigger eventTriggers;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;

        eventTriggers = gameObject.GetComponent<CustomEventTrigger>();
        String tagName;
        if (handlerTag.ToLower() == "firstplatform")
            tagName = "secondPlatform";
        else
            tagName = "firstPlatform";
        //Add a new ce_Trigger instance with parameters to the event trigger list
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call platform", //Trigger Name
            "MovingPlatform2", //Script Name
            "triggerMovement", //Method Name
            tagName, //Tag
            new object[] { false, false } //Parameters for method being called
        ));
    }

    // Update is called once per frame
    void Update()
    {
        if (movePlatform)
        {
            if (count < 60 * Time.deltaTime)
            {
                count += Time.deltaTime;
                return;
            }
            if (reverse)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition, Time.deltaTime * smoothing);
                if (transform.position == startPosition)
                {
                    movePlatform = false;
                    reverse = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, endPosition, Time.deltaTime * smoothing);
                if (transform.position == endPosition)
                {
                    movePlatform = false;
                    if (moveNextPlatformAtEnd)
                    {
                        Debug.Log("moveNextAtEnd");
                        //nextPlatformToMove.triggerMovement(false, false);
                        eventTriggers.triggerList[0].fire();
                        moveNextPlatformAtEnd = false;
                    }
                    triggerMovement(true, false);
                }
            }
        }
    }

    public bool triggerMovement(bool reverse, bool moveNextPlatformAtEnd)
    {
        if (!movePlatform)
        {
            try
            {
                this.reverse = reverse;
                this.moveNextPlatformAtEnd = moveNextPlatformAtEnd;
            }
            catch (Exception ex)
            {
                Debug.Log("Error assigning parameters in triggerMovement. Error is as follows:");
                Debug.Log(ex);
                return false;
            }
            movePlatform = true;
            count = 0.0f;
            return true;
        }
        return false;
    }

}
