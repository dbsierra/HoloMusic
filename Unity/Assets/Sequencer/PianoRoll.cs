using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;
using MusicDevice;

/// <summary>
/// Represents a sequence of MIDI notes of length 4 bars.
/// 1 bar = 4 beats (1/4 notes in our 4/4 time signature)
/// </summary>
public class PianoRoll : MonoBehaviour {

    List<NoteEvent>[] matrix;
    FMSynthesizer instrument; 
    byte step;
    int beatLength_s;
    int sample;
    bool ready;

	void Start () {
        matrix = new List<NoteEvent>[16*8]; //16 bars, 1/32nd note resolution
        for( int i =0; i<matrix.Length; i++ )
        {
            matrix[i] = new List<NoteEvent>();
        }
        step = 0;
        beatLength_s = (int)(Settings.BeatLength * Settings.SampleRate);
        instrument = new FMSynthesizer();
        ready = true;

        AddNote(0, Settings.getMIDI("C#4"), 1, 1);
        AddNote(1, Settings.getMIDI("F#4"), 1, 1);
        AddNote(2, Settings.getMIDI("E4"), 1, 1);
        AddNote(3, Settings.getMIDI("C#4"), 1, 1);
    }
	
	void Update () {
	
	}
    
    void AddNote(byte beat, byte pitch, byte velocity, byte duration)
    {
        MIDINote tmp = new MIDINote(pitch, duration, velocity);
        AddNote(beat, tmp);
    }

    //adds two notes, for the start and end of the duration
    void AddNote(byte beat, MIDINote n)
    {
        matrix[beat].Add(new NoteEvent(n, true));        
        int endBeat = Mathf.Clamp(beat + n.duration, 0, matrix.Length - 1);
        matrix[endBeat].Add(new NoteEvent(n, false));
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if( ready )
        {
            for (int i = 0; i < data.Length; i = i + channels)
            {
                if (sample >= beatLength_s)
                {
                    OnStep();
                    sample = 0;
                }
                else
                    sample++;


                float s = instrument.NextSample();

                data[i] = .5f * s;
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }
            }
        }

    }

    public void OnStep()
    {
        Debug.Log("Step: " + step + " " + matrix[step].Count);
       //play note event

       foreach( NoteEvent n in matrix[step] )
       {
            if( n.noteOn )
            {
                instrument.NoteOn(n.note);
            }
            else
                instrument.NoteOff(n.note);
        }

       step += 1;
       if (step >= matrix.Length)
           step = 0;
    }

}
