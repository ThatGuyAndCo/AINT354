using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCamera : MonoBehaviour
{

    public float vertSensitivity = 1.5f;
    public float maxRot = 80.0f;
    public float minRot = -80.0f;
    private float currRot = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseVec = Input.GetAxis("Mouse Y");

        currRot = Mathf.Clamp(currRot + (-mouseVec * vertSensitivity), minRot, maxRot);
        transform.localRotation = Quaternion.identity;
        transform.Rotate(Vector3.right, currRot);
    }
}
