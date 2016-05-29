using UnityEngine;
using System.Collections;

/// <summary>
/// Container that houses components for the FM Synthesizer
/// </summary>
public class FMSynthContainer  {

    EnvelopeGenerator eg;
    FMSynthEngine fmOsc;

    public float freq;
    public float Attack;
    public float Decay;
    public float ModIndex;

    private bool noteOn;
    private bool noteOff;

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

    public float GetSample()
    {

        sample = fmOsc.GetSample() * eg.GetSample();


        return sample;
    }

}
