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

    public static FocusManager Instance;
        
    private GestureRecognizer recognizer;

    private bool VHandClickStarted;
    private bool VHandHolding;
    private float VHandHoldTime = .5f;
    private float startTime;

    private Cursor cursorInfo;
      
    // Use this for initialization
    void Start () {

        Instance = this;

        focus = oldFocus = null;

        cursorInfo = Cursor.Instance;

        if (Camera.main == null)
        {
            Debug.LogError(" FocusManager: No main camera exists, unable to use FocusManager.", this);
            GameObject.Destroy(this);
            return;
        }

#if NETFX_CORE
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += FingerTap;
        recognizer.HoldStartedEvent += HoldStart;
        recognizer.HoldCompletedEvent += HoldComplete;
        recognizer.StartCapturingGestures();
#endif

    }

    private void FingerTap( InteractionSourceKind source, int tapCount, Ray headRay )
    {
        TapEvent();
    }
    private void HoldStart( InteractionSourceKind source, Ray headRay )
    {
        HoldStartEvent();
    }
    private void HoldComplete( InteractionSourceKind source, Ray headRay )
    {
        HoldCompleteEvent();
    }
    private void HoldStartEvent()
    {
        cursorInfo.CursorMat.SetColor("_Color", cursorInfo.HoldColor);
        if (focus != null)
        {
            InputTarget target = focus.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessageHoldStart();
            }
        }
    }
    private void HoldCompleteEvent()
    {
        cursorInfo.CursorMat.SetColor("_Color", cursorInfo.IdleColor);
        if (focus != null)
        {
            InputTarget target = focus.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessageHoldComplete();
            }
        }
    }
    private void TapEvent()
    {
        
        cursorInfo.CursorMat.SetColor("_Color", cursorInfo.SelectColor);
        if (focus != null)
        {
            InputTarget target = focus.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessageTap();
            }

        }

        StartCoroutine("AfterTap");

    }

    private IEnumerator AfterTap()
    {
        yield return new WaitForSeconds(.1f);
        cursorInfo.CursorMat.SetColor("_Color", cursorInfo.IdleColor);
    }



    // Update is called once per frame
    void Update () {

#if UNITY_EDITOR

        #region virtual hand
        
        if(Input.GetMouseButtonDown(0) )
        {
            
            startTime = Time.time;
            VHandClickStarted = true;
        }
        if( Input.GetMouseButtonUp(0) )
        {
            VHandClickStarted = false;
            if (!VHandHolding)
            {
                TapEvent();
            }   
            else
            {
                HoldCompleteEvent();
                VHandHolding = false;
            }
                
        }

        if( VHandClickStarted )
        {
            if( Time.time - startTime >= VHandHoldTime )
            {
                VHandHolding = true;
                VHandClickStarted = false;
                HoldStartEvent();
            }
        }
        #endregion

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
