using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Jobs;
//using Unity.Burst;
//using Unity.Collections;

public class TriggerPlatform2 : MonoBehaviour
{
    public bool startPlatform = false;
    //public MovingPlatform platformToMove;
    //public SoundPlayer soundToPlayOne;
    //public SoundPlayer soundToPlayTwo;
    //public SoundPlayer soundToPlayThree;

    CustomEventTrigger eventTriggers;

    // Start is called before the first frame update
    void Start()
    {
        eventTriggers = gameObject.GetComponent<CustomEventTrigger>();
        //Add a new ce_Trigger instance with parameters to the event trigger list
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call platform", //Trigger Name
            "MovingPlatform2", //Script Name
            "triggerMovement", //Method Name
            "firstPlatform", //Tag
            new object[] { false, true } //Parameters for method being called
        ));

        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call sound 1", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            "soundOne", //Tag
            new object[] { false } //Parameters for method being called
        ));

        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call sound 2", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            "soundTwo", //Tag
            new object[] { false } //Parameters for method being called
        ));

        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Call sound 3", //Trigger Name
            "SoundPlayer2", //Script Name
            "triggerAudio", //Method Name
            "soundThree", //Tag
            new object[] { false } //Parameters for method being called
        ));

        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Test call", //Trigger Name
            "TriggerPlatform2", //Script Name
            "standardEventTest", //Method Name
            "callTest" //Tag
        ));/*

        CustomEventTrigger.ce_TriggerJob.callback delegateToPass = callbackTest;
        eventTriggers.jobList.Add(new CustomEventTrigger.ce_TriggerJob(
            "Job test 1", //Trigger Name
            "JobTestScript", //Script Name
            "MyJob", //Job Name
            "jobTest", //Tag
            false,
            true,
            new double[10000000], //Blittable data array for the job to execute with and return data to
            delegateToPass
        ));

        job3Results = new NativeArray<double>(10000000, Allocator.Persistent);
        MyJob3 test3Job = new global::MyJob3();
        test3Job.results = job3Results;
        job3Handle = test3Job.Schedule();*/
    }

    //NativeArray<double> job3Results;
    //JobHandle job3Handle;

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            if (startPlatform)
            {
                //Trigger platform A movement
                //platformToMove.triggerMovement(false, true);
                eventTriggers.fireByName("Call platform");


                //eventTriggers.jobList[0].execute();


                /*NativeArray<double> jobResults = new NativeArray<double>(10000000, Allocator.Persistent);
                MyJob2 testJob = new global::MyJob2();
                testJob.results = jobResults;
                JobHandle handle = testJob.Schedule();
                handle.Complete();
                double[] results = jobResults.ToArray();
                jobResults.Dispose();
                Debug.Log("Scheduled Job finished, final array value: " + results[10000000 - 1]);*/


                /*double[] testReturn = standardCallTest();
                Debug.Log("standard call data returned, final array value: " + testReturn[10000000 - 1]);*/

                /*for (int i = 0; i < 10000; i++)
                {
                    standardCallTestNoReturn();
                }

                for (int i = 0; i < 10000; i++)
                {
                    eventTriggers.fireByName("Test call");
                }*/


                /*job3Handle.Complete();
                double[] results = job3Results.ToArray();
                job3Results.Dispose();
                Debug.Log("Scheduled Job finished");//, final array value: " + results[10000000 - 1]);*/
            }
            else
            {
                //Trigger victory sound
                float randNumber = UnityEngine.Random.Range(0, 0.9f);

                if(randNumber >= 0.3f)
                {
                    //soundToPlayOne.triggerAudio(false);
                    eventTriggers.fireByName("Call sound 1");
                }
                else if (randNumber >= 0.6f)
                {
                    //soundToPlayTwo.triggerAudio(false);
                    eventTriggers.fireByName("Call sound 2");
                }
                else
                {
                    //soundToPlayThree.triggerAudio(false);
                    eventTriggers.fireByName("Call sound 3");
                }
            }
        }
    }

    /*void callbackTest(List<Array> finishedData)
    {
        double[] recievedData = (double[])Array.CreateInstance(typeof(double), finishedData[0].Length);
        finishedData[0].CopyTo(recievedData, 0);
        Debug.Log("Event system job data recieved final array value: " + recievedData[10000000 - 1]);
    }*/

    /*double[] standardCallTest()
    {
        double[] calcTest = new double[10000000];
        for (int i = 0; i < 10000000; i++)
        {
            if (i == 0)
            {
                calcTest[i] = i;
            }
            else if (i % 2 == 0) {
                calcTest[i] = calcTest[i - 1] + i;
            }
            else
            {
                calcTest[i] = calcTest[i - 1] - i;
            }
        }
        return calcTest;
    }*/

    /*public bool standardEventTest()
    {
        double[] calcTest = new double[10];
        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
            {
                calcTest[i] = i;
            }
            else if (i % 2 == 0)
            {
                calcTest[i] = calcTest[i - 1] + i;
            }
            else
            {
                calcTest[i] = calcTest[i - 1] - i;
            }
        }
        Debug.Log("Im done!");
        return true;
    }

    public bool standardCallTestNoReturn()
    {
        double[] calcTest = new double[10];
        for (int i = 0; i < 10; i++)
        {
            if (i == 0)
            {
                calcTest[i] = i;
            }
            else if (i % 2 == 0)
            {
                calcTest[i] = calcTest[i - 1] + i;
            }
            else
            {
                calcTest[i] = calcTest[i - 1] - i;
            }
        }
        Debug.Log("Im done!");
        return true;
    }*/
}

/*[BurstCompile()]
public struct MyJob2 : IJob
{
    public NativeArray<double> results;

    public void Execute()
    {
        for (int i = 0; i < 10000000; i++)
        {
            if (i == 0)
            {
                results[i] = i;
            }
            else if (i % 2 == 0)
            {
                results[i] = results[i - 1] + i;
            }
            else
            {
                results[i] = results[i - 1] - i;
            }
        }
    }
}

[BurstCompile()]
public struct MyJob3 : IJob
{
    public NativeArray<double> results;

    public void Execute()
    {
        for (int i = 0; i < 10000000; i++)
        {
            if (i == 0)
            {
                results[i] = i;
            }
            else if (i % 2 == 0)
            {
                results[i] = results[i - 1] + i;
            }
            else
            {
                results[i] = results[i - 1] - i;
            }
        }
    }
}*/
