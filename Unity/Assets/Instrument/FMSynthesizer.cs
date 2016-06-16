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

    public FMVoice(string name)
    {
        eg = new EnvelopeGenerator(name);
        eg.Attack = .01f;
        eg.Decay = .2f;
        eg.Sustain = .5f;
        eg.Release = .3f;
        Name = name;
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
            o = Mathf.Sin(Mathf.PI * 2 * n.frequency * t) * Amplitude;
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