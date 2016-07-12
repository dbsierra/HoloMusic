using UnityEngine;
using System.Collections;

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

    }



}
