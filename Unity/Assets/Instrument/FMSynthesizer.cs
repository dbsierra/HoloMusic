using UnityEngine;
using System.Collections;
using MusicDevice;
using MusicUtilities;
using System;

public class FMSynthesizer : Instrument {

    VoiceManager voiceManager;

    public FMSynthesizer()
    {
        voiceManager = new VoiceManager(8);
        for(int i=0; i<8; i++)
        {
            voiceManager.AddVoice(new FMVoice(voiceManager));
        }
    }

    /// <summary>
    /// Allocate this note to a voice for this instrument
    /// </summary>
    /// <param name="n">note</param>
	public override void NoteOn(MIDINote n)
    {
        voiceManager.NoteOn(n);
    }

    public override void NoteOff(MIDINote n)
    {
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
    VoiceManager parentManager;
    public string nae { get; set; }
    float t;
    private bool processing;
    MIDINote n;

    public FMVoice(VoiceManager v)
    {
        parentManager = v;
       // this.instrument = instrument;
    }
    public void NoteOn(MIDINote n)
    {
        this.n = n;
        processing = true;
    }
    public void NoteOff()
    {
        Done();
    }
    public  float NextSample()
    {
        float o = 0;
        if (processing)
        {
            t += Settings.inc;
            o = Mathf.Sin(Mathf.PI * 2 * n.frequency * t);
            //Debug.Log("hup: " + o);
        }
        return o;
    }
    public void Done()
    {
        processing = false;
        parentManager.FinishVoice(this);
    }
}