using UnityEngine;
using System.Collections;
using MusicDevice;
using MusicUtilities;
using System;

public class FMSynthesizer : Instrument {

    VoiceManager voiceManager;
    public UIFMSynthesizer UI;

    public string Name;

    //
    public float Attack1, Attack2;
    public float Decay1, Decay2;
    public float Sustain1, Sustain2;
    public float Release1, Release2;
    public float mod1Index, mod2Index, mod3Index;
    public float mod1Ratio, mod2Ratio, mod3Ratio;
    //

    public FMSynthesizer(UIFMSynthesizer UI = null, int noOfVoices = 6 )
    {
        Name = "hi";
        this.UI = UI;

        noOfVoices = Mathf.Clamp(noOfVoices, 2, 12);
        FMVoice[] voices = new FMVoice[noOfVoices];
        for (int i = 0; i < voices.Length; i++)
        {
            voices[i] = new FMVoice("voice" + i, this);
            voices[i].UpdateParams();
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
    public EnvelopeGenerator eg;
    public EnvelopeGenerator eg2;

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

    UIFMSynthesizer p;

    float mod1Index, mod2Index, mod3Index;
    float mod1Ratio, mod2Ratio, mod3Ratio;

    public void UpdateParams()
    {
        eg.Attack = p.Attack1;
        eg.Decay = p.Decay1;
        eg.Sustain = p.Sustain1;
        eg.Release = p.Release1;
        eg2.Attack = p.Attack2;
        eg2.Decay = p.Decay2;
        eg2.Sustain = p.Sustain2;
        eg2.Release = p.Release2;
        mod1Index = p.mod1Index;
        mod2Index = p.mod2Index;
        mod3Index = p.mod3Index;
        mod1Ratio = p.mod1Ratio;
        mod2Ratio = p.mod2Ratio;
        mod3Ratio = p.mod3Ratio;
    }

    public FMVoice(string name, FMSynthesizer parent)
    {
        Name = name;
        p = parent.UI;

        eg = new EnvelopeGenerator(name);
        eg.Attack = p.Attack1;
        eg.Decay = p.Decay1;
        eg.Sustain = p.Sustain1;
        eg.Release = p.Release1;

        eg2 = new EnvelopeGenerator(name);
        eg2.Attack = p.Attack2;
        eg2.Decay = p.Decay2;
        eg2.Sustain = p.Sustain2;
        eg2.Release = p.Release2;


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


    public void SetParameters()
    {

    }

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

            float m3 = Mathf.Sin(baseFreq * mod3Ratio) * mod3Index;
            float m2 = Mathf.Sin(baseFreq * mod2Ratio + m3) * mod2Index;
            float m1 = Mathf.Sin(baseFreq * mod1Ratio + m2 ) * mod1Index * eg2.GetSample();
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