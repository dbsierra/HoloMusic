using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;
using Timeline;

public class SoundObjController : MonoBehaviour {

    private LinkedList<SoundObj> sounds;
    private const int MAX_SOUNDS = 5;

    public GameObject SoundObjPrefab;
    public AudioClip[] testClips;

    public enum State
    {
        ready = 0,
        creating = 1,
        recording = 2,
        storing = 3
    };
    public State state;

    void Start () {
        Initialize();


	}
	
    void Initialize()
    {
        sounds = new LinkedList<SoundObj>();

        state = State.ready;
    }

	// Update is called once per frame
	void Update () {

	    if( state == State.ready && Input.GetKeyDown(KeyCode.Q) )
        {
            CreateSoundObject();
        }
	}

    public void Record()
    {
        state = State.recording;
        GlobalTime.Instance.Record();
    }
    public void RecordDone()
    {
        state = State.ready;
        GlobalTime.Instance.Play();
    }

    void CreateSoundObject()
    {
        SoundObj soundObj = GameObject.Instantiate(SoundObjPrefab).GetComponent<SoundObj>();
        soundObj.controller = this;
        //soundObj.AudioSource.Play();
        sounds.AddLast(soundObj);
    }

}
