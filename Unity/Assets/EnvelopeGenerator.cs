using UnityEngine;
using System.Collections;


/// <summary>
/// Given a time value and a note on or off value, this will output a value between 0 and 1,
/// according to the values of ADSR. This value is usually meant to modulate something else,
/// often by multiplication.
/// </summary>
public class EnvelopeGenerator  {

    public float Attack; //Attack duration
    public float Decay; //Decay duration
    public float Sustain; //Sustain level (0 - 1)
    public float Release; //Release duration

    private float t;

    private bool noteOn;
    private bool noteOff;
    public bool Playing { get { return noteOn; } }

    private float env;

    public EnvelopeGenerator()
    {
    }

    public void NoteOn()
    {
        t = 0;
        noteOn = true;
        noteOff = false;
    }
    public void NoteOff()
    {
        if( noteOn )
        {
            noteOff = true;
        }
        noteOn = false;
    }

    public float GetSample()
    {
        if (noteOn)
        {
            
            t += MusicUtil.inc;


            if (t <= Attack)
                env = t / Attack;

            else if (t > Attack && t <= (Attack + Decay))
                env = (1 - (t - Attack)) / Decay + 1/Decay;

            else
                env = 0;

            //finished
            if (t >= Attack + Decay)
            {
                env = 0;
                NoteOff(); //temporary
            }
        }

        return env;
    }
}
