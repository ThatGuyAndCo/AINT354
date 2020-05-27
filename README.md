# AINT354

# How to Use

Using the tool is very simple, and falls into 4 basic steps. 

# Step 1...
is to ensure that some object, somewhere within the scene has the CustomEventMaster script attached. This will generally be an object that you might use for controlling generic things in the game, such as pausing, but if in doubt, put it on a new empty GameObject. As long as this object is active, nothing else is needed for this step.

# Step 2...
is to drag and drop the CustomEventTrigger script onto whichever object will fire off an event. You can add new triggers via the inspector, or access the triggerList variable via a reference to the script, using the ce_Trigger class. Each ce_Trigger is made up of the following properties, see the example later for how to properly use each:
triggerName - A name for the trigger, through which the trigger can be fired
scriptName - The name of the script file containing the code that the event will run
methodName - The name of the method for the event to call, the method being called must have a return type of boolean, which will be returned in a list once the trigger has been fired, to ensure the events have been fired successfully
tag - A tag used to identify which Handlers should handle the event
enableInactiveObjects - A boolean that will activate inactive objects (to be used with pooled objects), optional, defaults to false
exactTagMatch - A boolean that controls whether the Handler's tag needs to match the trigger's tag exactly, or if it just needs to contain the phrase in the trigger's tag
parameters - An object array for passing variables to the method being called

# Step 3...
requires adding the CustomEventHandler script to the object where the event will be handled. You will need to fill in the HandlerTag field as this will identify the Handler from all others in the scene. Tags do not need to be unique, as generic handlers can allow you to have multiple handlers for an event. See the examples below for more information.

# Step 4...
In the script where you want the event to be fired, get a reference to the CustomEventTrigger script. Here you can use the fireByName(triggerName) or fireAllByName(triggerName, exactNameMatch) methods to fire your event (see the examples below if necessary). You can also fire an event by accessing it through the triggerList property and calling the fire() method on it. Firing events will return a list of booleans which can be used to ensure that all events have been fired successfully.

# For example...
Let's say I have a Script called 'Attack.cs' and a script called 'Enemy.cs'. In 'Attack.cs', I will check each frame if a button is pressed, and if so, I will fire an event telling the 'Enemy.cs' to run a method called 'TakeDamage'. Lastly, I want only Human enemies to take damage, not Beast enemies. 

In the scene I have an object called Master which will hold the CustomEventMaster.cs script. I have an object called Player that contains the Attack.cs script. I have 5 other objects in the scene (Enemy1 to Enemy5), all of which hold the Enemy.cs script.

I add CustomEventTrigger to the Player object, and CustomEventHandler to the Enemy objects. 3 of the Enemy objects are given a tag of Beast, 2 are given a tag of Human.

The Attack.cs script would look something like this:
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : MonoBehaviour
{
    //Reference to the Trigger script
    CustomEventTrigger eventTriggers;

    // Start is called before the first frame update
    void Start()
    {
        //On start, get the script reference from the current object
        eventTriggers = gameObject.GetComponent<CustomEventTrigger>();
        
        //Add a new trigger
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Attack Humans", //Trigger Name
            "Enemy", //Script Name
            "TakeDamage", //Method Name
            "Human" //Tag
        ));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            eventTriggers.fireByName("Attack Humans");
        }
    }
}
```

Let's say the TakeDamage method requires a float, telling the method how much damage the enemy should take. We want the enemy to take 10.5 points of damage. The trigger would look more like this:
```
eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
    "Attack Humans", //Trigger Name
    "Enemy", //Script Name
    "TakeDamage", //Method Name
    "Human", //Tag
    new object[] { 10.5 }
));
```

Finally, lets say we want the Humans and Beasts to take damage together. There are 3 main ways we can do this. First, we can use a shared character between both of the Handlers. In this specific case, its a bit silly to do this, but if you had say 3 sound players with the tag 'sound1', 'sound2', and 'sound3', this method could be considered more acceptable. Either way, the handlers share a character 'a' (beAst, and humAn). So the trigger could look like this:
```
eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
    "Attack Humans", //Trigger Name
    "Enemy", //Script Name
    "TakeDamage", //Method Name
    "a", //Tag
    false, //Enable disabled objects
    false, //Exact tag match (default = true)
    new object[] { 10.5 }
));
```

By turning off exact tag match, the letter a will match with the tags beast and human.

The second method of doing this is to attach multiple handlers to the Enemies. We can keep the initial 'Beast' and 'Human' tags, but also add a 'Guard' tag to each of them. This way, we can attack all of the 'Guard' objects at once. 

The final method of doing this is to create multiple triggers. If we create a trigger for attacking Humans and a trigger for attacking Beasts, we can use the Trigger Name and the fireAllByName method to fire both triggers at once. So our attack class would look like this:
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Attack : MonoBehaviour
{
    //Reference to the Trigger script
    CustomEventTrigger eventTriggers;

    // Start is called before the first frame update
    void Start()
    {
        //On start, get the script reference from the current object
        eventTriggers = gameObject.GetComponent<CustomEventTrigger>();
        
        //Add a new trigger for Humans
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Attack Humans", //Trigger Name
            "Enemy", //Script Name
            "TakeDamage", //Method Name
            "Human" //Tag
        ));
        
        //Add a new trigger for Beasts
        eventTriggers.triggerList.Add(new CustomEventTrigger.ce_Trigger(
            "Attack Beasts", //Trigger Name
            "Enemy", //Script Name
            "TakeDamage", //Method Name
            "Beast" //Tag
        ));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //The false below turns off exact tag match. As in method 2, where we attack both enemies using the character 'a'
            //shared between the tags, here we have the word Attack in both of the Trigger Names, allowing them
            //both to be fired in one go.
            eventTriggers.fireAllByName("Attack", false);
        }
    }
}
```
