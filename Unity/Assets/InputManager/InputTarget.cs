using UnityEngine;
using System.Collections;

public class InputTarget : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	
    public void SendMessageTap()
    {
        Debug.Log("we here");
        gameObject.SendMessage("OnTap", SendMessageOptions.DontRequireReceiver);
    }


}
