using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;
using MusicDevice;

public class MasterClock : MonoBehaviour {

    public static MasterClock Instance;

    float time;
    public float MyTime;
    uint sample;
    byte step, oldStep;
    byte length = 16;  //in number of beats
    uint beatLength_s;

    Dictionary<MIDINote, Voice> test = new Dictionary<MIDINote, Voice>();

    public Sequencer[] sequencers;

	void Start () {
        Instance = this;

        MIDINote n1 = new MIDINote();
        MIDINote n2 = new MIDINote();
        FMVoice v1 = new FMVoice("hi");
        FMVoice v2 = new FMVoice("bye");

        test[n1] = v1;
        test[n2] = v2;

        Debug.Log(test[n1].nae);
        Debug.Log(test[n2].nae);

        synth = new FMSynthesizer();
        
        beatLength_s = (uint)(Settings.BeatLength * Settings.SampleRate);
    }

    FMSynthesizer synth;
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.A) )
        {
            MIDINote n = new MIDINote(48, 1);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {

        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        //for each sample of this block of audio data
        for (int i = 0; i < data.Length; i = i + channels)
        {
            MyTime += Settings.inc;
            if ( sample >= beatLength_s )
            {
                sample = 0;
                foreach (Sequencer s in sequencers)
                {
                    s.OnStep(step);
                }
                step++;
                if (step >=length)
                    step = 0;
            }
            sample += 1;


        }


    }

}
