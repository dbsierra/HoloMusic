using UnityEngine;
using System.Collections;
using MusicDevice;
using MusicUtilities;
using System;

public class FMSynthesizer : Instrument {

    VoiceManager voiceManager;

    public FMSynthesizer()
    {
        
        FMVoice[] voices = new FMVoice[8];
        for( int i=0; i<voices.Length; i++)
        {
            voices[i] = new FMVoice("voice"+i);
        }
        voiceManager = new VoiceManager(voices);
        
    }

    /// <summary>
    /// Allocate this note to a voice for this instrument
    /// </summary>
    /// <param name="n">note</param>
	public override void NoteOn(MIDINote n)
    {
        //Debug.Log("On: " + n.midi);
        voiceManager.NoteOn(n);
    }

    public override void NoteOff(MIDINote n)
    {
        //Debug.Log("Off: " + n.midi);
        voiceManager.NoteOff(n);
    }
    public override float NextSample()
    {
        return voiceManager.NextSample();
    }

}


public class FMVoice : Voice
{
    EnvelopeGenerator eg;
    public VoiceManager parentManager { get; set; }
    public float Amplitude { get; set; }
    public string Name { get; set; }
    float t;
    private bool processing;
    public bool Processing { get { return processing; } }
    MIDINote n;

    private float TWO_PI = 6.283185307f;

    private float[] table;
    private int index;

    public FMVoice(string name)
    {
        eg = new EnvelopeGenerator(name);
        eg.Attack = .01f;
        eg.Decay = .2f;
        eg.Sustain = .5f;
        eg.Release = .8f;
        Name = name;

        table = new float[Settings.SampleRate / 220];

        for( int i=0; i< table.Length; i++ )
        {
            table[i] = Mathf.Sin(TWO_PI * ((float)i / (float)(table.Length - 1))) * .8f;// + (Mathf.Pow( UnityEngine.Random.Range(0,1), 5 )*2-1);
            //Debug.Log(table[i]);
        }
        index = 0;
    }
    public void NoteOn(MIDINote n)
    {
        this.n = n;
        eg.GateOpen();
        processing = true;
    }
    public void NoteOff()
    {
        eg.GateClose();
        //Done();
    }


    public  float NextSample()
    {
        float o = 0;
        if (processing)
        {
            t += Settings.inc;

            Amplitude = eg.GetSample();

            /*
            float m3 = Mathf.Sin(TWO_PI * n.frequency * 7.5f * t) * 1f;
            float m2 = Mathf.Sin(TWO_PI * n.frequency * 4.5f * t + m3) * .2f; 
            float m1 = Mathf.Sin(TWO_PI * n.frequency * 2f * t + m2) * .5f; 
            o = Mathf.Sin(TWO_PI * n.frequency * t + m1   ) * Amplitude;
            */

            float m3 = Mathf.Sin(TWO_PI * n.frequency * 7.5f * t) * .3f;
            float m2 = Mathf.Sin(TWO_PI * n.frequency * 4f * t + m3) * .2f;
            float m1 = Mathf.Sin(TWO_PI * n.frequency * 2f * t + m2) * .5f;
            o = Mathf.Sin(TWO_PI * n.frequency * t + m1) * Amplitude;

            //o = table[index++] * Amplitude;
           // if (index >= table.Length)
            //    index = 0;

            //Debug.Log("hup: " + o);

            if (eg.CurState == EnvelopeGenerator.State.off)
            {
                Done();
                
            }
                
        }
        return o;
    }
    public void Done()
    {
        processing = false;
        parentManager.FinishVoice(this);
    }
}