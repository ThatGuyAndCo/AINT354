using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatform2 : MonoBehaviour
{
    public bool startPlatform = false;
    //public MovingPlatform platformToMove;
    //public SoundPlayer soundToPlayOne;
    //public SoundPlayer soundToPlayTwo;
    //public SoundPlayer soundToPlayThree;

    CustomEventMaster eventSys;

    // Start is called before the first frame update
    void Start()
    {
        eventSys = Object.FindObjectOfType<CustomEventMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            if (startPlatform)
            {
                //Trigger platform A movement
                //platformToMove.triggerMovement(false, true);
                eventSys.sendEvent("triggerMovement", "firstPlatform", false, "MovingPlatform2", new object[] { false, true });
            }
            else
            {
                //Trigger victory sound
                float randNumber = Random.Range(0, 0.9f);

                if(randNumber >= 0.3f)
                {
                    //soundToPlayOne.triggerAudio(false);
                    eventSys.sendEvent("triggerAudio", "soundOne", false, "SoundPlayer2", new object[] { false });
                }
                else if (randNumber >= 0.6f)
                {
                    //soundToPlayTwo.triggerAudio(false);
                    eventSys.sendEvent("triggerAudio", "soundTwo", false, "SoundPlayer2", new object[] { false });
                }
                else
                {
                    //soundToPlayThree.triggerAudio(false);
                    eventSys.sendEvent("triggerAudio", "soundThree", false, "SoundPlayer2", new object[] { false });
                }
            }
        }
    }
}
