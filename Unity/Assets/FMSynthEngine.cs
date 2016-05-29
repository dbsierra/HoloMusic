using UnityEngine;
using System.Collections;

public class FMSynthEngine {

    float phase;
    float phaseInc;

    float angFreq;          //frequency
    public float modIndex;  //modulation index

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
    public void NoteOff()
    {
        noteOff = true;
        noteOn = false;
        phase = 0;
    }
    public void SetFrequency(float freq)
    {
        phaseInc = freq * MusicUtil.TWO_PI * MusicUtil.inc;
    }

    private float FM()
    {
        float modulator = Mathf.Sin(phase * modIndex);
        return Mathf.Sin(phase + modulator);
    }

    public float GetSample()
    {

        phase += phaseInc;

        //if (phase >= MusicUtil.TWO_PI)
        //phase = 0;

        return FM();

    }
}

