using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

/// <summary>
/// FocusManager's job is to ray cast from the camera and keep a running tab of what object is currently in focus.
/// </summary>

public class FocusManager : MonoBehaviour {

    private GameObject oldFocus;
    private GameObject focus;
    public GameObject Focus
    { 
        get { return focus; }
    }

    private RaycastHit hitInfo;
    public RaycastHit HitInfo { get; private set; }

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


    #region gesture variables
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

        focus = oldFocus = null;

        cursorInfo.cursorMat = GameObject.Find("Cursor geo").GetComponent<Renderer>().material;
        cursorInfo.idleColor = new Color(.646f, .646f, .646f);
        cursorInfo.selectColor = new Color(0, .588f, 1);

        if (Camera.main == null)
        {
            Debug.LogError(" FocusManager: No main camera exists, unable to use FocusManager.", this);
            GameObject.Destroy(this);
            return;
        }

#if NETFX_CORE
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += FingerTap;
        recognizer.StartCapturingGestures();
#endif
    }



    private void FingerTap( InteractionSourceKind source, int tapCount, Ray headRay)
    {
        TapEvent();
    }

    private void TapEvent()
    {
        cursorInfo.cursorMat.SetColor("_Color", cursorInfo.selectColor);
        if (focus != null)
        {
            InputTarget target = focus.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessageTap();
            }
        }
#if UNITY_EDITOR
        cursorInfo.cursorMat.SetColor("_Color", cursorInfo.idleColor);
#endif
#if NETFX_CORE
        StartCoroutine("AfterTap");
#endif
    }

    private IEnumerator AfterTap()
    {
        yield return new WaitForSeconds(.3f);
        cursorInfo.cursorMat.SetColor("_Color", cursorInfo.idleColor);
    }



    // Update is called once per frame
    void Update () {

#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0) )
        {
            cursorInfo.cursorMat.SetColor("_Color", cursorInfo.selectColor);
        }
        if( Input.GetMouseButtonUp(0) )
        {
            TapEvent();
        }
#endif
        headPosition = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;
	
        
        if( Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30 ) )
        {
            HitInfo = hitInfo;
            focus = HitInfo.collider.gameObject;
            
        }
        else
        {
            focus = null;
        }

        
        if (oldFocus != focus)
        {
            //Is the new focus on an object?
            if( focus != null )
            {
                InputTarget target = focus.GetComponent<InputTarget>();
                
                //Is this object targetable?
                if( target != null )
                {
                    target.SendMessageGazeEnter();
                }
            }

            //Was the old focus on an object?
            if (oldFocus != null)
            {
                InputTarget target = oldFocus.GetComponent<InputTarget>();

                //targetable?
                if (target != null)
                {
                    target.SendMessageGazeExit();
                }
            }

            oldFocus = focus;
        }

    }



}
