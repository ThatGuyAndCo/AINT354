using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicControls : MonoBehaviour
{
    [Header("Player Settings")]
    public bool sandbag = false;
    public bool invincible = false;
    public int playerNumber = 0;
    public UIElements playerUI;

    public float respawnDelay = 3.0f;
    public float invincibilityCooldown = 5.0f;
    public bool dead = false;

    [Header("Main Camera Script")]
    public BasicCamera mainCameraScript;

    [Header("Movement Variables")]
    public Transform rotationAnchor;
    public Transform rotationTarget;
    public float rotationSmoothing = 0.5f;
    public float attackRotationMultiplier = 2.0f;
    public float initSpeed = 3.0f;
    public float gravity = 1.0f;
    public float horizSensitivity = 2.0f;
    public float initJumpSpeed = 1.0f;
    public float secondJumpSpeed = 1.0f;
    public float jumpSlowMultiplier = 1.0f;
    public float sprintSpeedMultiplier = 2.0f;

    public float jumpVeloc = 0.0f;
    private int timesJumped = 0;
    private float moveSpeed = 0.0f;
    public bool sprinting = false;
    private bool sprintBeforeJump = false;
    private CharacterController playerCont;
    private Vector3 moveVeloc = Vector3.zero;
    private Vector3 originalJumpVeloc = Vector3.zero;
    private Animator anim;
    private bool jumpPending = false;
    public bool jumpAttackGravityDisabled = false;

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
    public bool jumpAttack = false;
    public float initComboCooldown = 0.5f;
    public bool attackTriggered = false;
    public bool comboFinisher = false;
    public bool attackQueued = false;
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

    [Header("Hitboxes")]
    public GameObject[] lightAttackHitboxes;
    public bool[] lightHitboxesToClear;
    public GameObject[] heavyAttackHitboxes;
    public bool[] heavyHitboxesToClear;
    public GameObject[] chargedLightAttackHitboxes;
    public bool[] chargedLightHitboxesToClear;
    public GameObject[] chargedHeavyAttackHitboxes;
    public bool[] chargedHeavyHitboxesToClear;
    public GameObject[] sprintAttackHitboxes;
    public bool[] sprintHitboxesToClear;

    public Vector3 totalVeloc;


    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = initSpeed;
        playerCont = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        playerUI.initHPBar(maxHealth, health);
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 totalVeloc;

        if (respawnMove || (!sandbag && transform.position.y < -20) || transform.position.y < -500)
        {
            Debug.Log("**************Respawning");//Sometimes transform.position does not update the object, so recheck every frame until it does update the position
            transform.position = new Vector3(0.52f, 2.5f, 2.099f);
            respawnMove = false;
            return;
        }

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
            //Initially check if an attack is being charged. If it is and the button is no longer held, transition to normal attack, then carry on
            if (attacking && anim.GetBool("Charging"))
            {
                bool keepCharging = testChargeAttack(heavyAttack);
                anim.SetBool("Charging", keepCharging);
            }
            else if(attacking) 
            {
                anim.SetBool("Charging", false);
            }
            else//Ensure the charging variable is in its default state
            {
                anim.SetBool("Charging", true);
            }

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
                    Vector3 calcLookRot = (new Vector3(rotationTarget.position.x, transform.position.y, rotationTarget.position.z) + (clampedInput * moveSpeed)) - transform.position;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(calcLookRot), Time.deltaTime * rotationSmoothing);
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
                else if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && attacking && !attackAgain && !comboFinisher && !jumpAttack)
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
            else if (!playerCont.isGrounded && (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")))
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
                if (Input.GetButtonDown("Fire1"))
                    anim.SetBool("HeavyAttack", false);
                else
                    anim.SetBool("HeavyAttack", true);
                jumpAttack = true;
                anim.SetBool("IsAttacking", true);
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
            anim.SetBool("IsAttacking", true);
            anim.SetBool("DashAttack", true);
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

        //Cumulate gravity if airborne even when air heavy attacking, reset if the player isnt airborne or if air light attacking
        if ((playerCont.isGrounded && jumpVeloc < -0.3f) || jumpAttackGravityDisabled)
        {
            jumpVeloc = -0.3f;
        }
        else if (jumpVeloc > (-1 * gravity) || jumpAttackGravityDisabled == false)
        {
            jumpVeloc -= gravity * Time.deltaTime;
        }



        //Moving forward with a Y velocity of 0 can cause the character controller to raise ever so slightly, and isGrounded to become false
        //But to allow jumping attacks, we don't want gravity, so setting a very small negative Y value in the else fixes the issue
        if (!attacking || (attacking && !jumpAttackGravityDisabled))
            totalVeloc = jumpVeloc * Vector3.up;
        else if (attacking && jumpAttackGravityDisabled)
            totalVeloc = (jumpVeloc * Vector3.up) * 0.1f;
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
        else if ((!attacking || (attacking && !playerCont.isGrounded)) && !dashing)
        {
            triggeredImpact = false;
            totalVeloc += moveVeloc;
        }
        else
        {
            triggeredImpact = false;
        }

        ///////////////Apply Movement/////////////////

        if (!attacking || attackMovementActive || (!playerCont.isGrounded && !jumpAttackGravityDisabled))
        {
            if (attackMovementActive)
            {
                if (!playerCont.isGrounded)
                {
                    totalVeloc += transform.TransformDirection(Vector3.forward) * attackMovementSpeed * 0.2f; //Dont apply as much momentum to aerial light attacks
                }
                else {
                    totalVeloc += transform.TransformDirection(Vector3.forward);
                    totalVeloc *= attackMovementSpeed;
                }
            } else if (dashing && !dashCooldown)
            {
                totalVeloc += transform.TransformDirection(Vector3.forward) * dashSpeed;
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
            playerCont.slopeLimit = 75;
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
        playerCont.slopeLimit = 75;
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

        if(!playerCont.isGrounded)
            jumpAttackGravityDisabled = true;

        Invoke("clearHitboxes", 0.25f);
    }

    void triggerHeavyAttack(int animAttackNum)
    {
        heavyAttackHitboxes[animAttackNum - 1].SetActive(true);
        heavyHitboxesToClear[animAttackNum - 1] = true;
        attackTriggered = true;
        
        Invoke("clearHitboxes", 0.25f);
    }

    void triggerChargedLightAttack(int animAttackNum) //Can only pass 1 variable via animation event so need to split methods for normal and charged attack
    {
        chargedLightAttackHitboxes[animAttackNum - 1].SetActive(true);
        chargedLightHitboxesToClear[animAttackNum - 1] = true;
        attackTriggered = true;

        Invoke("clearHitboxes", 0.25f);
    }

    void triggerChargedHeavyAttack(int animAttackNum)
    {
        chargedHeavyAttackHitboxes[animAttackNum - 1].SetActive(true);
        chargedHeavyHitboxesToClear[animAttackNum - 1] = true;
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

    /////////////// Charge Attack Code /////////////////

    bool testChargeAttack(bool isHeavyAttack)
    {
        bool keepCharging = true;

        if (!isHeavyAttack)
            keepCharging = Input.GetButton("Fire1");
        else
            keepCharging = Input.GetButton("Fire2");

        return keepCharging;
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

        for (int i = 0; i < chargedLightHitboxesToClear.Length; i++)
        {
            if (chargedLightHitboxesToClear[i] == true)
            {
                chargedLightAttackHitboxes[i].SetActive(false);
                chargedLightHitboxesToClear[i] = false;
            }
        }

        for (int i = 0; i < chargedHeavyHitboxesToClear.Length; i++)
        {
            if (chargedHeavyHitboxesToClear[i] == true)
            {
                chargedHeavyAttackHitboxes[i].SetActive(false);
                chargedHeavyHitboxesToClear[i] = false;
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
        Debug.Log("Clear attack movement called");
        attackMovementActive = false;
        jumpAttackGravityDisabled = false;
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
                anim.SetBool("Charging", true);
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

    void finishJumpAttack()
    {
        if (attackTriggered)
        {
            clearAttackMovement();
            finishAttack();
            clearAttackMovement();
        }
    }

    void nextAttack()
    {
        attackTriggered = false;
        attackAgain = false;
        clearAttackMovement();
        CancelInvoke("finishAttack");
        adjustInput();
    }

    void adjustInput()
    {
        Vector3 clampedInput = rotationAnchor.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
        clampedInput = Vector3.ClampMagnitude(clampedInput, 1.0f);

        if (clampedInput.magnitude < 0.01f && mainCameraScript.lockedOn)
            attackVelocity = rotationAnchor.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.35f;
        else
            attackVelocity = transform.TransformDirection(Vector3.forward) + clampedInput * moveSpeed * 0.35f;
    }

    void finishAttack()
    {
        //Debug.Log("Calling finishAttack");
        anim.SetBool("IsAttacking", false);
        anim.SetBool("IsSprinting", false);
        anim.SetBool("Charging", true);
        jumpAttackGravityDisabled = false;
        attacking = false;
        attackAgain = false;
        jumpAttack = false;
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
            playerUI.updateHealth(maxHealth, health);
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

            if(cause == "killzone")
                health = 0;

            playerUI.updateHealth(maxHealth, health);
            mainCameraScript.clearLockon();
            anim.SetBool("Dead", true);
            anim.SetTrigger("DeathTrigger");
            //Show death message based on which player number died, play animation and particle effect
            Invoke("respawn", respawnDelay);
        }
    }

    private bool respawnMove = false; //Sometimes this method does not overwrite the player's current position, so moved to update loop
    void respawn()
    {
        moveVeloc = Vector3.zero;
        originalJumpVeloc = Vector3.zero;
        jumpVeloc = -0.1f;
        resetSandbag();
        anim.SetBool("Dead", false);
        health = maxHealth;
        playerUI.updateHealth(maxHealth, health);
        impactMultiplier = baseImpactMultiplier;
        dead = false;
        Invoke("resetInvincible", invincibilityCooldown);
        respawnMove = true;
    }

    void resetInvincible()
    {
        invincible = false;
    }
}
