using UnityEngine;
using System.Collections;
using MusicUtilities;
using MusicDevice;

public class UIFMSynthesizer : MonoBehaviour {

    FMSynthesizer fm;

    [Header("Envelope1")]
    [Range(0.001f, 5)]
    public float Attack1;
    [Range(0.001f, 5)]
    public float Decay1;
    [Range(0, 1)]
    public float Sustain1;
    [Range(0.001f, 5)]
    public float Release1;

    [Header("Envelope2")]
    [Range(0.001f, 5)]
    public float Attack2;
    [Range(0.001f, 5)]
    public float Decay2;
    [Range(0, 1)]
    public float Sustain2;
    [Range(0.001f, 5)]
    public float Release2;

    [Header("Modulator Index")]
    public float mod1Index;
    public float mod2Index;
    public float mod3Index;

    [Header("Modulator Ratio")]
    public float mod1Ratio;
    public float mod2Ratio;
    public float mod3Ratio;

    private bool ready;


    void Start () {
        fm = new FMSynthesizer(this, 6);
        UpdateParams();
        ready = true;

    }
	
    void UpdateParams()
    {
        fm.Attack1 = Attack1;
        fm.Attack2 = Attack2;
        fm.Decay1 = Decay1;
        fm.Decay2 = Decay2;
        fm.Sustain1 = Sustain1;
        fm.Sustain2 = Sustain2;
        fm.Release1 = Release1;
        fm.Release2 = Release2;
        fm.mod1Index = mod1Index;
        fm.mod1Ratio = mod1Ratio;
        fm.mod2Index = mod2Index;
        fm.mod2Ratio = mod2Ratio;
        fm.mod3Index = mod3Index;
        fm.mod3Ratio = mod3Ratio;
    }

	void Update () {
       // UpdateParams();
	}

    public void NoteOn(MIDINote n)
    {
        fm.NoteOn(n);
    }

    public void NoteOff(MIDINote n)
    {
        fm.NoteOff(n);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if( ready )
        {
            for (int i = 0; i < data.Length; i = i + channels)
            {
                float s = fm.NextSample();

                data[i] = .1f * s;

                //if we are in stereo, duplicate the sample for L+R channels
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }
            }
        }

    }


}
