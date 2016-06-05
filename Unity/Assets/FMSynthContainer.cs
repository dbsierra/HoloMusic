using UnityEngine;
using System.Collections;

/// <summary>
/// Container that houses components for the FM Synthesizer. Gets wrapped in a SynthInterface and
/// instanced when needed for each voice of the final synthesizer
/// </summary>
public class FMSynthContainer  {

    EnvelopeGenerator eg;
    FMSynthEngine fmOsc;

    public float freq;
    public float Attack;
    public float Decay;
    public float ModIndex;
    public float ModFreq;

    private bool noteOn;
    private bool noteOff;

    public bool Playing { get { return noteOn; } }

    float sample;

	public FMSynthContainer()
    {
        fmOsc = new FMSynthEngine();
        eg = new EnvelopeGenerator();
    }

    public void NoteOn(float freq)
    {
        noteOn = true;
        noteOff = false;
        fmOsc.NoteOn(freq);
        fmOsc.modIndex = ModIndex;
        fmOsc.ModFreq = ModFreq;
        eg.NoteOn();
        eg.Attack = Attack;
        eg.Decay = Decay;
    }
    public void NoteOff()
    {
        noteOff = true;
        noteOn = false;
        fmOsc.NoteOff();
        eg.NoteOff();
    }

    public float GetNextSample()
    {

        //if( eg.Playing )
        sample = fmOsc.GetSample() * eg.GetSample();


        return sample;
    }

}
