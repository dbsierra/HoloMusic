using UnityEngine;
using System.Collections;

/// <summary>
/// Container that houses components for the FM Synthesizer. Gets wrapped in a SynthInterface and
/// instanced when needed for each voice of the final synthesizer
/// </summary>
public class FMSynthContainer  {

    MusicUtil.MIDINote note;

    EnvelopeGenerator eg;
    FMSynthEngine fmOsc;

    //public float freq;
    public float Attack;
    public float Decay;
    public float Sustain;
    public float Release;
    public float ModIndex;
    public float ModFreq;

    private bool noteOn;
    private bool noteOff;

    private uint duration;

    bool playing;
    public bool Playing { get { return playing; } }

    float sample;

    float time;

    public string name;

	public FMSynthContainer(string n)
    {
        name = n;
        fmOsc = new FMSynthEngine();
        eg = new EnvelopeGenerator(n);
    }
    bool go;
    public void NoteOn(MusicUtil.MIDINote note)
    {
        this.note = note;

        playing = true;
        go = true;
        fmOsc.NoteOn(note.frequency);
        fmOsc.modIndex = ModIndex;
        fmOsc.ModFreq = ModFreq;
        eg.GateOpen();
        eg.Attack = Attack;
        eg.Decay = Decay;
        eg.Sustain = Sustain;
        eg.Release = Release;
        time = 0;
        //Debug.Log("note on " + note.frequency + " time: " + time);
        duration = (uint)(note.duration * MusicUtil.MusicUtil.BeatLength * MusicUtil.MusicUtil.SampleRate);
        //Debug.Log(MusicUtil.MusicUtil.BeatLength + " ");
    }
    /*
    public void NoteOn(float freq)
    {
        noteOn = true;
        noteOff = false;
        fmOsc.NoteOn(freq);
        fmOsc.modIndex = ModIndex;
        fmOsc.ModFreq = ModFreq;
        eg.GateOpen();
        eg.Attack = Attack;
        eg.Decay = Decay;
        

    }*/
    public void NoteOff()
    {
        noteOff = true;
 
        eg.GateClose();
        //Debug.Log("note off " + note.frequency + " time: " + time);
    }

    public void StopPlaying()
    {
        playing = false;
    }


    int sampleCount = 0;
    int startTime = 0;
    public float GetNextSample()
    {
        if( playing )
        {
            time ++;
            if( time >= duration)
            {
                NoteOff();
            }
            sample = fmOsc.GetSample() * eg.GetSample();

            if ( eg.CurState == EnvelopeGenerator.State.off )
            {
                Debug.Log("Stopped playing at: " + MasterClock.Instance.MyTime);
                StopPlaying();
            }
                
        }

        if( go )
        {
            if (sampleCount - startTime >= 1000)
            {
                //Debug.Log(name + " " + env);
                Debug.Log(name + " " + playing);
                startTime = sampleCount;
            }
        }

        sampleCount++;
        //if( eg.Playing )



        return sample;
    }

}
