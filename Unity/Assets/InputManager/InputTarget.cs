using UnityEngine;
using System.Collections;

public class InputTarget : MonoBehaviour {

   // public GameObject Target;

	// Use this for initialization
	void Start () {
	
	}

    public void SendMessage_Tap(){

        //if (Target.Equals(null))
            gameObject.SendMessage("OnTap", SendMessageOptions.DontRequireReceiver);

        /*
        else
        {
            try
            {
                Target.GetComponent<IButtonReceiver>().OnTap( gameObject.name );
                
            }
        catch
            {
                ;
            }
        }
        */

    }

    public void SendMessage_HoldStart()
    {
        //if( Target.Equals(null) )
            gameObject.SendMessage("OnHoldStart", SendMessageOptions.DontRequireReceiver);  
    }

    public void SendMessage_HoldComplete()
    {
        //if (Target.Equals(null))
            gameObject.SendMessage("OnHoldComplete", SendMessageOptions.DontRequireReceiver);
    }
    
    public void SendMessage_GazeEnter()
    {
       // if (Target.Equals(null))
            gameObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessage_GazeExit()
    {
      //  if (Target.Equals(null))
            gameObject.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessage_NavigationStarted(Vector3 relativePosition)
    {
      //  if (Target.Equals(null))
            gameObject.SendMessage("OnNavigationStarted", relativePosition, SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessage_NavigationUpdated(Vector3 relativePosition)
    {
      //  if (Target.Equals(null))
            gameObject.SendMessage("OnNavigationUpdated", relativePosition, SendMessageOptions.DontRequireReceiver);
    }
    public void SendMessage_NavigationCompleted(Vector3 relativePosition)
    {
      //  if (Target.Equals(null))
            gameObject.SendMessage("OnNavigationCompleted", relativePosition, SendMessageOptions.DontRequireReceiver);
    }

}
