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
        micIncomingClip = Microphone.Start(Microphone.devices[0], false, (int)Mathf.Ceil( GlobalTime.Instance.MaxRecordingSteps * Settings.BeatLength + (16*Settings.BeatLength) ), AudioSettings.outputSampleRate);
    }
    public void RecordStart()
    {
        state = State.recording;
    }
    public void RecordFinish()
    {
        state = State.storing;

        Microphone.End(Microphone.devices[0]);

        //declare and initialize array of incoming mic samples
        micAudioSamples = new float[micIncomingClip.samples];
        micIncomingClip.GetData(micAudioSamples, 0);

        finalAudioSamples = new float[micAudioSamples.Length];
        Debug.Log(recordingSampleLengthAll + " " + recordingSampleStart + " " + micAudioSamples.Length );
        int c = 0;
        for( int i=recordingSampleStart; i< micAudioSamples.Length; i++)
        {
            finalAudioSamples[c++] = micAudioSamples[i];
        }

        //Debug.Log(recordingSampleLengthUse + " " + micAudioSamples.Length);

        //create the final clip to be stored in this sound object and set the data to incoming mic samples
        //micFinalClip = AudioClip.Create("Recording", (GlobalTime.Instance.MaxRecordingSteps * Settings.BeatLength_s + (16 * Settings.BeatLength_s)), 1, Settings.SampleRate, false);
        //micFinalClip.SetData(micAudioSamples, 0);
        //audioSource.clip = micFinalClip;

        Object.Destroy(recordingSession.gameObject);
        audioElement.gameObject.SetActive(true);

        Reset();

        state = State.audioSecured;
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



 
    #region Execution logic
    //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
    void OnAudioFilterRead(float[] data, int channels)
    {
        //Debug.Log(state);

        if ( transportState == TransportState.playing && state == State.audioSecured )
        {
            //grab whatever global sample we're currently at and go from there
            int s = GlobalTime.Instance.GlobalSample;

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
            for (int i = 0; i < data.Length; i += channels)
            {
                recordingSampleLengthAll++;
            }
        }

    }

    void Update()
    {
       // Debug.Log(audioSource.isPlaying);
    }
    //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
    #endregion
   
}
