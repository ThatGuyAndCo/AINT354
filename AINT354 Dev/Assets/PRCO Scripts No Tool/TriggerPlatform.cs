using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatform : MonoBehaviour
{
    public bool startPlatform = false;
    public MovingPlatform platformToMove;
    public SoundPlayer soundToPlayOne;
    public SoundPlayer soundToPlayTwo;
    public SoundPlayer soundToPlayThree;

    // Start is called before the first frame update
    void Start()
    {
        
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
                platformToMove.triggerMovement(false, true);
            }
            else
            {
                //Trigger victory sound
                float randNumber = Random.Range(0, 0.9f);

                if(randNumber >= 0.3f)
                {
                    soundToPlayOne.triggerAudio(false);
                }
                else if (randNumber >= 0.6f)
                {
                    soundToPlayTwo.triggerAudio(false);
                }
                else
                {
                    soundToPlayThree.triggerAudio(false);
                }
            }
        }
    }
}
