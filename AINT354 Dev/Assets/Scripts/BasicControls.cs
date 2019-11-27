using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicControls : MonoBehaviour
{
    [Header("Player Settings")]
    public bool sandbag = false;
    public bool invincible = false;
    public int playerNumber = 0;

    public float respawnDelay = 3.0f;
    public float invincibilityCooldown = 5.0f;
    public bool dead = false;

    [Header("Main Camera Script")]
    public BasicCamera mainCameraScript;

    [Header("Movement Variables")]
    public Transform rotationAnchor;
    public float rotationSmoothing = 0.5f;
    public float attackRotationMultiplier = 2.0f;
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
    public bool sprinting = false;
    private bool sprintBeforeJump = false;
    private CharacterController playerCont;
    private Vector3 moveVeloc = Vector3.zero;
    private Vector3 originalJumpVeloc = Vector3.zero;
    private Animator anim;
    private bool jumpPending = false;

    [Header("Dashing Variables")]
    public float dashSpeed = 9.0f;
    public bool dashable = true;
    public bool dashing = false;
    public bool canDashAttack = false;
    public bool isDashAttacking = false;
    public float dashDelay = 0.5f;
    public bool dashCooldown = false;
    public Vector3 dashVelocity = Vector3.zero;

    [Header("Combat Variables")]
    public bool attacking = false;
    public int attackNum = 0;
    public bool attackAgain = false;
    public bool heavyAttack = false;
    public float initComboCooldown = 0.5f;
    public bool attackTriggered = false;
    public bool comboFinisher = false;
    public bool attackQueued = false;
    public GameObject[] lightAttackHitboxes;
    public bool[] lightHitboxesToClear;
    public GameObject[] heavyAttackHitboxes;
    public bool[] heavyHitboxesToClear;
    public GameObject[] sprintAttackHitboxes;
    public bool[] sprintHitboxesToClear;
    public bool triggeredSandbag = false;
    public bool recoveryPhase = false;
    public bool attackMovementActive = false;
    public float attackMovementSpeed = 1.0f;
    public float dashAttackSpeed = 2.0f;
    public Vector3 attackVelocity = Vector3.zero; //This is only used for calculating rotations during attacks
    public Vector4 pushDirection = Vector4.zero;

    [Header("Stun and Impact Variables")]
    public float stun = 0.0f;
    public float maxHealth = 100.0f;
    public float health = 100.0f;
    public float baseImpactMultiplier = 1.0f;
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
    void Update()
    {
        Vector3 totalVeloc;

        //Allows bypassing of sandbag for certain inputs, i.e. recovery dodge and recovery attack
        //Timing for this is set by method call in animation
        if (playerNumber != 0 && !dead)
        {
            if (dashable)
            {
                if (Input.GetButtonDown("Dash"))
                {
                    //Dashing overrides any other actions this frame, allows cancelling attacks when certain percent through animation
                    finishAttack();
                    anim.SetTrigger("Dash");
                    Vector3 clampedInput = rotationAnchor.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
                    clampedInput = Vector3.ClampMagnitude(clampedInput, 1.0f);
                    dashVelocity = transform.position + clampedInput;
                    //Debug.Log("Dash Velocity" + dashVelocity);
                    transform.LookAt(dashVelocity);

                    dashable = false;
                    dashing = true;
                    attackMovementActive = false;
                    attacking = false;
                    sprinting = false;
                    knocked = false;
                    recoveryPhase = false;
                }
            }

            //Check for triggerStun/KnockbackOnCommand inputs
            if (Input.GetKeyDown(KeyCode.J))
                triggerStunOnCommand();
            else if (Input.GetKeyDown(KeyCode.K))
                triggerKnockbackOnCommand();
        }

        //Handle all code that checks for inputs in following block
        //This allows the creation of a sandbag character and allows Stun and Knockback to disable the player for a short time
        if (!sandbag && !dashing && !dead)
        {
            //float mouseDir = Input.GetAxis("Camera X");
            Vector3 clampedInput = rotationAnchor.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            clampedInput = Vector3.ClampMagnitude(clampedInput, 1.0f);
            
            //Rotate towards locked-on target if no input registered, overwrite the normal rotation of an attack direction
            if (clampedInput.magnitude < 0.01f && mainCameraScript.lockedOn)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((new Vector3(rotationAnchor.position.x, transform.position.y, rotationAnchor.position.z) + (rotationAnchor.TransformDirection(Vector3.forward) * moveSpeed)) - transform.position), Time.deltaTime * rotationSmoothing);

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
            /*else if (Input.GetButtonDown("Jump") && timesJumped == 1 && !jumpPending)
            {
                originalJumpVeloc = moveVeloc + (clampedInput * moveSpeed * jumpSlowMultiplier * 0.5f);
                timesJumped++;
                anim.SetInteger("JumpNumber", timesJumped);
            }*/

            ///////////////Grounded or Jumping Movement/////////////////
            if (playerCont.isGrounded)
            {
                moveVeloc = clampedInput * moveSpeed;
            }
            else
            {
                moveVeloc = Vector3.ClampMagnitude((originalJumpVeloc * 0.75f) + (clampedInput * moveSpeed * jumpSlowMultiplier), originalJumpVeloc.magnitude);
            }

            ///////////////Rotate Camera/////////////////
            if (attacking)
            {
                //transform.LookAt(attackVelocity); //Not smoothing rotation during attacking for extra player control
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(attackVelocity), Time.deltaTime * rotationSmoothing * attackRotationMultiplier);
            }
            else if(dashing)
            {
                transform.LookAt(dashVelocity); //Not smoothing rotation during attacking for extra player control
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dashVelocity), Time.deltaTime * rotationSmoothing );
            }
            else
            {
                //transform.LookAt(transform.position + (clampedInput * moveSpeed));
                if (clampedInput != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((new Vector3(rotationAnchor.position.x, transform.position.y, rotationAnchor.position.z) + (clampedInput * moveSpeed)) - transform.position), Time.deltaTime * rotationSmoothing);
                }
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
                if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && !attacking)
                {
                    attacking = true;
                    attackNum++;
                    if (clampedInput.magnitude < 0.01f && mainCameraScript.lockedOn)
                        attackVelocity = rotationAnchor.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                    else
                        attackVelocity = transform.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                    //transform.LookAt(transform.position + (clampedInput * moveSpeed));
                    anim.SetBool("IsAttacking", true);
                    anim.SetInteger("AttackNumber", attackNum);
                    dashable = true;
                    if (Input.GetButtonDown("Fire1"))
                    {
                        heavyAttack = false;
                    }
                    else
                    {
                        heavyAttack = true;
                    }
                    anim.SetBool("HeavyAttack", heavyAttack);
                    comboFinisher = false;
                    //Debug.Log("Triggering attack, setting: \n   Attacking = true, AttackNumber = " + attackNum + "");
                }
                else if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && attacking && !attackAgain && !comboFinisher)
                {
                    //Debug.Log("Setting attackAgain = true");
                    CancelInvoke("finishAttack");
                    attackAgain = true;
                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (heavyAttack)
                            comboFinisher = true;
                        heavyAttack = false;
                    }
                    else
                    {
                        if (!heavyAttack)
                            comboFinisher = true;
                        heavyAttack = true;
                    }

                    if ((!heavyAttack && attackNum == 3) || (heavyAttack && attackNum == 2)) //If add to light attack combo, update this (value == attack before final attack of combo)
                    {
                        comboFinisher = true;
                    }

                    attackQueued = true;

                    if (attackTriggered)
                    {
                        if (clampedInput.magnitude < 0.01f && mainCameraScript.lockedOn)
                            attackVelocity = rotationAnchor.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                        else
                            attackVelocity = transform.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                        Debug.Log("Attack Number = " + attackNum + ", attack veloc = " + attackVelocity);
                        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(attackVelocity - transform.position), Time.deltaTime * rotationSmoothing * attackRotationMultiplier);
                        nextAction();
                    }
                }
                else if (attacking && attackAgain && attackTriggered && attackQueued)
                {
                    attackQueued = false;
                    if (clampedInput.magnitude < 0.01f && mainCameraScript.lockedOn)
                        attackVelocity = rotationAnchor.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                    else
                        attackVelocity = transform.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                    Debug.Log("Attack Number = " + attackNum + ", attack veloc = " + attackVelocity);
                    //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(attackVelocity - transform.position), Time.deltaTime * rotationSmoothing * attackRotationMultiplier);
                    nextAction();
                }
            }
            else if (sprinting && (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")))
            {
                //Sprint attacks need to use the same framework as dash attacks due to motion in the animation 
                //and getting rid of it by looping pose, which causes the animation to loop unless the state returns to idle
                //But without the dash attack delay mechanic, there is a very sudden lurch when the transition happens
                attacking = true;
                attackNum++;
                if (clampedInput.magnitude < 0.01f && mainCameraScript.lockedOn)
                    attackVelocity = rotationAnchor.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                else
                    attackVelocity = transform.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.15f;
                anim.SetBool("IsAttacking", true);
                anim.SetInteger("AttackNumber", attackNum);
                anim.SetBool("IsSprinting", true);
                isDashAttacking = true;
                //Debug.Log("Triggering sprint attack, setting: \n   Attacking = true, AttackNumber = " + attackNum + "");
            }

        }
        else if (dashing && canDashAttack && !dashCooldown && (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")))
        {
            attacking = true;
            isDashAttacking = true;
            canDashAttack = false;
            attackNum++;
            attackVelocity = transform.TransformDirection(Vector3.forward) * moveSpeed * 0.15f;
            anim.SetTrigger("DashAttack");
            anim.SetBool("IsAttacking", true);
            //Debug.Log("Triggering dash attack, setting: \n   Attacking = true, AttackNumber = " + attackNum + "");
            activateAttackMovement(dashAttackSpeed);
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

        //Cumulate gravity, but reset if the player isnt airborne
        if (jumpVeloc > (-1 * gravity))
        {
            jumpVeloc -= gravity * Time.deltaTime;
        }

        if (playerCont.isGrounded)
        {
            jumpVeloc = -0.1f;
        }

        //Moving forward with a Y velocity of 0 can cause the character controller to raise ever so slightly, and isGrounded to become false
        //But to allow jumping attacks, we don't want gravity, so setting a very small negative Y value in the else fixes the issue
        if (!attacking)
            totalVeloc = jumpVeloc * Vector3.up;
        else
            totalVeloc = new Vector3(0, -0.1f, 0);

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
        else if (!attacking && !dashing)
        {
            triggeredImpact = false;
            totalVeloc += moveVeloc;
        }
        else
        {
            triggeredImpact = false;
        }

        ///////////////Apply Movement/////////////////

        if (!attacking || attackMovementActive)
        {
            if (attackMovementActive)
            {
                totalVeloc += transform.TransformDirection(Vector3.forward);
                totalVeloc *= attackMovementSpeed;
            } else if (dashing && !dashCooldown)
            {
                totalVeloc += transform.TransformDirection(Vector3.forward);
                totalVeloc *= dashSpeed;
            } else if (dashCooldown)
            {
                totalVeloc = new Vector3(0, -1, 0);
                anim.SetFloat("MovementSpeed", 0.0f);
            }

            if (pushDirection.magnitude > 0)
            {
                totalVeloc += new Vector3(pushDirection.x, pushDirection.y, pushDirection.z) * pushDirection.w;
                pushDirection = Vector4.Lerp(pushDirection, Vector4.zero, Time.deltaTime * 15);
            }

            playerCont.Move(totalVeloc * Time.deltaTime);

            if (!dashCooldown)
            {
                anim.SetFloat("MovementSpeed", moveVeloc.magnitude / (initSpeed * sprintSpeedMultiplier));
            }
            anim.SetBool("IsGrounded", playerCont.isGrounded);
        }


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
            mainCameraScript.setCameraZoom(0.2f);
        }
        /*else if (timesJumped == 2)
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
            mainCameraScript.setCameraZoom(1.5f);
        }*/
    }

    void resetControllerHeight()
    {
        playerCont.slopeLimit = 110;
        playerCont.center = new Vector3(0, 0.85f, 0);
        mainCameraScript.setCameraZoom(0.0f);
    }

    /////////////// Attack Code /////////////////


    //Can only use 1 param in animation method call, so need 2 methods for attack types
    //Uses attack number which is non-indexed, so animAttackNum - 1 for index of hitbox

    void triggerLightAttack(int animAttackNum)
    {
        lightAttackHitboxes[animAttackNum - 1].SetActive(true);
        lightHitboxesToClear[animAttackNum - 1] = true;
        attackTriggered = true;
        
        Invoke("clearHitboxes", 0.25f);
    }

    void triggerHeavyAttack(int animAttackNum)
    {
        heavyAttackHitboxes[animAttackNum - 1].SetActive(true);
        heavyHitboxesToClear[animAttackNum - 1] = true;
        attackTriggered = true;
        
        Invoke("clearHitboxes", 0.25f);
    }

    void triggerSprintAttack(int animAttackNum)
    { 
        sprintAttackHitboxes[animAttackNum - 1].SetActive(true);
        sprintHitboxesToClear[animAttackNum - 1] = true;
        attackTriggered = true;
        
        Invoke("clearHitboxes", 0.25f);
    }

    //As animations can be interrupted, need to invoke a clearing method after a short period of time
    //The challenge is that we cannot clear all hitboxes as that might clear a hitbox that was just opened by
    //the next attack, and we cannot pass variables in an invoke. Any variables set on a class level could be overwritten.
    //As such, I need an boolean array for each attack type and an index for each hitbox, to be set to true in a method called
    //by the animation. This in turn will invoke a 'Clear' method that will disable any hitboxes who's index is true.
    void clearHitboxes()
    {
        dashable = true;
        for (int i = 0; i < lightHitboxesToClear.Length; i++)
        {
            if (lightHitboxesToClear[i] == true)
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

        for (int i = 0; i < sprintHitboxesToClear.Length; i++)
        {
            if (sprintHitboxesToClear[i] == true)
            {
                sprintAttackHitboxes[i].SetActive(false);
                sprintHitboxesToClear[i] = false;
            }
        }
    }

    //Called by animation to manually set amount of time and speed of movement for each animation
    void activateAttackMovement(float speed)
    {
        attackMovementActive = true;
        attackMovementSpeed = speed;
    }

    void clearAttackMovement()
    {
        attackMovementActive = false;
    }

    void nextAction()
    {
        //Debug.Log("Calling nextAction");
        if (attackAgain)
        {
            attackNum++;
            if (attackNum < 5)
            {
                attackTriggered = false;
                dashable = false;
                anim.SetBool("IsAttacking", true);
                anim.SetInteger("AttackNumber", attackNum);
                anim.SetBool("HeavyAttack", heavyAttack);
                anim.SetBool("Finisher", comboFinisher);
            }
            else
            {
                //Debug.Log("End of combo, invoking finishAttack in " + initComboCooldown + " seconds");
                Invoke("finishAttack", initComboCooldown);
            }
        }
        else
        {
            //Debug.Log("AttackAgain = false, invoking finishAttack in " + initComboCooldown + " seconds");
            Invoke("finishAttack", initComboCooldown);
        }
    }

    //Allows overwriting for certain attacks, whilst keeping default cooldown for others
    void invokeFinishAttack(float delay)
    {
        if(delay == 0)
        {
            delay = initComboCooldown;
        }
        if (attackTriggered)
        {
            //Debug.Log("Invoking finish attack");
            Invoke("finishAttack", delay);
        }
    }

    void nextAttack()
    {
        attackTriggered = false;
        attackAgain = false;
        CancelInvoke("finishAttack");
    }

    void finishAttack()
    {
        //Debug.Log("Calling finishAttack");
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsSprinting", false);
        attacking = false;
        attackAgain = false;
        attackTriggered = false;
        isDashAttacking = false;
        dashing = false;
        dashable = true;
        attackNum = 0;
        anim.SetInteger("AttackNumber", attackNum);
    }


    /////////////// Dash Attack Code /////////////////

    public void enableDashAttack()
    {
        canDashAttack = true;
    }

    public void disableDashAttack()
    {
        canDashAttack = false;
    }

    public void invokeDisableDash()
    {
        if (!isDashAttacking)
        {
            Invoke("disableDash", dashDelay);
            dashCooldown = true;
        }
    }

    public void disableDash()
    {
        dashing = false;
        dashable = true;
        dashCooldown = false;
    }

    /////////////// Impact Code /////////////////

    public void addImpact(Vector3 direction, float force, Vector3 impactOriginVec)
    {
        if (!dashing && !invincible)
        {
            direction.Normalize();
            if (direction.y < 0)
            {
                direction.y = -direction.y;
            }
            impact += (direction.normalized * force / mass) * impactMultiplier;
            impactOrigin = impactOriginVec;
            
        }
    }

    public void addStun(float force)
    {
        if (!dashing && !invincible)
        {
            stun += force;
        }
    }

    public void addDamage(float damage)
    {
        if (!dashing && !invincible)
        {        
            health -= damage;
            if(health <= 0)
            {
                health = 0;
                die("damage");
            }
        }
    }

    public void addImpactMultiplierDamage(float damage)
    {
        if (!dashing && !invincible)
        {
            impactMultiplier += damage;
        }
    }

    public void addPushback(Vector3 impact, float force)
    {
        if (!(impact.magnitude > impactThreshold) && !invincible)
        {
            pushDirection = new Vector4(impact.x, transform.position.y, impact.z, force);
        }
    }

    /////////////// Sandbag Code /////////////////

    void applySandbag()
    {
        sandbag = true;
        finishAttack();
        sprinting = false;
        jumpPending = false;
        sprintBeforeJump = false;
        dashable = false;
        if(knocked)
            playerCont.center = new Vector3(0, 0.85f, -1.48f);
    }

    void triggerRecoveryPhase()
    {
        recoveryPhase = true;
        dashable = true;
    }

    void resetSandbag()
    {
        if(playerNumber != 0)
            sandbag = false;
        triggeredSandbag = false;
        if (knocked)
            playerCont.center = new Vector3(0, 0.85f, 0f);
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

    /////////////// Death Code /////////////////

        //Add flashing effect to materials to show player is invincible, or a shield around them

    public void die(string cause)
    {
        if (!invincible || cause == "killzone")
        {
            //Respawn in middle of arena
            //Set invincible
            applySandbag(); //May as well reuse code
            invincible = true;
            dead = true;
            mainCameraScript.clearLockon();
            anim.SetBool("Dead", true);
            anim.SetTrigger("DeathTrigger");
            //Show death message based on which player number died, play animation and particle effect
            Invoke("respawn", respawnDelay);
        }
    }

    void respawn()
    {
        moveVeloc = Vector3.zero;
        originalJumpVeloc = Vector3.zero;
        transform.position = new Vector3(0.52f, 0.5f, 2.099f);
        resetSandbag();
        anim.SetBool("Dead", false);
        health = maxHealth;
        impactMultiplier = baseImpactMultiplier;
        dead = false;
        Invoke("resetInvincible", invincibilityCooldown);
    }

    void resetInvincible()
    {
        invincible = false;
    }
}
