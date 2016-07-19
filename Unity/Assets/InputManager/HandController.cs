using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

using MusicDevice;
using MusicUtilities;

/// <summary>
/// Provide direct access to the world space hand position.
/// If not in the HoloLens, provide an alternate virtual hand via mouse or xbox controller input.
/// 
/// Virtual hand:
///     - User presses button for 'finger up'
///     - User holds button to switch mouse to control hand position
/// </summary>
public class HandController : MonoBehaviour {

    bool virtualHand;
    bool vHandActivated;

    bool holoHand;

    private Vector3 handPosition;
    public Vector3 HandPosition { get { return handPosition; } }

    /// <summary>
    /// Tracks the hand detected state.
    /// </summary>
    public bool HandDetected
    {
        get;
        private set;
    }


    void Awake()
    {
        InteractionManager.SourceDetected += InteractionManager_SourceDetected;
        InteractionManager.SourceLost += InteractionManager_SourceLost;
        InteractionManager.SourcePressed += InteractionManager_SourcePressed;
        InteractionManager.SourceReleased += InteractionManager_SourceReleased;

    }
    void OnDestroy()
    {
        InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
        InteractionManager.SourceLost -= InteractionManager_SourceLost;
        InteractionManager.SourceReleased -= InteractionManager_SourceReleased;
        InteractionManager.SourcePressed -= InteractionManager_SourcePressed;
    }




    // Use this for initialization
    void Start () {
#if UNITY_EDITOR
        virtualHand = true;
#endif
#if NETFX_CORE
        holoHand = true;
#endif


    }


	// Update is called once per frame
	void Update () {

	    if( Input.GetKeyDown(KeyCode.C) )
        {
            vHandActivated = true;
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            vHandActivated = false;

        }

#if UNITY_EDITOR
        handPosition = Input.mousePosition;
#endif
#if NETFX_CORE
      
#endif

    }


    private void Test()
    {
        this.GetComponent<AudioSource>().Stop();
        this.GetComponent<AudioSource>().Play();
    }

    private void InteractionManager_SourceDetected(InteractionSourceState hand)
    {
        HandDetected = true;
        
    }

    private void InteractionManager_SourceLost(InteractionSourceState hand)
    {
        HandDetected = false;
    }

    private void InteractionManager_SourcePressed(InteractionSourceState hand)
    {
        
    }

    private void InteractionManager_SourceReleased(InteractionSourceState hand)
    {

    }







}
