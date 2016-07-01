using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;
using MusicDevice;

/// <summary>
/// Represents a sequence of MIDI notes within 4 bars.
/// 1 bar = 4 beats (1/4 notes)
/// </summary>
public class PianoRoll : MonoBehaviour {


    List<MIDINote> heldNotes; //note that has sent a gate On but not a gate Off message
    List<MIDINote>[] matrix;
    FMSynthesizer instrument;
    int step;

	void Start () {
        //Init
        heldNotes = new List<MIDINote>();
        matrix = new List<MIDINote>[16*8]; //16 bars, 1/32nd note resolution
        step = 0;
	}
	
	void Update () {
	
	}

    void OnAudioFilterRead(float[] data, int channels)
    { 
        
    }

    public void OnStep()
    {
       step += 1;
       //play note event
       foreach( MIDINote n in matrix[step] )
       {
            if( n.noteOn )
            {

            }
       }
    }

}
