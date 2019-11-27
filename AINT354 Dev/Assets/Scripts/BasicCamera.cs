using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCamera : MonoBehaviour
{
    [Header("Anchor")]
    public GameObject anchor;
    public GameObject cameraVertRot;
    [Header("Movement Variables")]
    public float vertSensitivity = 1.5f;
    public float horizSensitivity = 2.0f;
    public float smoothing = 0.5f;
    public float maxYRot = 80.0f;
    public float minYRot = -50.0f;
    public float xRotationClamp = 80.0f;
    public float currYRot = 0.0f;
    public float currXRot = 0.0f;
    public float zoomLevel = 0.0f;
    [Header("Lock-On Variables")]
    public GameObject target;
    public bool lockedOn = false;

    // For note: Horizontal rot is considered rotation around Vec3.Up (i.e. looking left & right), and Vertical rot is around Vec3.Left (i.e. up and down)
    void Update()
    {
        if (Input.GetButtonDown("Lock-On"))
        {
            lockedOn = !lockedOn;
            currXRot = 0;
        }

        Vector3 smoothedPosition = Vector3.Slerp(transform.position, anchor.transform.position - transform.TransformDirection(new Vector3(0, 0, zoomLevel)), smoothing);
        transform.position = smoothedPosition;

        Vector2 mouseVec = new Vector2(Input.GetAxis("Camera X"), Input.GetAxis("Camera Y"));

        currYRot = Mathf.Clamp(currYRot + (-mouseVec.y * vertSensitivity), minYRot, maxYRot);
        currXRot = currXRot + (mouseVec.x * horizSensitivity);

        transform.localRotation = Quaternion.identity;
        if (!lockedOn)
        {
            if (currXRot > 360)
                currXRot -= 360;
            else if (currXRot < 0)
                currXRot += 360;

            transform.rotation = Quaternion.Euler(0, currXRot, 0);
            cameraVertRot.transform.rotation = Quaternion.Euler(currYRot, currXRot, 0);
        }

        else if (lockedOn)
        {
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        }
        /*else
        {
            if (currXRot < -xRotationClamp)
                currXRot = -xRotationClamp;
            else if (currXRot > xRotationClamp)
                currXRot = xRotationClamp;

            transform.rotation += Quaternion.Euler(0, currXRot, 0);
        }*/

        //Need to pass both X and Y to vertRot else it will offset the Y rot based on parent's rot to keep an overall value of 0, preventing horizontal camera movement
    }

    public void clearLockon()
    {
        lockedOn = false;
    }

    public void setCameraZoom(float zoomLevel)
    {
        this.zoomLevel = zoomLevel;
    }
}
