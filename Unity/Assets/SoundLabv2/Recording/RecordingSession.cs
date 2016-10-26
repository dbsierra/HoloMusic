using UnityEngine;
using System.Collections;
using Timeline;

public class RecordingSession : MonoBehaviour {

    public SoundObj targetSoundObject;

    public GameObject tapToRecord;
    public GameObject tapToFinish;


	// Use this for initialization
	void Awake () {
        Initialize();
	}
	
    void Initialize()
    {

    }

	// Update is called once per frame
	void Update () {
	
	}

    public void OnTap()
    {
        if( targetSoundObject.state == SoundObj.State.ready )
        {
            //with pickup, stop first everything

            tapToRecord.SetActive(false);
            tapToFinish.SetActive(true);
            targetSoundObject.RecordInitialize();

        }
    }

}
