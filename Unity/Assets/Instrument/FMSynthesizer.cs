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
        float o = 0;



        return o;
    }

}


public class FMVoice : Voice
{
    EnvelopeGenerator eg;
    VoiceManager parentManager;
    public string nae { get; set; }
    float t;

    public FMVoice(VoiceManager v)
    {
        parentManager = v;
       // this.instrument = instrument;
    }
    public  void NoteOn(MIDINote n)
    {
    }
    public  void NoteOff()
    {
    }
    public  float NextSample()
    {
        t += Settings.inc;
        return Mathf.Sin(Mathf.PI * 2 * 220);
    }
    public void Done()
    {

    }
}