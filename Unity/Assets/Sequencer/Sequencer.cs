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


    public SequencerBlock SeqBlock;
    public SynthManager instrument;

    public Color ActiveColor;
    public Color IdleColor;

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

    private matrixRow[] matrix;
    private struct matrixRow
    {
        public int count;
        public float[] notes;
        public SequencerBlock[] blocks;
    }

    private byte step;
    private byte oldStep = byte.MaxValue;
    private float time;

    private float spacing = .05f;

    void Awake () {

        
        pianoNotes = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        matrix = new matrixRow[16];

        for( int i =0; i < matrix.Length; i++ )
        {
            matrix[i].count = 0;
            matrix[i].notes = new float[12];
            matrix[i].blocks = new SequencerBlock[12];
        }

		ready = true;

        //assemble the sequencer GUI
        float size = SeqBlock.transform.localScale.x;
        for ( int row = 0; row < 12; row ++ )
        {
            for( int col = 0; col < 16; col++ )
            {
                matrix[col].blocks[row] = (SequencerBlock)GameObject.Instantiate(SeqBlock, new Vector3(col*(.1f+spacing) , row * (.1f + spacing), 0), Quaternion.identity);
                matrix[col].blocks[row].transform.parent = transform;
                matrix[col].blocks[row].NoteIndex = row;
                matrix[col].blocks[row].Beat = col;
            }
        }
	}

    /// <summary>
    /// Adds a note to the sequencer matrix
    /// </summary>
    /// <param name="beat"></param>
    /// <param name="noteIndex"></param>
    public void AddNote(int beat, int noteIndex)
    {
        matrix[beat].notes[noteIndex] = MusicUtil.getFreq( pianoNotes[noteIndex] + octave );
        matrix[beat].count++;
    }

    /// <summary>
    /// remove a note from the sequencer matrix
    /// </summary>
    /// <param name="beat"></param>
    /// <param name="noteIndex"></param>
    public void RemoveNote(int beat, int noteIndex)
    {
        matrix[beat].notes[noteIndex] = 0;
        matrix[beat].count--;
    }

    /// <summary>
    /// return all notes for this beat
    /// </summary>
    /// <returns>array of all notes for this row</returns>
    public float[] GetRow()
    {
        
        return matrix[step].notes;
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
                //Debug.Log(freqs[i]);
                i++;
            }
        }
        if (i == 0)
            freqs = new float[] { 0 };

        return freqs;
    }

    
	void Update () {
        time += Time.deltaTime;

        step = (byte)(Mathf.Floor( time / MusicUtil.BeatLength ) % size);

        //TRIGGER!
        if ( step != oldStep )
        {

            foreach ( float f in GetRow() )
            {
                if( f != 0 )
                    instrument.NoteOn(f);
            }

            for (int i = 0; i < 12; i++)
            {
                matrix[step].blocks[i].Highlight();

                if( oldStep >= 0 && oldStep <= matrix.Length )
                    matrix[oldStep].blocks[i].DeHighlight();    
            }

            oldStep = step;
        }
    }

}
