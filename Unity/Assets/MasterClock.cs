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

   // public Sequencer[] sequencers;

	void Start () {
        Instance = this;

        synth = new FMSynthesizer();
        
        beatLength_s = (uint)(Settings.BeatLength * Settings.SampleRate);
    }

    FMSynthesizer synth;
    MIDINote n;
    bool on;
    void Update()
    {

    }

    /*
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
    */


}
