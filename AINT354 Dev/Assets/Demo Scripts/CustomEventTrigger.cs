using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventTrigger : MonoBehaviour
{
    [System.Serializable]
    public class ce_TriggerStruct
    {
        private CustomEventMaster master;

        //Defaults provided for creating via inspector
        public string scriptName = "";
        public string methodName = "";
        public string tag = "";
        public bool enableInactiveObjects = false;
        public bool exactTagMatch = true;
        public bool foldout = true; //Used for Inspector
        private object[] _parameters;

        public ce_TriggerStruct(string scriptName, string methodName, string tag)
        {
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = false;
            this.exactTagMatch = true;
            this.parameters = new object[] { };
            this.master = Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public ce_TriggerStruct(string scriptName, string methodName, string tag, object[] parameters)
        {
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = false;
            this.exactTagMatch = true;
            this.parameters = parameters;
            this.master = Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public ce_TriggerStruct(string scriptName, string methodName, string tag, bool enableInactiveObjects, bool exactTagMatch)
        {
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = enableInactiveObjects;
            this.exactTagMatch = exactTagMatch;
            this.parameters = new object[] { };
            this.master = Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public ce_TriggerStruct(string scriptName, string methodName, string tag, bool enableInactiveObjects, bool exactTagMatch, object[] parameters)
        {
            this.scriptName = scriptName;
            this.methodName = methodName;
            this.tag = tag;
            this.enableInactiveObjects = enableInactiveObjects;
            this.exactTagMatch = exactTagMatch;
            this.parameters = parameters;
            this.master = Object.FindObjectOfType<CustomEventMaster>();
            this.foldout = true;
        }

        public List<bool> fire()
        {
            if(master == null)
                master = Object.FindObjectOfType<CustomEventMaster>();

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

    public List<ce_TriggerStruct> triggerList = new List<ce_TriggerStruct>();
}
