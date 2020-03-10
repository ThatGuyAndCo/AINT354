using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSoundTrigger2 : MonoBehaviour
{
    private bool inTrigger = false;

    //0 = sound 1, 1 = sequential, 2 = simultanious
    public int soundType = 0;
    public string soundTagToPlay;
    CustomEventMaster eventSys;

    //public SoundPlayer soundToPlayOne;
    //public SoundPlayer soundToPlayTwo;
    //public SoundPlayer soundToPlayThree;

    // Start is called before the first frame update
    void Start()
    {
        eventSys = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
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
                    //eventSys.sendEvent("triggerAudio", soundTagToPlay, false, "SoundPlayer2", new object[] { false });
                    break;
                case 1:
                    //eventSys.sendEvent("triggerAudio", "soundOne", false, "SoundPlayer2", new object[] { true });
                    break;
                case 2:
                    //eventSys.sendEvent("triggerAudio", "soundOne", false, "SoundPlayer2", new object[] { false });
                    //eventSys.sendEvent("triggerAudio", "soundTwo", false, "SoundPlayer2", new object[] { false });
                    //eventSys.sendEvent("triggerAudio", "soundThree", false, "SoundPlayer2", new object[] { false });
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
