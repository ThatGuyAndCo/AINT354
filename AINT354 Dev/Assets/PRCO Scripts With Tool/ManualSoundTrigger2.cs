using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSoundTrigger2 : MonoBehaviour
{
    private bool inTrigger = false;

    //0 = sound 1, 1 = sequential, 2 = simultanious
    public int soundType = 0;
    public string soundTagToPlay;
    CustomEventTrigger eventTriggers;

    //public SoundPlayer2 soundToPlayOne;
    //public SoundPlayer2 soundToPlayTwo;
    //public SoundPlayer2 soundToPlayThree;

    // Start is called before the first frame update
    void Start()
    {
        eventTriggers = gameObject.GetComponent<CustomEventTrigger>();

        //Individual to trigger
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call my sound", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            soundTagToPlay, //Tag
            new object[] { false } //Parameters for method being called
        ));

        //Sequential
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call seq sound", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            "soundOne", //Tag
            new object[] { true } //Parameters for method being called
        ));

        //Simultainous
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call all sound", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            "sound", //Tag
            false, //Enable disabled objects
            false, //Exact tag match (default = true)
            new object[] { false } //Parameters for method being called
        ));
    }

    // Update is called once per frame
    void Update()
    {
        if (inTrigger && Input.GetButtonDown("Interact"))
        {
            //Play sound
            switch (soundType)
            {
                case 0:
                    eventTriggers.fireByName("Call my sound");
                    break;
                case 1:
                    eventTriggers.fireByName("Call seq sound");
                    break;
                case 2:
                    eventTriggers.fireByName("Call all sound");
                    break;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
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
