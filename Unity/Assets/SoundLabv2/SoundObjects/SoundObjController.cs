﻿using UnityEngine;
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

    //main thread bools
    bool bFinalizeAudioRecording;
    bool bInitializeAudioRecording;

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
        bInitializeAudioRecording = true;
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
        //Debug.Log("Pause");
        foreach (SoundObj so in SoundObjects)
        {
            so.Pause();
        }
    }
    void Play()
    {
        //Debug.Log("Play");
        foreach (SoundObj so in SoundObjects)
        {
            so.Play();
        }
    }
    void Stop()
    {
        //Debug.Log("Stop");
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

        //if first recording, use metronome


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
        bFinalizeAudioRecording = true;
    }
    IEnumerator WaitUntilFinishedStoringAudio()
    {
        curRecSoundObj.RecordFinish();

        Debug.Log("audio being stored");
        while ( curRecSoundObj.state != SoundObj.State.audioSecured )
            yield return null;

        Debug.Log("done storing audio");
        curRecSoundObj = null;

        GlobalTime.Instance.Play();

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

        if(bInitializeAudioRecording)
        {
            bInitializeAudioRecording = false;
            Debug.Log(curRecSoundObj);
            if (curRecSoundObj)
                curRecSoundObj.InitializeMic();
        }

        if( bFinalizeAudioRecording )
        {
            bFinalizeAudioRecording = false;
            state = State.storing;
            StartCoroutine(WaitUntilFinishedStoringAudio());
        }
    }
    //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
    #endregion




}
