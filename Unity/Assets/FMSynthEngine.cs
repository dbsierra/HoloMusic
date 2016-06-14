using UnityEngine;
using System.Collections;
using MusicUtilities;

public class FMSynthEngine {

    float phase;
    float phaseInc;

    float angFreq;          //frequency
    public float modIndex;  //modulation index
    public float ModFreq;

    private bool noteOn;
    private bool noteOff;

    public FMSynthEngine()
    {

    }

    public void NoteOn(float freq)
    {
        noteOn = true;
        noteOff = false;
        SetFrequency(freq);
        phase = 0;
    }

    public void SetFrequency(float freq)
    {
        phaseInc = freq * Settings.TWO_PI * Settings.inc;
    }

    private float FM()
    {
        float modulator = modIndex * Mathf.Sin(phase * ModFreq);
        return Mathf.Sin(phase + modulator);
    }

    public float GetSample()
    {
        
        phase += phaseInc;


        return FM();

    }
}

