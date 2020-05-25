using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator KnockBack(Vector3 dir, float force)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(dir * force, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        dir = Vector3.zero;
    }
}
