using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundPlayer2 : CustomEventHandler
{
    private AudioSource myAudio;
    //public SoundPlayer nextToPlay;
    private bool sequential = false;
    CustomEventMaster eventSys;

    // Start is called before the first frame update
    void Start()
    {
        myAudio = gameObject.GetComponent<AudioSource>();
        eventSys = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
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
                //eventSys.sendEvent("triggerAudio", "soundTwo", false, "SoundPlayer2", new object[] { true });
            }
            else if(handlerTag == "soundTwo")
            {
                //eventSys.sendEvent("triggerAudio", "soundThree", false, "SoundPlayer2", new object[] { false });
            }
            else
            {
                sequential = false;
            }
        }
    }

    public bool triggerAudio(bool sequential)
    {
        bool seq;
        try
        {
            seq = sequential;
        }
        catch (Exception ex)
        {
            Debug.Log("Error occured at triggerAudio. Failed to cast sequential to bool. Error is as follows:");
            Debug.Log(ex);
            return false;
        }
        myAudio.Play();
        this.sequential = seq;
        return true;
    }
}
