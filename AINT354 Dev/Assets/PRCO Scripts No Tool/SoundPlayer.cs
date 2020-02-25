using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource myAudio;
    public SoundPlayer nextToPlay;
    private bool sequential = false;
    
    // Start is called before the first frame update
    void Start()
    {
        myAudio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (nextToPlay != null)
        {
            if (sequential && !myAudio.isPlaying)
            {
                sequential = false;
                nextToPlay.triggerAudio(true);
            }
        }
        else
        {
            sequential = false;
        }
    }

    public void triggerAudio(bool sequential)
    {
        myAudio.Play();
        this.sequential = sequential;
    }
}
