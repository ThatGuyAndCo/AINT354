using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
*/
public class JobTestScript : MonoBehaviour
{
    
    
}
/*
[BurstCompile()]
public struct MyJob : IJob
{
    public NativeArray<double> dataArray;

    public void setDataArray(NativeArray<double> dataArray) { this.dataArray = dataArray; }

    public void Execute()
    {
        Debug.Log("I have been called");
        for (int i = 0; i < 10000000; i++)
        {
            if (i == 0)
            {
                dataArray[i] = i;
            }
            else if (i % 2 == 0)
            {
                dataArray[i] = dataArray[i - 1] + i;
            }
            else
            {
                dataArray[i] = dataArray[i - 1] - i;
            }
        }
    }
}*/

