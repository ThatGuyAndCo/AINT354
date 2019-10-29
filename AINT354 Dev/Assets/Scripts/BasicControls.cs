using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicControls: MonoBehaviour
{
    [Header("Player Settings")]
    public bool sandbag = false;
    public int playerNumber = 0;

    [Header("Movement Variables")]
    public Transform rotationAnchor;
    public float rotationSmoothing = 0.5f;
    public float initSpeed = 3.0f;
    public float gravity = 1.0f;
    public float horizSensitivity = 2.0f;
    public float initJumpSpeed = 1.0f;
    public float secondJumpSpeed = 1.0f;
    public float jumpSlowMultiplier = 1.0f;
    public float sprintSpeedMultiplier = 2.0f;

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

    [Header("Combat Variables")]
    public bool attacking = false;
    public int attackNum = 0;
    public bool attackAgain = false;
    public bool heavyAttack = false;
    public float initComboCooldown = 0.5f;
    public bool attackTriggered = false;
    public bool comboFinisher = false;
    public GameObject[] lightAttackHitboxes;
    public bool[] lightHitboxesToClear;
    public GameObject[] heavyAttackHitboxes;
    public bool[] heavyHitboxesToClear;
    public bool triggeredSandbag = false;
    public bool recoveryPhase = false;
    public Vector3 attackVelocity = Vector3.forward;
    public float attackRotClamp = 80.0f;

    [Header("Stun and Impact Variables")]
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
    private bool triggeredImpact = false;
    private Vector3 impactOrigin = Vector3.zero;


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
        Vector3 totalVeloc;

        //Allows bypassing of sandbag for certain inputs, i.e. recovery dodge and recovery attack
        //Timing for this is set by method call in animation
        if(playerNumber != 0)
        {
            if(knocked && sandbag && recoveryPhase)
            {
                //check for dash input
                //resetSandbag();
            }

            //Check for triggerStun/KnockbackOnCommand inputs
            if (Input.GetKeyDown(KeyCode.J))
                triggerStunOnCommand();
            else if (Input.GetKeyDown(KeyCode.K))
                triggerKnockbackOnCommand();
        }

        //Handle all code that checks for inputs in following block
        //This allows the creation of a sandbag character and allows Stun and Knockback to disable the player for a short time
        if (!sandbag)
        {
            //float mouseDir = Input.GetAxis("Camera X");
            Vector3 clampedInput = rotationAnchor.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            clampedInput = Vector3.ClampMagnitude(clampedInput, 1.0f);

            ///////////////Sprinting/////////////////
            if (!sprinting && Input.GetButtonDown("Sprint") && playerCont.isGrounded && !attacking)
            {
                sprinting = true;
            }
            else if (sprinting && Input.GetButtonDown("Sprint"))
            {
                sprinting = false;
            }

            if (sprinting && moveSpeed < initSpeed * sprintSpeedMultiplier) //Transition to sprint
            {
                moveSpeed += sprintSpeedMultiplier * Time.deltaTime * 5;
            }
            else if (sprinting && moveSpeed > initSpeed * sprintSpeedMultiplier) //If goes over sprint speed, reset to sprint speed
            {
                moveSpeed = initSpeed * sprintSpeedMultiplier;
            }
            else if (!sprinting && moveSpeed > initSpeed) //Transition from sprint to walk
            {
                moveSpeed -= sprintSpeedMultiplier * Time.deltaTime * 5;
            }
            else if (!sprinting && !attacking && playerCont.isGrounded && moveSpeed < initSpeed) //Update check when add combat
            {
                moveSpeed = initSpeed;
            }

            ///////////////Jumping/////////////////
            if (Input.GetButtonDown("Jump") && timesJumped == 0 && !attacking)
            {
                originalJumpVeloc = moveVeloc;
                timesJumped++;
                jumpPending = true;
                anim.SetInteger("JumpNumber", timesJumped);
            }
            else if (Input.GetButtonDown("Jump") && timesJumped == 1 && !jumpPending)
            {
                originalJumpVeloc += (clampedInput * moveSpeed * jumpSlowMultiplier) * 0.5f;
                timesJumped++;
                anim.SetInteger("JumpNumber", timesJumped);
            }

            ///////////////Attacking Movement/////////////////
            if (attacking)
            {
                moveVeloc = attackVelocity;
            }
            else if (playerCont.isGrounded)
            {
                moveVeloc = clampedInput * moveSpeed;
            }
            else
            {
                moveVeloc = (originalJumpVeloc * 0.75f) + (clampedInput * moveSpeed * jumpSlowMultiplier);
            }

            ///////////////Rotate Camera/////////////////
            if (!attacking)
            {
                transform.LookAt(transform.position + (clampedInput * moveSpeed));
            }

            ///////////////Input-based Sprint Reset/////////////////
            if (clampedInput.magnitude < 0.25f)
            {
                sprinting = false;
            }

            ///////////////Check Attack and Queue or Call Next Attack/////////////////
            // Attack Triggered is handled by methods called by the animations to properly set timing
            if (playerCont.isGrounded && !sprinting && !jumpPending)
            {
                if (Input.GetButtonDown("Fire1") && !attacking)
                {
                    attacking = true;
                    attackNum++;
                    attackVelocity = transform.TransformDirection(Vector3.forward + clampedInput * moveSpeed * 0.15f);
                    transform.LookAt(transform.position + (clampedInput * moveSpeed));
                    anim.SetBool("IsAttacking", true);
                    anim.SetInteger("AttackNumber", attackNum);
                    heavyAttack = false;
                    anim.SetBool("HeavyAttack", heavyAttack);
                    comboFinisher = false;
                    Debug.Log("Triggering attack, setting: \n   Attacking = true, AttackNumber = " + attackNum + "");
                }
                else if (Input.GetButtonDown("Fire1") && attacking && !attackAgain && !comboFinisher)
                {
                    Debug.Log("Setting attackAgain = true");
                    CancelInvoke("finishAttack");
                    attackAgain = true;
                    heavyAttack = false;
                    attackVelocity = transform.TransformDirection(Vector3.forward + clampedInput * moveSpeed * 0.15f);
                    transform.LookAt(transform.position + (clampedInput * moveSpeed));
                    if (attackNum == 3) //If add to light attack combo, update this (value == attack before final attack of combo)
                    {
                        comboFinisher = true;
                    }
                    if (attackTriggered)
                        nextAction();
                }
                else if (attacking && attackAgain && attackTriggered)
                {
                    nextAction();
                }
            }
        }

        ///////////////Calculate Stun/////////////////
        bool stunned = false;
        if (stun > stunThreshold)
        {
            //If not already a Sandbag, disable player inputs
            if (playerNumber != 0 && !triggeredSandbag)
            {
                applySandbag();
                triggeredSandbag = true;
            }
            stun = 0.0f;
            stunned = true;
            moveVeloc = Vector3.zero;
        }

        ///////////////Calculate Knockback and Gravity/////////////////
        if (impact.magnitude > impactThreshold)
        {
            knocked = true;
        }

        if (jumpVeloc > (-1 * gravity))
        {
            jumpVeloc -= gravity * Time.deltaTime;
        }

        totalVeloc = jumpVeloc * Vector3.up;

        if (knocked == true)
        {
            //If not already a Sandbag, disable player inputs
            if (playerNumber != 0 && !triggeredSandbag)
            {
                applySandbag();
                triggeredSandbag = true;
            }
            totalVeloc += impact;
            impact = Vector3.Lerp(impact, Vector3.zero, (impactFadeTime) * Time.deltaTime);
        }
        else
        {
            impact = Vector3.Lerp(impact, Vector3.zero, (impactFadeTime * 0.1f) * Time.deltaTime);
        }

        ///////////////Apply Stun and Knockback Rotation and Calc Movement/////////////////
        
        if (stun > 0)
            stun -= stunFadeTime * Time.deltaTime;
        else if (stun < 0)
            stun = 0;

        if (stunned || knocked)
        {
            if (!triggeredImpact)
            {
                transform.LookAt(new Vector3(impactOrigin.x, transform.position.y, impactOrigin.z));
                triggeredImpact = true;

                if (stunned && !knocked)
                {
                    anim.SetTrigger("Stun");
                    anim.SetFloat("RandomiseStun", Random.Range(0.0f, 1.0f));
                }
                else
                {
                    anim.SetTrigger("Knockback");
                }
            }
        }
        else
        {
            triggeredImpact = false;
            totalVeloc += moveVeloc;
        }

        ///////////////Apply Movement/////////////////
        
        playerCont.Move(totalVeloc * Time.deltaTime);
        
        anim.SetFloat("MovementSpeed", moveVeloc.magnitude / (initSpeed * sprintSpeedMultiplier));
        anim.SetBool("IsGrounded", playerCont.isGrounded);
        

        ///////////////Reset Jump and Sprinting/////////////////
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
    }


    /////////////// Jump Code /////////////////

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

    /////////////// Attack Code /////////////////


    //Can only use 1 param in animation method call, so need 2 methods for attack types
    //Uses attack number which is non-indexed, so animAttackNum - 1 for index of hitbox

    void triggerLightAttack(int animAttackNum)
    {
        Debug.Log("Calling triggerLightAttack");

        lightAttackHitboxes[animAttackNum - 1].SetActive(true);
        lightHitboxesToClear[animAttackNum - 1] = true;
        attackTriggered = true;

        Debug.Log("Invoking clearHitboxes");
        Invoke("clearHitboxes", 0.25f);
    }

    void triggerHeavyAttack(int animAttackNum)
    {
        Debug.Log("Calling triggerLightAttack");

        heavyAttackHitboxes[animAttackNum - 1].SetActive(true);
        heavyHitboxesToClear[animAttackNum - 1] = true;
        attackTriggered = true;

        Debug.Log("Invoking clearHitboxes");
        Invoke("clearHitboxes", 0.25f);
    }

    //As animations can be interrupted, need to invoke a clearing method after a short period of time
    //The challenge is that we cannot clear all hitboxes as that might clear a hitbox that was just opened by
    //the next attack, and we cannot pass variables in an invoke. Any variables set on a class level could be overwritten.
    //As such, I need an boolean array for each attack type and an index for each hitbox, to be set to true in a method called
    //by the animation. This in turn will invoke a 'Clear' method that will disable any hitboxes who's index is true.
    void clearHitboxes() 
    {
        for(int i = 0; i < lightHitboxesToClear.Length; i++)
        {
            if(lightHitboxesToClear[i] == true)
            {
                lightAttackHitboxes[i].SetActive(false);
                lightHitboxesToClear[i] = false;
            }
        }

        for (int i = 0; i < heavyHitboxesToClear.Length; i++)
        {
            if (heavyHitboxesToClear[i] == true)
            {
                heavyAttackHitboxes[i].SetActive(false);
                heavyHitboxesToClear[i] = false;
            }
        }
    }

    void nextAction()
    {
        Debug.Log("Calling nextAction");
        if (attackAgain)
        {
            attackNum++;
            if (attackNum < 5)
            {
                anim.SetBool("IsAttacking", true);
                anim.SetInteger("AttackNumber", attackNum);
                anim.SetBool("HeavyAttack", heavyAttack);
                attackAgain = false;
                anim.SetBool("Finisher", comboFinisher);
            }
            else
            {
                Debug.Log("End of combo, invoking finishAttack in " + initComboCooldown + " seconds");
                Invoke("finishAttack", initComboCooldown);
            }
        }
        else
        {
            Debug.Log("AttackAgain = false, invoking finishAttack in " + initComboCooldown + " seconds");
            Invoke("finishAttack", initComboCooldown);
        }
    }

    void invokeFinishAttack()
    {
        Invoke("finishAttack", initComboCooldown);
    }

    void nextAttack()
    {
        attackTriggered = false;
        CancelInvoke("finishAttack");
    }

    void finishAttack()
    {
        Debug.Log("Calling finishAttack");
        anim.SetBool("IsAttacking", false);
        attacking = false;
        attackAgain = false;
        attackTriggered = false;
        attackNum = 0;
        anim.SetInteger("AttackNumber", attackNum);
    }

    /////////////// Impact Code /////////////////

    public void addImpact(Vector3 direction, float force, Vector3 impactOriginVec)
    {
        direction.Normalize();
        if (direction.y < 0)
        {
            direction.y = -direction.y;
        }
        impact += (direction.normalized * force / mass) * impactMultiplier;
        impactOrigin = impactOriginVec;
    }

    public void addStun(float force)
    {
        stun += force;
    }

    public void addDamage(float damage)
    {
        health -= damage;
    }

    public void addImpactMultiplierDamage(float damage)
    {
        impactMultiplier += damage;
    }

    /////////////// Impact Code /////////////////
    
    void applySandbag()
    {
        sandbag = true;
        finishAttack();
        sprinting = false;
        jumpPending = false;
        sprintBeforeJump = false;
        
    }

    void triggerRecoveryPhase()
    {
        recoveryPhase = true;
    }

    void resetSandbag()
    {
        Debug.Log("Called resetSandbag");
        if(playerNumber != 0)
            sandbag = false;
        triggeredSandbag = false;
        knocked = false;
        recoveryPhase = false;
    }
    
    void triggerStunOnCommand()
    {
        addStun(100.0f);
        addImpact(transform.TransformDirection(-Vector3.forward), 1.0f, transform.position + transform.TransformDirection(Vector3.forward));
    }

    void triggerKnockbackOnCommand()
    {
        addImpact(transform.TransformDirection(-Vector3.forward), 25.0f, transform.position + transform.TransformDirection(Vector3.forward));
    }
}
