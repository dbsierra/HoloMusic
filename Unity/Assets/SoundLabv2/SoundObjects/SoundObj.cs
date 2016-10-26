using UnityEngine;
using System.Collections;
using MusicUtilities;
using Timeline;

[RequireComponent(typeof(AudioSource))]
public class SoundObj : MonoBehaviour {

    private AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } }

    float[] micAudioSamples;
    float[] finalAudioSamples;
    AudioClip micIncomingClip;
    AudioClip micFinalClip;

    public SoundObjController controller;
    public GameObject RecordingSessionPrefab;
    RecordingSession recordingSession;
    public GameObject AudioObjectPrefab;
    AudioElement audioElement;

    bool audioStorageComplete;

    string MicrophoneDevice; 

    //main thread bools
    bool bExitRecordingAnim;

    public enum TransportState
    {
        paused = 0,
        playing = 1,
        stop = 2
    }
    public enum State
    {
        ready=0,
        recordingInit=1,
        recording=2,
        storing=3,
        audioSecured=4
    };
    public State state;
    public TransportState transportState;



    #region Intializers
    //------------------------------------------------------------------------------//
    void Awake()
    {
        Initialize();
    }
    void Initialize()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        recordingSession = GameObject.Instantiate(RecordingSessionPrefab).GetComponent<RecordingSession>();
        recordingSession.targetSoundObject = this;

        audioElement = GameObject.Instantiate(AudioObjectPrefab).GetComponent<AudioElement>();
        audioElement.gameObject.SetActive(false);
        audioElement.transform.localPosition = Vector3.zero;
        audioElement.transform.SetParent(transform, false);

        MicrophoneDevice = Microphone.devices[0];

        state = State.ready;
    }
    void Reset()
    {
        recordingSampleLengthAll = 0;
        recordingSampleStart = 0;
    }
    public void OnDisable()
    {
    }
    //------------------------------------------------------------------------------//
    #endregion



    #region event callbacks
    //------------------------------------------------------------------------------//
    //------------------------------------------------------------------------------//
    #endregion



    #region recording functions
    int recordingSampleLengthAll;
    int recordingSampleStart;
    //------------------------------------------------------------------------------//
    public void RecordInitialize()
    {
        state = State.recordingInit;
        controller.RecordInitialize(this);
    }
    public void InitializeMic()
    {
        micIncomingClip = Microphone.Start(MicrophoneDevice, false, (int)Mathf.Ceil(GlobalTime.Instance.MaxRecordingSteps * Settings.BeatLength + (16 * Settings.BeatLength)) + 1, AudioSettings.outputSampleRate);
        
    }
    uint startSample;
    public void RecordStart()
    {
        state = State.recording;
        startSample = GlobalTime.Instance.MasterSample;
    }
    public void RecordFinish()
    {
        Debug.Log("Hey " + (GlobalTime.Instance.MasterSample-startSample) );
        state = State.storing;

        FinalizeAudioRecording();

        Object.Destroy(recordingSession.gameObject);
        audioElement.gameObject.SetActive(true);

        Reset();

        state = State.audioSecured;

        //Debug.Log(recordingSampleLengthUse + " " + micAudioSamples.Length);

        //create the final clip to be stored in this sound object and set the data to incoming mic samples
        //micFinalClip = AudioClip.Create("Recording", (GlobalTime.Instance.MaxRecordingSteps * Settings.BeatLength_s + (16 * Settings.BeatLength_s)), 1, Settings.SampleRate, false);
        //micFinalClip.SetData(micAudioSamples, 0);
        //audioSource.clip = micFinalClip;
        Microphone.End(MicrophoneDevice);
        
    }
    void FinalizeAudioRecording()
    {
        //declare and initialize array of incoming mic samples

        micAudioSamples = new float[micIncomingClip.samples];
        micIncomingClip.GetData(micAudioSamples, 0);

        finalAudioSamples = new float[micAudioSamples.Length];
        /*
        //find first sample that is above .008
        int c = recordingSampleStart;
        for (int i = recordingSampleStart; i < micAudioSamples.Length; i++)
        {
            if( Mathf.Abs(micAudioSamples[i]) >= .008f )
            {
                break;
            }
            c++;
        }
        recordingSampleStart = c;
        */

        //WARNING WARNING HARD CODED NUMBER AHEAD, QUICK FIX
        //errrr....I notice there is a gap of ~ 7000 samples from when the mic starts recording to when the metronome hits
        recordingSampleStart += 7000;

        //Debug.Log(recordingSampleLengthAll + " " + recordingSampleStart + " " + micAudioSamples.Length);
        int c = 0;
        for (int i = recordingSampleStart; i < micAudioSamples.Length; i++)
        {
            finalAudioSamples[c++] = micAudioSamples[i];
        }

        Debug.Log("l: " + (micAudioSamples.Length - recordingSampleStart) );

        micFinalClip = AudioClip.Create("Recording", micAudioSamples.Length, 1, Settings.SampleRate, false);
        micFinalClip.SetData(micAudioSamples, 0);
        SavWav.Save("test.wav", micFinalClip);
        micFinalClip.SetData(finalAudioSamples, 0);
        SavWav.Save("test2.wav", micFinalClip);
    }
    //------------------------------------------------------------------------------//
    #endregion



    #region Transport controls
    //------------------------------------------------------------------------------//
    public void Play()
    {
        transportState = TransportState.playing;
    }
    public void Pause()
    {
        transportState = TransportState.paused;
    }
    public void Stop()
    {
        transportState = TransportState.stop;
    }
    //------------------------------------------------------------------------------//
    #endregion


    bool PrintSample = true;
    public bool Me;

    #region Execution logic
    //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
    //TODO
    //It is not really in sync, it's getting cut off before it can play the last samples
    void OnAudioFilterRead(float[] data, int channels)
    {
        //Debug.Log(state);

        if ( transportState == TransportState.playing && state == State.audioSecured )
        {
            //grab whatever global sample we're currently at and go from there
            int s = GlobalTime.Instance.SyncSample;
           // Debug.Log("my sample: " + s);
            for (int i = 0; i < data.Length; i += channels)
            {

                if (s < finalAudioSamples.Length)
                {
                    data[i] = finalAudioSamples[s++];
                }

                //if we are in stereo, duplicate the sample for L+R channels
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }
            }
        }
        if( state == State.recordingInit )
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                recordingSampleLengthAll++;
                recordingSampleStart++;
            }
        }
        else if (state == State.recording)
        {
            //Debug.Log("here it is: " + recordingSampleStart);
            for (int i = 0; i < data.Length; i += channels)
            {
                recordingSampleLengthAll++;
            }
        }

    }


    void Update()
    {
  

            if (bExitRecordingAnim)
        {
            bExitRecordingAnim = false;
        }
    }
    //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
    #endregion
   
}
