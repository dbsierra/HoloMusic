using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;
using MusicDevice;


/// <summary>
/// Sends step events and keeps track of the current global step of the composition
/// </summary>
public class MasterClock : MonoBehaviour {

    public static MasterClock Instance;

    float time;
    uint sample;
    
    int localSample;
    public int LocalSample { get { return localSample; } }

    int step;

    uint maxBars = 4;
    uint barLength = 16;
    uint maxSteps;

    public delegate void StepAction(int step);
    public static event StepAction OnStep;

    //Metronome
    AudioSource audioSource;
    AudioClip metroClip;
    float[] metroSamples;
    int metroSampleCounter;
    int metroAudioLength;
    int metroRepetition = 4; //4 means quarter note, so metronome will repeat on each quarternote

    public bool MetronomeOn;

    int beatLength_s;

    bool ready;
    public bool Ready { get { return ready; } }

    void Start () {
        Init();
    }

    void Init()
    {
        //Init variables
        Instance = this;
        maxSteps = maxBars * barLength;
        beatLength_s = Settings.BeatLength_s;

        //Init metronome samples array
        audioSource = GetComponent<AudioSource>();
        metroClip = audioSource.clip;
        metroAudioLength = metroClip.samples;
        metroSamples = new float[metroAudioLength];
        metroClip.GetData(metroSamples, 0);

        ready = true;
    }

    void Update()
    {
        if( !MetronomeOn )
        {
            //ensures that if it wakes up between steps it doesn't play till the next step
            metroSampleCounter = metroAudioLength;  
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (ready)
        {
            //for each sample of this block of audio data
            for (int i = 0; i < data.Length; i = i + channels)
            {
                if (sample >= beatLength_s)
                {
                    //if step exceeds max, wrap around
                    if (step >= maxSteps)
                    {
                        step = 0;
                        localSample = 0;
                    }
                        
                    //Fire step event
                    if (OnStep != null)
                        OnStep(step);

                    //inc and reset
                    step++;
                    sample = 0;
                }
                else
                    sample += 1;

                //metronome samples reset
                if( step % metroRepetition == 0 )
                    metroSampleCounter = 0;

                //play a sample from the metronome if on
                if(MetronomeOn)
                    if ( metroSampleCounter < metroAudioLength )
                        data[i] = metroSamples[metroSampleCounter++];


                localSample++;

                //if we are in stereo, duplicate the sample for L+R channels
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }
            }
        }
    }
    

    public void Reset()
    {
        step = 0;
        sample = 0;
        localSample = 0;
        metroSampleCounter = 0;
    }

    /// <summary>
    /// Reset the step sequence, start with an N bar pickup, then begin the step sequence
    /// </summary>
    public void RecordWithPickup()
    {
        Reset();
        MetronomeOn = true;
    }

}
