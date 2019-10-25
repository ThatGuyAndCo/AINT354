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
    public GameObject hitbox;
    public bool sandbag = false;

    private float jumpVeloc = 0.0f;
    private int timesJumped = 0;
    private float moveSpeed = 0.0f;
    private bool sprinting = false;
    private bool sprintBeforeJump = false;
    private CharacterController playerCont;
    private Vector3 moveVeloc = Vector3.zero;
    private Vector3 originalJumpVeloc = Vector3.zero;
    private Animator anim;
    private bool jumpPending = false;
    private bool attacking = false;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = initSpeed;
        playerCont = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    //Fix Sprint
    void Update()
    {
        if (!sandbag)
        {
            float mouseDir = Input.GetAxis("Camera X");
            Vector3 clampedInput = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            clampedInput = Vector3.ClampMagnitude(clampedInput, 1.0f);

            if (!sprinting && Input.GetButtonDown("Sprint") && playerCont.isGrounded && !attacking)
            {
                sprinting = true;
            }
            else if (sprinting && Input.GetButtonDown("Sprint"))
            {
                sprinting = false;
            }

            if (sprinting && moveSpeed < initSpeed * sprintSpeedMultiplier)
            {
                moveSpeed += sprintSpeedMultiplier * Time.deltaTime * 5;
            }
            else if (sprinting && moveSpeed > initSpeed * sprintSpeedMultiplier)
            {
                moveSpeed = initSpeed * sprintSpeedMultiplier;
            }
            else if (!sprinting && moveSpeed > initSpeed)
            {
                moveSpeed -= sprintSpeedMultiplier * Time.deltaTime * 5;
            }
            else if (!sprinting && playerCont.isGrounded && moveSpeed < initSpeed) //Update check when add combat
            {
                moveSpeed = initSpeed;
            }

            if (Input.GetButtonDown("Jump") && timesJumped == 0 && !attacking)
            {
                originalJumpVeloc = moveVeloc;
                timesJumped++;
                jumpPending = true;
                anim.SetInteger("JumpNumber", timesJumped);
            }
            else if (Input.GetButtonDown("Jump") && timesJumped == 1 && !jumpPending)
            {
                timesJumped++;
                anim.SetInteger("JumpNumber", timesJumped);
            }

            if (jumpVeloc > (-1 * gravity))
            {
                jumpVeloc -= gravity * Time.deltaTime;
            }

            if (attacking)
            {
                moveVeloc = clampedInput * moveSpeed * 0.25f;
            }
            else if (playerCont.isGrounded)
            {
                moveVeloc = clampedInput * moveSpeed;
            }
            else
            {
                moveVeloc = (originalJumpVeloc * 0.5f) + (clampedInput * moveSpeed * jumpSlowMultiplier);
            }

            Vector3 totalVeloc = moveVeloc + (jumpVeloc * Vector3.up);

            playerCont.Move(totalVeloc * Time.deltaTime);
            transform.Rotate(Vector3.up, mouseDir * horizSensitivity);

            if (playerCont.isGrounded && timesJumped != 0 && !jumpPending)
            {
                timesJumped = 0;
                anim.SetInteger("JumpNumber", 0);
                moveSpeed = initSpeed;
                sprinting = sprintBeforeJump;
                sprintBeforeJump = false;
                playerCont.slopeLimit = 110;
                playerCont.center = new Vector3(0, 0.85f, 0);
            }

            if (clampedInput.magnitude < 0.25f)
            {
                sprinting = false;
            }

            anim.SetFloat("MovementSpeed", moveVeloc.magnitude / (initSpeed * sprintSpeedMultiplier));
            anim.SetBool("IsGrounded", playerCont.isGrounded);

            if (playerCont.isGrounded && !sprinting && !jumpPending)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    anim.SetBool("IsAttacking", true);
                    attacking = true;
                }
            }
        }
        else
        {
            if (impact.magnitude > impactThreshold)
            {
                knocked = true;
            }

            if (jumpVeloc > (-1 * gravity))
            {
                jumpVeloc -= gravity * Time.deltaTime;
            }

            Vector3 totalVeloc = jumpVeloc * Vector3.up;

            if (knocked == true)
            {
                //Disable player inputs
                //GetComponent<CharacterController>().Move(impact * Time.deltaTime);
                totalVeloc += impact;
                impact = Vector3.Lerp(impact, Vector3.zero, (impactFadeTime) * Time.deltaTime);
            }
            else
            {
                impact = Vector3.Lerp(impact, Vector3.zero, (impactFadeTime * 0.1f) * Time.deltaTime);
            }

            playerCont.Move(totalVeloc * Time.deltaTime);

            if (stun > stunThreshold)
            {
                //Stun the character
                stun = 0.0f;
                impactMultiplier += 1.0f;
            }
            stun = Mathf.Lerp(stun, 0.0f, stunFadeTime * Time.deltaTime);
        }
    }

    void triggerJump()
    {
        if (timesJumped == 1)
        {
            jumpPending = false;
            if (sprinting)
                sprintBeforeJump = true;
            sprinting = false;
            jumpVeloc = initJumpSpeed;
        }
        else if(timesJumped == 2)
        {
            if (jumpVeloc > 0)
            {
                jumpVeloc = 0;
            }
            else
            {
                jumpVeloc /= 2;
            }
            jumpVeloc += secondJumpSpeed;
            playerCont.slopeLimit = 250;
            playerCont.center = new Vector3(0, 1.2f, 0);
        }
    }

    void resetControllerHeight()
    {
        playerCont.slopeLimit = 110;
        playerCont.center = new Vector3(0, 0.85f, 0);
    }

    void triggerAttack()
    {
        hitbox.SetActive(true);
    }

    void finishAttack()
    {
        anim.SetBool("IsAttacking", false);
        attacking = false;
        hitbox.SetActive(false);
    }

    /////////////// Impact Code /////////////////

    public float stun = 0.0f;
    public float health = 100.0f;
    public float impactMultiplier = 1.0f;
    public float mass = 3.0f;

    private Vector3 impact = Vector3.zero;
    public float impactThreshold = 15.0f;
    public float impactFadeTime = 5.0f;
    public bool knocked = false;
    public float stunThreshold = 5.0f;
    public float stunFadeTime = 0.2f;

    public void addImpact(Vector3 direction, float force)
    {
        direction.Normalize();
        if (direction.y < 0)
        {
            direction.y = -direction.y;
        }
        impact += (direction.normalized * force / mass) * impactMultiplier;
    }

    public void addStun(float force)
    {
        stun += force;
    }

    public void addDamage(float damage)
    {
        health -= damage;
        impactMultiplier += damage * 0.1f;
    }
}
