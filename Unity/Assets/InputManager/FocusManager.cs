using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

/// <summary>
/// FocusManager's job is to ray cast from the camera and keep a running tab of what object is currently in focus.
/// </summary>

public class FocusManager : MonoBehaviour {

    private GameObject oldFocus;
    private GameObject focus;
    private GameObject focusCache;
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
    private Vector3 VHandStartPos;
    private float VHandHoldTime = .2f;
    private float startTime;

    private Cursor cursorInfo;
      
    // Use this for initialization
    void Start ()
    {

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
        recognizer.TappedEvent += Recognizer_FingerTap;
        recognizer.HoldStartedEvent += Recognizer_HoldStart;
        recognizer.HoldCompletedEvent += Recognizer_HoldComplete;
        recognizer.NavigationStartedEvent += Recognizer_NavigationStarted;
        recognizer.NavigationUpdatedEvent += Recognizer_NavigationUpdated;
        recognizer.NavigationCompletedEvent += Recognizer_NavigationCompleted;
        recognizer.StartCapturingGestures();
#endif

    }
      
    private void Recognizer_FingerTap( InteractionSourceKind source, int tapCount, Ray headRay )
    {
        TapEvent();
    }
    private void Recognizer_HoldStart( InteractionSourceKind source, Ray headRay )
    {
        HoldStartEvent();
    }
    private void Recognizer_HoldComplete( InteractionSourceKind source, Ray headRay )
    {
        HoldCompleteEvent();
    }
    private void Recognizer_NavigationStarted(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
    {
        NavigationStartedEvent(relativePosition);
    }
    private void Recognizer_NavigationUpdated(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
    {
        NavigationUpdatedEvent(relativePosition);
    }
    private void Recognizer_NavigationCompleted(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
    {
        NavigationCompletedEvent(relativePosition);
    }


    private void NavigationStartedEvent(Vector3 relativePosition)
    {
        if (focus != null)
        {
            focusCache = focus;

            #if UNITY_EDITOR
                VHandStartPos = Input.mousePosition;
                relativePosition = VHandStartPos;
            #endif

            cursorInfo.CursorMat.SetColor("_Color", cursorInfo.HoldColor);
            InputTarget target = focus.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessage_NavigationStarted(relativePosition);
            }
        }
    }
    /// <summary>
    /// User click+hold+move
    /// </summary>
    /// <param name="relativePosition"> relative offset position of hand in normalized units (within a 1m cube). Where you click start is the origin of the 1m cube boundaries.</param>
    private void NavigationUpdatedEvent(Vector3 relativePosition)
    {

#if UNITY_EDITOR

        relativePosition = relativePosition / 200f;
#endif
        if( focusCache )
        {
            InputTarget target = focusCache.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessage_NavigationUpdated(relativePosition);
            }
        }
    }
    private void NavigationCompletedEvent(Vector3 relativePosition)
    {
        
        cursorInfo.CursorMat.SetColor("_Color", cursorInfo.IdleColor);

        if( focusCache)
        {
            InputTarget target = focusCache.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessage_NavigationCompleted(relativePosition);
            }

            focusCache = null;
        }

    }
    private void HoldStartEvent()
    {
        
        if (focus != null)
        {
            InputTarget target = focus.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessage_HoldStart();
            }
        }
    }
    private void HoldCompleteEvent()
    {
        
        if (focus != null)
        {
            InputTarget target = focus.GetComponent<InputTarget>();

            if (target != null)
            {
                target.SendMessage_HoldComplete();
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
                target.SendMessage_Tap();
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
                //HoldCompleteEvent();
                NavigationCompletedEvent(Input.mousePosition);
                VHandHolding = false;
            }
        }

        if( VHandClickStarted )
        {
            if( Time.time - startTime >= VHandHoldTime )
            {
                VHandHolding = true;
                VHandClickStarted = false;
                NavigationStartedEvent(Input.mousePosition);
            }
        }

        if( VHandHolding )
        {
            NavigationUpdatedEvent(Input.mousePosition - VHandStartPos);   
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
                    target.SendMessage_GazeEnter();
                }
            }

            //Was the old focus on an object?
            if (oldFocus != null)
            {
                InputTarget target = oldFocus.GetComponent<InputTarget>();

                //targetable?
                if (target != null)
                {
                    target.SendMessage_GazeExit();
                }
            }

            oldFocus = focus;
        }

    }



}
