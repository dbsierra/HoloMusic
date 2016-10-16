using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;
using Timeline;

public class SoundObjController : MonoBehaviour {

    private LinkedList<SoundObj> SoundObjects;
    private const int MAX_SOUNDS = 5;

    public GameObject SoundObjPrefab;
    public AudioClip[] testClips;

    public enum State
    {
        ready = 0,
        creating = 1,
        recordInit = 2,
        recording = 3,
        storing = 4
    };
    public State state;

    public bool RecordWithPickup;

    private SoundObj curRecSoundObj;

    #region Initializers
    //------------------------------------------------------------------------------//
    void Start () {
        Initialize();
	}
    void Initialize()
    {
        SoundObjects = new LinkedList<SoundObj>();

        GlobalTime.EnterPickup += OnPickupEnter;
        GlobalTime.EnterRecording += OnRecordEnter;
        GlobalTime.ExitRecording += OnRecordExit;
        GlobalTime.OnStop += OnStop;
        GlobalTime.OnPause += OnPause;
        GlobalTime.OnPlay += OnPlay;

        state = State.ready;
    }
    public void OnDisable()
    {
        GlobalTime.EnterPickup -= OnPickupEnter;
        GlobalTime.EnterRecording -= OnRecordEnter;
        GlobalTime.ExitRecording -= OnRecordExit;
        GlobalTime.OnStop -= OnStop;
        GlobalTime.OnPause -= OnPause;
        GlobalTime.OnPlay -= OnPlay;
    }
    void CreateSoundObject()
    {
        SoundObj soundObj = GameObject.Instantiate(SoundObjPrefab).GetComponent<SoundObj>();
        soundObj.controller = this;
        SoundObjects.AddLast(soundObj);
    }
    //------------------------------------------------------------------------------//
    #endregion




    #region Event Callbacks
    //------------------------------------------------------------------------------//
    public void OnStop()
    {
        Stop();
    }
    public void OnPause()
    {
        Pause();
    }
    public void OnPlay()
    {
        Play();
    }
    public void OnPickupEnter()
    {
    }
    public void OnRecordEnter()
    {
        Record();
    }
    public void OnRecordExit()
    {
        RecordDone();
    }
    //------------------------------------------------------------------------------//
    #endregion



    #region Transport functions
    //------------------------------------------------------------------------------//
    void Pause()
    {
        Debug.Log("Pausing");
        foreach (SoundObj so in SoundObjects)
        {
            so.Pause();
        }
    }
    void Play()
    {
        Debug.Log("Playing");
        foreach (SoundObj so in SoundObjects)
        {
            so.Play();
        }
    }
    void Stop()
    {
        Debug.Log("Stopping");
        foreach (SoundObj so in SoundObjects)
        {
            so.Stop();
        }
    }
    //------------------------------------------------------------------------------//
    #endregion




    #region Recording functions
    //------------------------------------------------------------------------------//
    public void RecordInitialize( SoundObj soundObj )
    {
        state = State.recordInit;
        curRecSoundObj = soundObj;

        //with pickup, stop first everything
        GlobalTime.Instance.Stop();
        GlobalTime.Instance.Record();
    }
    public void Record()
    {
        state = State.recording;
        curRecSoundObj.RecordStart();
    }
    public void RecordDone()
    {
        state = State.storing;

        curRecSoundObj.RecordFinish();

        GlobalTime.Instance.Play();

        curRecSoundObj = null;

        state = State.ready;
    }
    //------------------------------------------------------------------------------//
    #endregion




    #region Execution logic 
    //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
    void Update () {

	    if( state == State.ready && Input.GetKeyDown(KeyCode.Q) )
        {
            CreateSoundObject();
        }
    }
    //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
    #endregion




}
