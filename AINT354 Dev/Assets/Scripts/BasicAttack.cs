using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [Header("Parent's Hitbox")]
    public CharacterController parentsHitbox;
    [Header("Damage Tag")]
    public string damageTag = "Character";
    [Header("Damage Values")]
    public float healthDamage = 1.0f;
    public bool applyImpactMultiplier = false;
    public float impactMultiplierDamage = 0.5f;
    [Header("Stun Values")]
    public bool applyStun = true;
    public float stunDamage = 1.5f;
    [Header("Knockback Values")]
    public float impactMagnitude = 1.0f;
    public float impactHorizontalMultiplier = 1.0f;
    public float impactVerticalMultiplier = 0.5f;

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Entering trigger");
        if (col.tag == damageTag && col != parentsHitbox)
        {
            try
            {
                //Debug.Log("Applying stuffz");
                BasicControls target = col.gameObject.GetComponent<BasicControls>();

                target.addDamage(healthDamage);

                if (applyImpactMultiplier)
                    target.addImpactMultiplierDamage(impactMultiplierDamage);

                if (applyStun)
                    target.addStun(stunDamage);

                //impact will always be added, for light attacks not designed to knockback, the multipliers will be very low
                target.addImpact(new Vector3((col.transform.position.x - transform.position.x) * impactHorizontalMultiplier, 1.0f * impactVerticalMultiplier, (col.transform.position.z - transform.position.z) * impactHorizontalMultiplier), impactMagnitude, transform.position);
                //Debug.Log("Applied stuffz");
            }
            catch { }
        }
    }
}
