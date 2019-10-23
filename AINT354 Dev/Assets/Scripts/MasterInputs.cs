using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterInputs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            Cursor.visible = !Cursor.visible;
        }
    }
}
