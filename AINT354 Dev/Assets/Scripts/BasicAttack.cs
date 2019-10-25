using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Entering trigger");
        try
        {
            Debug.Log("Applying stuffz");
            BasicControls target = col.gameObject.GetComponent<BasicControls>();
            target.addImpact(new Vector3(col.transform.position.x - transform.position.x, 1, col.transform.position.z - transform.position.z), 5.0f);
            target.addStun(1.5f);
            target.addDamage(10.0f);
            Debug.Log("Applied stuffz");
        }
        catch{}
    }
}
