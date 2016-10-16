using UnityEngine;
using System.Collections;
using MusicUtilities;
using Timeline;

[RequireComponent(typeof(AudioSource))]
public class SoundObj : MonoBehaviour {

    private AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } }

    private bool done;
    public bool Done { get { return done; } }

    float[] micAudioSamples;
    AudioClip micIncomingClip;
    AudioClip micFinalClip;

    public SoundObjController controller;
    public GameObject RecordingSessionPrefab;
    public GameObject AudioObjectPrefab;

    //Main thread transport booleans
    bool Pause;
    bool Play;
    bool Stop;

    public enum State
    {
        ready=0,
        recording=1,
        storing=2,
        active=3
    };
    public State state;

    void Awake()
    {
        Initialize();
    }
    void Initialize()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        RecordingSession rs = GameObject.Instantiate(RecordingSessionPrefab).GetComponent<RecordingSession>();
        rs.targetSoundObject = this;

        GlobalTime.EnterPickup += OnPickupEnter;
        GlobalTime.EnterRecording += OnRecordEnter;
        GlobalTime.ExitRecording += OnRecordExit;

        GlobalTime.OnStop += OnStop;
        GlobalTime.OnPause += OnPause;
        GlobalTime.OnPlay += OnPlay;

        done = true;
        state = State.ready;
    }

    public void OnDisable()
    {
        GlobalTime.EnterPickup -= OnPickupEnter;
        GlobalTime.EnterRecording -= OnRecordEnter;
        GlobalTime.ExitRecording -= OnRecordExit;
    }

    void Update()
    {
        if( Pause )
        {
            Pause = false;
            audioSource.Pause();
        }
        else if (Stop)
        {
            Stop = false;
            audioSource.Stop();
        }
        else if(Play)
        {
            Play = false;
            audioSource.Play();
        }
    }

    public void OnStop()
    {
        Stop = true;
    }
    public void OnPause()
    {
        Pause = true;
    }
    public void OnPlay()
    {
        Play = true;
    }
    public void OnPickupEnter()
    {
        Debug.Log("Pickup");
    }
    public void OnRecordEnter()
    {
        Debug.Log("Recording");
    }
    public void OnRecordExit()
    {
        Debug.Log("Done Recording");
        RecordFinish();  
    }

    public void Record()
    {
        state = State.recording;
        controller.Record();
        micIncomingClip = Microphone.Start(Microphone.devices[0], false, (int)Mathf.Ceil( GlobalTime.Instance.MaxRecordingSteps * Settings.BeatLength + (16*Settings.BeatLength) ), AudioSettings.outputSampleRate);
        
    }

    public void RecordFinish()
    {  
        state = State.ready;
        
        Microphone.End(Microphone.devices[0]);

        //declare and initialize array of incoming mic samples
        micAudioSamples = new float[micIncomingClip.samples];
        micIncomingClip.GetData(micAudioSamples, 0);

        //create the final clip to be stored in this sound object and set the data to incoming mic samples
        micFinalClip = AudioClip.Create("Recording", (GlobalTime.Instance.MaxRecordingSteps * Settings.BeatLength_s + (16 * Settings.BeatLength_s)), 1, Settings.SampleRate, false);
        micFinalClip.SetData(micAudioSamples, 0);

        //audioSource.clip = micFinalClip;

        audioSource.Play();
        controller.RecordDone();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {

        //grab whatever global sample we're currently at and go from there
        int s = GlobalTime.Instance.GlobalSample;

        for (int i = 0; i < data.Length; i+=channels)
        {

            if (s < micAudioSamples.Length)
            {
                data[i] = micAudioSamples[s++];
            }

            //if we are in stereo, duplicate the sample for L+R channels
            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
        }
    }


}
