using UnityEngine;
using System.Collections;

public class LooperInterface : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        transform.position = FocusManager.Instance.HeadPosition;

       float dot = Vector3.Dot(transform.forward, FocusManager.Instance.transform.forward);

        if( dot <= .6 )
        {
            transform.LookAt(FocusManager.Instance.transform.forward);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
        }

       // transform.LookAt( FocusManager.Instance.transform.forward);
        //transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

        //transform.localEulerAngles = new Vector3( 0,   FocusManager.Instance.transform.localEulerAngles.y, 0);

    }
}
