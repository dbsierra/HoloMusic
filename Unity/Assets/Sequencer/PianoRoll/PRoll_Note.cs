using UnityEngine;
using System.Collections;
using Sequencer.PianoRoll;

public class PRoll_Note : MonoBehaviour {

    public PRoll_Controller Controller;
    public byte PitchIndex;
    public byte PositionIndex;

	void Start () {
        
    }
	
	void Update () {
	
	}

    public void Delete()
    {
        Controller.RemoveNote(PositionIndex, PitchIndex);
        GameObject.Destroy(gameObject);
    }

    private void OnTap()
    {
        Delete();
    }
}
