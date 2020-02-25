using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class CustomEventHandler : MonoBehaviour
{
    public string handlerTag = "";
    
    public bool callMethod(string methodName, string componentName, object[] optionalParams)
    {
        Debug.Log(GetType());
        Debug.Log(gameObject.GetComponent(componentName));
        Debug.Log(gameObject.GetComponent(componentName).GetType());
        Debug.Log(gameObject.GetComponent(componentName).GetType().GetMethod(methodName));
        MethodInfo myMethod = gameObject.GetComponent(componentName).GetType().GetMethod(methodName);
        if(myMethod == null)
        {
            Debug.Log("Method " + methodName + " not found on Event Handler for tag " + handlerTag);
            return false;
        }
        try
        {
            return (bool)myMethod.Invoke(this, optionalParams);
        }
        catch (Exception e)
        {
            Debug.Log("Error occured processing method " + methodName + " on handler for tag " + handlerTag + ". Please ensure parameters have been set where required. Error is as follows:");
            Debug.Log(e);
            return false;
        }
    }
}
