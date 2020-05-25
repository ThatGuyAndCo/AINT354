using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundPlayer2 : CustomEventHandler
{
    private AudioSource myAudio;
    //public SoundPlayer nextToPlay;
    private bool sequential = false;
    CustomEventTrigger eventTriggers;

    // Start is called before the first frame update
    void Start()
    {
        myAudio = gameObject.GetComponent<AudioSource>();
        eventTriggers = gameObject.GetComponent<CustomEventTrigger>();

        //Sequential sound 2
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call seq sound", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            "soundTwo", //Tag
            new object[] { true } //Parameters for method being called
        ));

        //Sequential sound 3
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call seq sound", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            "soundThree", //Tag
            new object[] { false } //Parameters for method being called
        ));

    }

    // Update is called once per frame
    void Update()
    {
        if (sequential && !myAudio.isPlaying)
        {
            sequential = false;
            if (handlerTag == "soundOne")
            {
                //nextToPlay.triggerAudio(true);
                eventTriggers.triggerList[0].fire();
            }
            else if(handlerTag == "soundTwo")
            {
                eventTriggers.triggerList[1].fire();
            }
        }
    }

    public bool triggerAudio(bool sequential)
    {
        myAudio.Play();
        this.sequential = sequential;
        return true;
    }

    public bool playMyAudio()
    {
        myAudio.Play();
        return true;
    }
}
