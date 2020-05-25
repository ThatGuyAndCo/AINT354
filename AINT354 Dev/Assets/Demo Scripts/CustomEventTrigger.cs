using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventTrigger : MonoBehaviour
{

    /************************************************************************************************Basic Triggers*******************************************************************************/

    [System.Serializable]
    public class ce_Trigger
    {
        private CustomEventMaster master;

        //Defaults provided for creating via inspector
        public string triggerName = "";
        public string scriptName = "";
        public string methodName = "";
        public string tag = "";
        public bool enableInactiveObjects = false;
        public bool exactTagMatch = true;
        public bool foldout = true; //Used for Inspector
        private object[] _parameters;

        public ce_Trigger(string triggerName, string scriptName, string methodName, string tag)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = false;
            this.exactTagMatch = true;
            this.parameters = new object[] { };
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public ce_Trigger(string triggerName, string scriptName, string methodName, string tag, object[] parameters)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = false;
            this.exactTagMatch = true;
            this.parameters = parameters;
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public ce_Trigger(string triggerName, string scriptName, string methodName, string tag, bool enableInactiveObjects, bool exactTagMatch)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = enableInactiveObjects;
            this.exactTagMatch = exactTagMatch;
            this.parameters = new object[] { };
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public ce_Trigger(string triggerName, string scriptName, string methodName, string tag, bool enableInactiveObjects, bool exactTagMatch, object[] parameters)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = enableInactiveObjects;
            this.exactTagMatch = exactTagMatch;
            this.parameters = parameters;
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public List<bool> fire()
        {
            if(master == null)
                master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();

            return master.sendEvent(methodName, tag, enableInactiveObjects, exactTagMatch, scriptName, parameters);
        }

        public object[] parameters{
            get {
                return _parameters;
            }
            set {
                _parameters = value;
            }
        }
    }

    public List<ce_Trigger> triggerList = new List<ce_Trigger>();

    //If method finds multiple triggers with the same name it will fire the first trigger it finds (case in-sensitive)
    public List<bool> fireByName(string triggerName)
    {
        List<bool> returnables = new List<bool>();
        for (int i = 0; i < triggerList.Count; i++)
        {
            if (triggerList[i].triggerName.ToLower() == triggerName.ToLower())
            {
                returnables = triggerList[i].fire();
                break;
            }
        }
        return returnables;
    }

    //If method finds multiple triggers with the same name it will fire all of them (case in-sensitive). If exact match is false, it will fire any triggers that have the given string in the trigger name
    public List<bool> fireAllByName(string triggerName, bool exactMatch)
    {
        List<bool> returnables = new List<bool>();
        for (int i = 0; i < triggerList.Count; i++)
        {
            if ((exactMatch && triggerList[i].triggerName.ToLower() == triggerName.ToLower()) || (!exactMatch && triggerList[i].triggerName.ToLower().Contains(triggerName.ToLower())))
            {
                returnables.AddRange(triggerList[i].fire());
            }
        }
        return returnables;
    }
    





    /****************************************************************************************************Jobs*******************************************************************************/
    /*
    public class ce_TriggerJob //: IJob
    {
        private CustomEventMaster master;
        
        public string triggerName;
        private string scriptName;
        private string jobName;
        private string tag;
        private bool enableInactiveObjects;
        private bool exactTagMatch;

        private Array dataArray;
        public delegate void callback(List<Array> dataArray);
        callback callbackMethod;
        private Type dataArrayType;
        //private bool correctDataArrayType = false;


        //No data array, no callback method, used for simple job triggers
        public ce_TriggerJob(string triggerName, string scriptName, string jobName, string tag)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.jobName = jobName;
            this.tag = tag;
            this.enableInactiveObjects = false;
            this.exactTagMatch = true;
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
        }

        //No data array, no callback method, used for simple job triggers
        public ce_TriggerJob(string triggerName, string scriptName, string jobName, string tag, bool enableInactiveObjects, bool exactTagMatch)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.jobName = jobName;
            this.tag = tag;
            this.enableInactiveObjects = enableInactiveObjects;
            this.exactTagMatch = exactTagMatch;
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
        }

        //dataArray will be passed to the job, callback method called on successful execution. Callback method needs params of List<bool> and T[], to return the successes/failures and the data from the job respectively
        //T[] of callback needs to be the same datatype as the dataArray parameter. DataArray param supports only blittable data types, see https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types for more info
        public ce_TriggerJob(string triggerName, string scriptName, string jobName, string tag, Array dataArray, callback callbackMethod)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.jobName = jobName;
            this.tag = tag;
            this.enableInactiveObjects = false;
            this.exactTagMatch = true;
            this.dataArray = dataArray;
            this.callbackMethod = callbackMethod;
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
            this.dataArrayType = dataArray.GetType().GetElementType();
            Debug.Log(dataArray.GetType().GetElementType().ToString());
        }

        //dataArray will be passed to the job, callback method called on successful execution. Callback method needs params of List<bool> and T[], to return the successes/failures and the data from the job respectively
        //T[] of callback needs to be the same datatype as the dataArray parameter. DataArray param supports only blittable data types, see https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types for more info
        public ce_TriggerJob(string triggerName, string scriptName, string jobName, string tag, bool enableInactiveObjects, bool exactTagMatch, Array dataArray, callback callbackMethod)
        {
            this.triggerName = triggerName;
            this.scriptName = scriptName;
            this.jobName = jobName;
            this.tag = tag;
            this.enableInactiveObjects = enableInactiveObjects;
            this.exactTagMatch = exactTagMatch;
            this.dataArray = dataArray;
            this.callbackMethod = callbackMethod;
            this.master = UnityEngine.Object.FindObjectOfType<CustomEventMaster>();
            this.dataArrayType = dataArray.GetType().GetElementType();
            Debug.Log(dataArray.GetType().GetElementType().ToString());
        }

        public void updateCallback(callback callbackMethod)
        {
            this.callbackMethod = callbackMethod;
        }

        public void updateDataArray(dynamic[] dataArray)
        {
            this.dataArray = dataArray;
            this.dataArrayType = dataArray.GetType().GetElementType();
            Debug.Log(dataArray.GetType().GetElementType().ToString());
        }*/

        /*public void checkDataArrayType()
        {
            switch (dataArrayType)
            {
                case System.Int16 :

                    break;
            }
        }*//*

        //If needed, call updateDataArray first. If the job fails, the returned array will only contain null. If multiple jobs execute, it will return all results in a List from order of execution, the failed ones containing arrays with null.
        public void execute()
        {
            object[] objectArray = (object[])Array.CreateInstance(typeof(object), dataArray.Length);
            dataArray.CopyTo(objectArray, 0);
            List<Array> returnedData = master.sendEventJob(jobName, tag, enableInactiveObjects, exactTagMatch, scriptName, objectArray, dataArrayType);
            if (callbackMethod != null)
            {
                callbackMethod.Invoke(returnedData);
            }
        }
    }

    public List<ce_TriggerJob> jobList = new List<ce_TriggerJob>();

    //If method finds multiple triggers with the same name it will fire the first trigger it finds (case in-sensitive)
    public void executeByName(string triggerName)
    {
        for (int i = 0; i < triggerList.Count; i++)
        {
            if (jobList[i].triggerName.ToLower() == triggerName.ToLower())
            {
                jobList[i].execute();
                break;
            }
        }
    }

    //If method finds multiple triggers with the same name it will fire all of them (case in-sensitive). If exact match is false, it will fire any triggers that have the given string in the trigger name
    public void executeAllByName(string triggerName, bool exactMatch)
    {
        for (int i = 0; i < jobList.Count; i++)
        {
            if ((exactMatch && jobList[i].triggerName.ToLower() == triggerName.ToLower()) || (!exactMatch && jobList[i].triggerName.ToLower().Contains(triggerName.ToLower())))
            {
                jobList[i].execute();
            }
        }
    }*/


}
