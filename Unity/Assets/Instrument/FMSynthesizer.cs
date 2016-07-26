using UnityEngine;
using System.Collections;
using MusicDevice;
using MusicUtilities;
using System;

public class FMSynthesizer : Instrument {

    VoiceManager voiceManager;

    public FMSynthesizer()
    {
        
        FMVoice[] voices = new FMVoice[6];
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
        return  voiceManager.NextSample();
    }

}


public class FMVoice : Voice
{
    EnvelopeGenerator eg;
    EnvelopeGenerator eg2;

    public VoiceManager parentManager { get; set; }
    public float Gain { get; set; }
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
        eg.Decay = .3f;
        eg.Sustain = .5f;
        eg.Release = .2f;

        eg2 = new EnvelopeGenerator(name);
        eg2.Attack = .01f;
        eg2.Decay = .1f;
        eg2.Sustain =  0f;
        eg2.Release = .05f;


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
        eg2.GateOpen();
        processing = true;
        oldT = t;
    }
    public void NoteOff()
    {
        eg.GateClose();
        eg2.GateClose();
        //Done();
    }

    float oldT;

    public float NextSample()
    {
        float o = 0;
        if (processing)
        {
            t += 1;

            Gain = eg.GetSample();

            /*
            float m3 = Mathf.Sin(TWO_PI * n.frequency * 7.5f * t) * 1f;
            float m2 = Mathf.Sin(TWO_PI * n.frequency * 4.5f * t + m3) * .2f; 
            float m1 = Mathf.Sin(TWO_PI * n.frequency * 2f * t + m2) * .5f; 
            o = Mathf.Sin(TWO_PI * n.frequency * t + m1   ) * Amplitude;
            */

            float baseFreq = (TWO_PI * n.frequency * t) / Settings.SampleRate;

            float m3 = Mathf.Sin(baseFreq * 7f) * .3f;
            float m2 = Mathf.Sin(baseFreq * 3.3f + m3) * .9f;
            float m1 = Mathf.Sin(baseFreq * 1.9f + m2 ) * .5f * eg2.GetSample();
            o = Mathf.Sin(baseFreq + m1) * Gain;

            //o = Mathf.Sin( (TWO_PI * n.frequency * t) / Settings.SampleRate) * Gain;

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