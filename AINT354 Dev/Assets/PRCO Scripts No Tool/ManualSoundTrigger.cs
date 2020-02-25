using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSoundTrigger : MonoBehaviour
{
    private bool inTrigger = false;

    //0 = sound 1, 1 = sequential, 2 = simultanious
    public int soundType = 0;

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
        if (inTrigger && Input.GetButtonDown("Interact"))
        {
            //Play sound
            switch (soundType)
            {
                case 0:
                    soundToPlayOne.triggerAudio(false);
                    break;
                case 1:
                    soundToPlayOne.triggerAudio(true);
                    break;
                case 2:
                    soundToPlayOne.triggerAudio(false);
                    soundToPlayTwo.triggerAudio(false);
                    soundToPlayThree.triggerAudio(false);
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
