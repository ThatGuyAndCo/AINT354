using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEventMaster : MonoBehaviour
{
    private CustomEventHandler[] allEventHandlers;
    // Start is called before the first frame update
    void Start()
    {
        allEventHandlers = Resources.FindObjectsOfTypeAll<CustomEventHandler>();
    }

    public List<bool> sendEvent(string methodName, string tag, bool activateInactive, bool exactTagMatch, string componentName, object[] optionalParams)
    {
        List<bool> results = new List<bool>();
        for(int i = 0; i < allEventHandlers.Length; i++)
        {
            if (exactTagMatch)
            {
                if (allEventHandlers[i].handlerTag.ToLower().Trim() == tag.ToLower().Trim())
                {
                    if (activateInactive && !allEventHandlers[i].gameObject.activeSelf)
                    {
                        allEventHandlers[i].gameObject.SetActive(true);
                    }
                    results.Add(allEventHandlers[i].callMethod(methodName, componentName, optionalParams));
                }
            }
            else
            {
                if (allEventHandlers[i].handlerTag.ToLower().Trim().Contains(tag.ToLower().Trim()))
                {
                    if (activateInactive && !allEventHandlers[i].gameObject.activeSelf)
                    {
                        allEventHandlers[i].gameObject.SetActive(true);
                    }
                    results.Add(allEventHandlers[i].callMethod(methodName, componentName, optionalParams));
                }
            }
        }
        return results;
    }


}
