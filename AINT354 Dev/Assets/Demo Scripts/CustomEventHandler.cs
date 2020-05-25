using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Jobs;
//using Unity.Collections;

public class CustomEventHandler : MonoBehaviour
{
    public string handlerTag = "";

    public bool callMethod(string methodName, string componentName, object[] optionalParams)
    {
        Debug.Log("Calling method " + methodName);
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
            return (bool)myMethod.Invoke(gameObject.GetComponent(componentName), optionalParams);
        }
        catch (Exception e)
        {
            Debug.Log("Error occured processing method " + methodName + " on handler for tag " + handlerTag + ". Please ensure parameters have been set where required (note that parameters with default values are not supported), and that the method being called is returning a boolean. Error is as follows:");
            Debug.Log(e);
            return false;
        }
    }

    /*public Array callJob(string jobName, string componentName, object[] baseDataArray, Type dataArrayType)
    {
        Debug.Log("Calling Job " + jobName);
        Debug.Log(GetType());
        Debug.Log(gameObject.GetComponent(componentName));
        Debug.Log(gameObject.GetComponent(componentName).GetType());
        Type compType = gameObject.GetComponent(componentName).GetType();
        Assembly ass = compType.Assembly;
        Type jobType = ass.GetType(jobName);
        Debug.Log(jobType);
        if (jobType == null)
        {
            Debug.Log("Job " + jobName + " not found on Event Handler for tag " + handlerTag);
            return new object[] { null };
        }
        try
        {
            dynamic instance = Activator.CreateInstance(jobType);
            MethodInfo myMethod = instance.GetType().GetMethod("setDataArray");
            if(myMethod == null)
            {
                Debug.Log("could not find necessary method 'setDataArray' in the Job's struct. Returning null.");
                return new object[] { null };
            }

            if (dataArrayType == null)
            {
                JobHandle handleNoArray = instance.Schedule();
                handleNoArray.Complete();
                return new object[] { null };
            }
            else
            {
                //Need to split everything into a big switch case and a lot of repeated code, due to needing to dispose of the array after finishing, and NativeArrays not accepting a generic type
                switch (dataArrayType.ToString())
                {
                    /*case "System.Byte":
                        NativeArray<System.Byte> dataArrayByte = new NativeArray<System.Byte>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayByte[i] = (System.Byte)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayByte);
                        JobHandle handleByte = instance.Schedule();
                        handleByte.Complete();
                        Array returnArrayByte = dataArrayByte.ToArray();
                        dataArrayByte.Dispose();
                        return returnArrayByte;
                    case "System.SByte":
                        NativeArray<System.SByte> dataArraySByte = new NativeArray<System.SByte>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArraySByte[i] = (System.SByte)baseDataArray[i];
                        }
                        instance.setDataArray(dataArraySByte);
                        JobHandle handleSByte = instance.Schedule();
                        handleSByte.Complete();
                        Array returnArraySByte = dataArraySByte.ToArray();
                        dataArraySByte.Dispose();
                        return returnArraySByte;
                    case "System.Int16":
                        NativeArray<System.Int16> dataArrayInt16 = new NativeArray<System.Int16>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayInt16[i] = (System.Int16)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayInt16);
                        instance.Execute();
                        //JobHandle handleInt16 = instance.Schedule();
                        //handleInt16.Complete();
                        Array returnArrayInt16 = dataArrayInt16.ToArray();
                        dataArrayInt16.Dispose();
                        return returnArrayInt16;
                    case "System.UInt16":
                        NativeArray<System.UInt16> dataArrayUInt16 = new NativeArray<System.UInt16>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayUInt16[i] = (System.UInt16)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayUInt16);
                        JobHandle handleUInt16 = instance.Schedule();
                        handleUInt16.Complete();
                        Array returnArrayUInt16 = dataArrayUInt16.ToArray();
                        dataArrayUInt16.Dispose();
                        return returnArrayUInt16;
                    case "System.Int32":
                        NativeArray<System.Int32> dataArrayInt32 = new NativeArray<System.Int32>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayInt32[i] = (System.Int32)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayInt32);
                        JobHandle handleInt32 = instance.Schedule();
                        handleInt32.Complete();
                        Array returnArrayInt32 = dataArrayInt32.ToArray();
                        dataArrayInt32.Dispose();
                        return returnArrayInt32;
                    case "System.UInt32":
                        NativeArray<System.UInt32> dataArrayUInt32 = new NativeArray<System.UInt32>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayUInt32[i] = (System.UInt32)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayUInt32);
                        JobHandle handleUInt32 = instance.Schedule();
                        handleUInt32.Complete();
                        Array returnArrayUInt32 = dataArrayUInt32.ToArray();
                        dataArrayUInt32.Dispose();
                        return returnArrayUInt32;
                    case "System.Int64":
                        NativeArray<System.Int64> dataArrayInt64 = new NativeArray<System.Int64>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayInt64[i] = (System.Int64)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayInt64);
                        JobHandle handleInt64 = instance.Schedule();
                        handleInt64.Complete();
                        Array returnArrayInt64 = dataArrayInt64.ToArray();
                        dataArrayInt64.Dispose();
                        return returnArrayInt64;
                    case "System.UInt64":
                        NativeArray<System.UInt64> dataArrayUInt64 = new NativeArray<System.UInt64>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayUInt64[i] = (System.UInt64)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayUInt64);
                        JobHandle handleUInt64 = instance.Schedule();
                        handleUInt64.Complete();
                        Array returnArrayUInt64 = dataArrayUInt64.ToArray();
                        dataArrayUInt64.Dispose();
                        return returnArrayUInt64;
                    case "System.IntPtr":
                        NativeArray<System.IntPtr> dataArrayIntPtr = new NativeArray<System.IntPtr>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayIntPtr[i] = (System.IntPtr)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayIntPtr);
                        JobHandle handleIntPtr = instance.Schedule();
                        handleIntPtr.Complete();
                        Array returnArrayIntPtr = dataArrayIntPtr.ToArray();
                        dataArrayIntPtr.Dispose();
                        return returnArrayIntPtr;
                    case "System.Single":
                        NativeArray<System.Single> dataArraySingle = new NativeArray<System.Single>(baseDataArray.Length, Allocator.TempJob);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArraySingle[i] = (System.Single)baseDataArray[i];
                        }
                        instance.setDataArray(dataArraySingle);
                        JobHandle handleSingle = instance.Schedule();
                        handleSingle.Complete();
                        Array returnArraySingle = dataArraySingle.ToArray();
                        dataArraySingle.Dispose();
                        return returnArraySingle;*//*
                    case "System.Double":
                        NativeArray<System.Double> dataArrayDouble = new NativeArray<System.Double>(baseDataArray.Length, Allocator.Persistent);
                        for (int i = 0; i < baseDataArray.Length; i++)
                        {
                            dataArrayDouble[i] = (System.Double)baseDataArray[i];
                        }
                        instance.setDataArray(dataArrayDouble);
                        instance.Execute();
                        //JobHandle handleDouble = instance.Schedule();
                        //handleDouble.Complete();
                        Array returnArrayDouble = dataArrayDouble.ToArray();
                        dataArrayDouble.Dispose();
                        return returnArrayDouble;
                    default:
                        Debug.Log("Job called with invalid data type, returning null.");
                        return new object[] { null };
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error occured processing job " + jobName + " on handler for tag " + handlerTag + ". Please ensure the setArrayData method accepts the correct data type sent in the Event. Error is as follows:");
            Debug.Log(e);
            return new object[] { null };
        }
    }*/
}
