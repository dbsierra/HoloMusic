using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

/// <summary>
/// FocusManager's job is to ray cast from the camera and keep a running tab of what object is currently in focus.
/// </summary>

public class FocusManager : MonoBehaviour {

    private GameObject focus;
    public GameObject Focus
    {
        get { return focus; }
    }
    private RaycastHit hitInfo;
    public RaycastHit HitInfo
    {
        get { return hitInfo; }
    }
    private Vector3 headPosition;
    public Vector3 HeadPosition
    {
        get { return headPosition; }
    }
    private Vector3 gazeDirection;
    public Vector3 GazeDirection
    {
        get { return gazeDirection; }
    }


    #region Gesture
    GestureRecognizer recognizer;

    struct CursorInfo
    {
        public Material cursorMat;
        public Color idleColor;
        public Color selectColor;
    }
    CursorInfo cursorInfo;
    #endregion

    // Use this for initialization
    void Start () {

        cursorInfo.cursorMat = GameObject.Find("Cursor geo").GetComponent<Renderer>().material;
        cursorInfo.idleColor = new Color(.646f, .646f, .646f);
        cursorInfo.selectColor = new Color(0, .588f, 1);

        if (Camera.main == null)
        {
            Debug.LogError(" FocusManager: No main camera exists, unable to use FocusManager.", this);
            GameObject.Destroy(this);
            return;
        }

        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += FingerTap;
        //recognizer.StartCapturingGestures();
    }
	
    private void FingerTap( InteractionSourceKind source, int tapCount, Ray headRay)
    {
        //finger tap stuff
    }

    

	// Update is called once per frame
	void Update () {

        if(Input.GetMouseButtonDown(0) )
        {
            cursorInfo.cursorMat.SetColor("_Color", cursorInfo.selectColor);
        }
        if( Input.GetMouseButtonUp(0) )
        {
            cursorInfo.cursorMat.SetColor("_Color", cursorInfo.idleColor);
            if ( focus != null )
            {
                InputTarget target = focus.GetComponent<InputTarget>();

                if (target != null)
                {
                    target.SendMessageTap();
                }
            }
        }

        headPosition = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;
	
        if( Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30 ) )
        {
            focus = hitInfo.collider.gameObject;
        }
        else
        {
            focus = null;
        }

	}



}
