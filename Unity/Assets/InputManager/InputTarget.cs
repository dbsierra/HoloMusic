using UnityEngine;
using System.Collections;

public class InputTarget : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	
    public void SendMessage_Tap(){ gameObject.SendMessage("OnTap", SendMessageOptions.DontRequireReceiver); }

    public void SendMessage_HoldStart()
    {
        gameObject.SendMessage("OnHoldStart", SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessage_HoldComplete()
    {
        gameObject.SendMessage("OnHoldComplete", SendMessageOptions.DontRequireReceiver);
    }
    
    public void SendMessage_GazeEnter()
    {
        gameObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessage_GazeExit()
    {
        gameObject.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessage_NavigationStarted(Vector3 relativePosition)
    {
        gameObject.SendMessage("OnNavigationStarted", relativePosition, SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessage_NavigationUpdated(Vector3 relativePosition)
    {
        gameObject.SendMessage("OnNavigationUpdated", relativePosition, SendMessageOptions.DontRequireReceiver);
    }
    public void SendMessage_NavigationCompleted(Vector3 relativePosition)
    {
        gameObject.SendMessage("OnNavigationCompleted", relativePosition, SendMessageOptions.DontRequireReceiver);
    }

}
