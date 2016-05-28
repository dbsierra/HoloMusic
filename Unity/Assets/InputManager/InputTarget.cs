using UnityEngine;
using System.Collections;

public class InputTarget : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	
    public void SendMessageTap()
    {
        Debug.Log("tap");
        gameObject.SendMessage("OnTap", SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessageGazeEnter()
    {
        Debug.Log("gaze enter");
        gameObject.SendMessage("OnGazeEnter", SendMessageOptions.DontRequireReceiver);
    }

    public void SendMessageGazeExit()
    {
        Debug.Log("gaze exit");
        gameObject.SendMessage("OnGazeExit", SendMessageOptions.DontRequireReceiver);
    }
}
