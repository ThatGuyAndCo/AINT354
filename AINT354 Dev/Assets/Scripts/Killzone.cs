using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Exiting play-area");
        try
        {
            col.transform.gameObject.GetComponent<BasicControls>().die("killzone");
        }
        catch { }
    }
}
