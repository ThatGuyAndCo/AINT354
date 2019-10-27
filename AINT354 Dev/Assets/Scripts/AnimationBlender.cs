using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBlender : MonoBehaviour
{
    public Animation anim;
    public Transform spine;

    // Start is called before the first frame update
    void Start()
    {
        anim["xbot@Cross Punch"].AddMixingTransform(spine);
    }
}
