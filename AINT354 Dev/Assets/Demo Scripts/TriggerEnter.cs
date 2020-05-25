using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnter : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            Vector3 dir = col.transform.position - transform.position;
            StartCoroutine(col.gameObject.GetComponent<PlayerHealth>().KnockBack(dir, 0.5f));
        }
    }
}
