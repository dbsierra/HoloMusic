using UnityEngine;
using System.Collections;

public class ManualCameraControl : MonoBehaviour {

    void Start () {
	
	}

    float yStart;
    float xStart;
    float xDelta;
    float yDelta;
    private float TurnSensitivity = 400;
    private float MoveSensitivity = 0.5f;
    bool turning;
    Vector3 initialRot;
    Vector3 newRot;

	void Update () {

        if (Input.GetMouseButtonDown(1))
        {
            xStart = Input.mousePosition.x ;
            yStart = Input.mousePosition.y ;
            turning = true;
            initialRot = transform.localEulerAngles;
        }

        if(turning)
        {
            xDelta = (Input.mousePosition.x - xStart) / Screen.width;
            yDelta = (Input.mousePosition.y - yStart) / Screen.height;            

            transform.localEulerAngles = new Vector3(initialRot.x - yDelta * TurnSensitivity, initialRot.y + xDelta * TurnSensitivity, 0);
        }

        if (Input.GetMouseButtonUp(1))
        {
            turning = false;
        }


        if(Input.GetButton("MoveForward"))
        {
            transform.Translate( Vector3.forward * MoveSensitivity );
        }
        else if (Input.GetButton("MoveBack"))
        {
            transform.Translate(Vector3.forward * MoveSensitivity * -1);
        }
        else if(Input.GetButton("MoveLeft"))
        {
            transform.Translate(Vector3.right * MoveSensitivity * -1);
        }
        else if(Input.GetButton("MoveRight"))
        {
            transform.Translate(Vector3.right * MoveSensitivity );
        }

    }



}
