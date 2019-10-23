using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicControls: MonoBehaviour
{
    public float initSpeed = 3.0f;
    public float gravity = 1.0f;
    public float horizSensitivity = 2.0f;
    public float initJumpSpeed = 1.0f;
    public float secondJumpSpeed = 1.0f;
    public float jumpSlowMultiplier = 1.0f;
    public float sprintSpeedMultiplier = 2.0f;

    private float jumpVeloc = 0.0f;
    public int timesJumped = 0;
    private float moveSpeed = 0.0f;
    private bool sprinting = false;
    private CharacterController playerCont;
    private Vector3 moveVeloc = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = initSpeed;
        playerCont = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    //Fix Sprint
    void Update()
    {
        Debug.Log("Sprinting: " + sprinting);
        Debug.Log("Move Speed: " + moveSpeed);
        Debug.Log("Target Speed: " + initSpeed * sprintSpeedMultiplier);
        float mouseDir = Input.GetAxis("Mouse X");


        if (sprinting == false && Input.GetButtonDown("Sprint") && playerCont.isGrounded)
        {
            sprinting = true;
            Debug.Log("Sprint pressed");
        } else if(sprinting == true && Input.GetButtonDown("Sprint"))
        {
            sprinting = false;
            Debug.Log("Stopping sprint");
        }

        if(sprinting && moveSpeed != initSpeed * sprintSpeedMultiplier)
        {
            moveSpeed += sprintSpeedMultiplier * Time.deltaTime;
            Debug.Log("Sprint speed increasing");
        }

        if (Input.GetButtonDown("Jump") && timesJumped == 0)
        {
            sprinting = false;
            jumpVeloc = initJumpSpeed;
            timesJumped++;
            moveSpeed = moveSpeed * jumpSlowMultiplier;
        } else if(Input.GetButtonDown("Jump") && timesJumped == 1)
        {
            jumpVeloc /= 2;
            jumpVeloc += secondJumpSpeed;
            timesJumped++;
            moveSpeed = moveSpeed * jumpSlowMultiplier;
        }

        if (jumpVeloc > (-1 * gravity))
        {
            jumpVeloc -= gravity * Time.deltaTime;
        }
        
        if (playerCont.isGrounded)
        {
            moveVeloc = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))) * moveSpeed;
        }

        Vector3 totalVeloc = moveVeloc + (jumpVeloc * Vector3.up); 

        playerCont.Move(totalVeloc * Time.deltaTime);
        transform.Rotate(Vector3.up, mouseDir * horizSensitivity);
        
        if(playerCont.isGrounded)
        {
            timesJumped = 0;
            moveSpeed = initSpeed;
        }     
    }
}
