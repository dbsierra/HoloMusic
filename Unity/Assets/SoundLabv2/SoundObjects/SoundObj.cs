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

        done = true;
        state = State.ready;
    }

    public void OnDisable()
    {
        GlobalTime.EnterPickup -= OnPickupEnter;
        GlobalTime.EnterRecording -= OnRecordEnter;
        GlobalTime.ExitRecording -= OnRecordExit;
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
        micIncomingClip = Microphone.Start(Microphone.devices[0], false, (int)( GlobalTime.Instance.MaxRecordingSteps * Settings.BeatLength + (16*Settings.BeatLength) ), AudioSettings.outputSampleRate);
    }
    public void RecordFinish()
    {
        
        state = State.ready;
        controller.RecordStop();
        Microphone.End(Microphone.devices[0]);
        micFinalClip = micIncomingClip;
        audioSource.clip = micFinalClip;
        audioSource.Play();
    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i = i + channels)
        {
            //if we are in stereo, duplicate the sample for L+R channels
            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
        }
    }


}
