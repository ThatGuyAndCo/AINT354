using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Vector3 startPosition;
    public Vector3 endPosition;
    public float smoothing = 5f;

    private float count = 0.0f;
    private bool movePlatform = false;
    private bool reverse = false;
    private bool moveNextPlatformAtEnd = false;

    public MovingPlatform nextPlatformToMove;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
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
                if(transform.position == endPosition)
                {
                    movePlatform = false;
                    if (moveNextPlatformAtEnd)
                    {
                        Debug.Log("moveNextAtEnd");
                        nextPlatformToMove.triggerMovement(false, false);
                        moveNextPlatformAtEnd = false;
                    }
                    triggerMovement(true, false);
                }
            }
        }
    }

    public void triggerMovement(bool reverse, bool moveNextPlatformAtEnd)
    {
        if (!movePlatform)
        {
            this.reverse = reverse;
            movePlatform = true;
            this.moveNextPlatformAtEnd = moveNextPlatformAtEnd;
            count = 0.0f;
        }
    }

}
