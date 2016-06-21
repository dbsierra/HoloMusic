using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;

/*
public class SqnRow
{
    public Dictionary<string, MIDINote> map;
    public SqnRow()
    {
        map = new Dictionary<string, MIDINote>();
    }
}

public class Sequencer : MonoBehaviour
{
    public byte octave;

    private SqnRow[] sqnMatrix;

    private void SequencerDebugValues()
    {
        AddNote(0, "C" + octave, 2, 1);
        AddNote(4, "D" + octave, 2, 1);
        AddNote(8, "E" + octave, 2, 1);
        AddNote(12, "F" + octave, 2, 1);
    }
    public void AddNote(byte beat, string note, byte length, byte velocity)
    {
        MIDINote n = new MIDINote(Settings.getMIDI(note), length, velocity);
        sqnMatrix[beat].map[note] = n;
    }

    void Start()
    {
        
    }
    void Update()
    {

    }

}

    */


public class Sequencer : MonoBehaviour {
    
	string[] pianoNotes;
    float[] frequencies;

    public struct TestStruct
    {
        public int yeah;
    }
    public TestStruct test;

    public SequencerBlock SeqBlock;
    public SynthManager instrument;

    public Color ActiveColor;
    public Color IdleColor;
    public byte octave = 4;

    public TileData Grid;

	private int size = 16;
	public int Size{
		get{ return size;}
	}

	private bool ready;
	public bool Ready{get{ return ready;}}

    private matrixRow[] matrix;
    private struct matrixRow
    {
        public int count;
        public float[] notes;
        public SequencerBlock[] blocks;
        public List<MIDINote> midiNotes;
    }
    private List<MIDINote> notesCurrentlyPlaying;


    private byte step = 0;
    private byte oldStep = 0;
    private float time;

    private float spacing = .05f;

    #region sequencer matrix functions
    /// <summary>
    /// Add some values to the sequencer matrix for quick debug purposes
    /// </summary>
    private void SequencerDebugValues()
    {
        AddNote(0, "C", 2, 1);
        AddNote(4, "D", 2, 1);
        AddNote(8, "E", 2, 1);
        AddNote(12, "F", 2, 1);
    }
    /// <summary>
    /// Pint out the sequencer matrix
    /// </summary>
    private void SequencerPrint()
    {
        for (int i = 0; i < matrix.Length; i++)
        {
            string s = "";
            foreach( MIDINote n in matrix[i].midiNotes )
            {               
                s += " " + Settings.getFreq( n.midi );
            }
            //Debug.Log("beat:" + i + " | " + s);
        }
    }
    /// <summary>
    /// Initialize the sequencer matrix
    /// </summary>
    private void InitSequencerMatrix()
    {
        matrix = new matrixRow[16];

        for (int i = 0; i < matrix.Length; i++)
        {
            matrix[i].count = 0;
            matrix[i].notes = new float[12];
            matrix[i].blocks = new SequencerBlock[12];
            matrix[i].midiNotes = new List<MIDINote>();
        } 
    }
    #endregion

    /// <summary>
    /// Add a note to the sequencer matrix
    /// </summary>
    /// <param name="beat">The beat you are adding a note to</param>
    /// <param name="note">The name of the C sans octave</param>
    /// <param name="length">The length of the note</param>
    /// <param name="velocity">The velocity of the note</param>
    public void AddNote(byte beat, string note, byte length, byte velocity)
    {
        MIDINote n = new MIDINote(Settings.getMIDI(note + octave), length, velocity) ;
        n.midi = Settings.getMIDI(note + octave);
        matrix[beat].midiNotes.Add(n);
    }

    void Awake () {

        pianoNotes = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        notesCurrentlyPlaying = new List<MIDINote>();
        InitSequencerMatrix();
        SequencerDebugValues();
        SequencerPrint();

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


        for (int row = 0; row < 16; row++)
        {
            foreach( MIDINote n in matrix[row].midiNotes )
            {

              //  Debug.Log( n.midi );
            }
        }
    }


    /// <summary>
    /// return all notes for this beat
    /// </summary>
    /// <returns>array of all notes for this row</returns>
    private List<MIDINote> GetRow()
    {
        return matrix[step].midiNotes;
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
                freqs[i] = Settings.getFreq(n);
                //Debug.Log(freqs[i]);
                i++;
            }
        }
        if (i == 0)
            freqs = new float[] { 0 };

        return freqs;
    }

    public void OnStep(byte s)
    {
        step = s;
        //Debug.Log(s);
        //send note over
        foreach( MIDINote n in GetRow() )
        {

            //Debug.Log("On " + n.midi);
            instrument.NoteOn(n);
           // Debug.Log(n.midi);
        }
        
    }

    void Update () {
       
        if( step != oldStep )
        {
           // Debug.Log("TRIGGER on MAIN THREAD");
            oldStep = step;

            for (int i = 0; i < 12; i++)
            {
                matrix[step].blocks[i].Highlight();

                if (oldStep >= 0 && oldStep <= matrix.Length)
                    matrix[oldStep].blocks[i].DeHighlight();
            }
        }
    }

}
