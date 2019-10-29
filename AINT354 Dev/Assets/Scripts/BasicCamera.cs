using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCamera : MonoBehaviour
{
    [Header("Anchor")]
    public GameObject anchor;
    public GameObject cameraObj;
    [Header("Movement Variables")]
    public float vertSensitivity = 1.5f;
    public float horizSensitivity = 2.0f;
    public float smoothing = 0.5f;
    public float maxYRot = 80.0f;
    public float minYRot = -80.0f;
    public float xRotationClamp = 80.0f;
    private float currYRot = 0.0f;
    private float currXRot = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseVec = new Vector2(Input.GetAxis("Camera X"), Input.GetAxis("Camera Y"));

        currYRot = Mathf.Clamp(currYRot + (-mouseVec.y * vertSensitivity), minYRot, maxYRot);
        currXRot = currXRot + (mouseVec.x * horizSensitivity);

        Vector3 smoothedPosition = Vector3.Slerp(transform.position, anchor.transform.position, smoothing);
        transform.position = smoothedPosition;

        /*if (mouseVec.x != 0.0f) {
            transform.localRotation = Quaternion.identity;
            transform.Rotate(currXRot, currYRot, 0);
        }*/

        transform.localRotation = Quaternion.identity;
        transform.rotation = Quaternion.Euler(currYRot, currXRot, 0);
    }
}
