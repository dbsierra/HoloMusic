using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// The Sequencer will hold a 16x12 array of bools.
/// 12 = number of notes to an octave.
/// 16 = 4 bars of 4 beats each.
/// 
/// For each beat, a synthesizer will look up the corresponding array of notes
/// and see which notes in that array are turned on.
/// It will then play each of those notes.
/// 
/// TODO:
/// -Implement MIDI velocity for each note
/// -Everytime note is pressed or depressed on a row of sequencer, update the note information for that array
/// </summary>
public class Sequencer : MonoBehaviour {
    
	string[] pianoNotes;
    float[] frequencies;



    public TileData Grid;

	private int size = 16;
	public int Size{
		get{ return size;}
	}

    private int octave = 4;

	private bool ready;
	public bool Ready{
		get{ return ready;}
	}

    void Awake () {


        pianoNotes = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };


		ready = true;
	}
	
    public float[] GetNotes(int beat)
    {
        string[] notes = new string[12];

        int i = 0;
        int nIndex = 0;
        foreach( bool b in Grid.rows[beat].row )
        {
            if( b )
            {
                notes[nIndex++] = pianoNotes[i] + "" + octave;
            }
            i++;
        }

        float[] freqs = new float[nIndex];
        i = 0;
        foreach (string n in notes)
        {
            if (n != null)
            {
                freqs[i] = MusicUtil.getFreq(n);
                i++;
            }
        }
        if (i == 0)
            freqs = new float[] { 0 };

        return freqs;
    }

	void Update () {
	
	}

}
