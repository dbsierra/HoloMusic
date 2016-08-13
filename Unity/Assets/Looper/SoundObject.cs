using UnityEngine;
using System.Collections;

public class SoundObject : MonoBehaviour {

    public GameObject Visuals;

    Microphone mic;

    AudioClip clip;
    float[] clipData;

    AudioClip recordedClip;
    float[] newClipData;

    AudioSource asource;
    bool recording;

    float recordingTimer;

    int maxRecordingTime = 15;

    bool muted;

    public int Index;
  

    void Awake()
    {

    }

	// Use this for initialization
	void Start () {
        mic = new Microphone();

        //foreach (string s in Microphone.devices)
        //    Debug.Log(s);

        asource = GetComponent<AudioSource>();
        Visuals.SetActive(false);
	}
	
    public void Enter()
    {
        Visuals.SetActive(true);
    }
    public void Exit()
    {
        Visuals.SetActive(false);
    }

    public void MutePlayback()
    {
        if (muted)
        {
            asource.volume = 0;
        }
        else
            asource.volume = 1;
        muted = !muted;
    }

    public void SoloPlayback()
    {

    }

    private AudioClip TransferClips( AudioClip sourceClip, float clipLength )
    {
        float[] source = new float[sourceClip.samples];
        sourceClip.GetData(source, 0);
        int lengthInSamples = (int)(clipLength * AudioSettings.outputSampleRate);

        //ensure it doesn't surpass the source clip size
        lengthInSamples = Mathf.Min(lengthInSamples, source.Length);

        float[] target = new float[lengthInSamples];

        for( int i=0; i < lengthInSamples; i++ )
        {
            target[i] = source[i];
        }

        AudioClip targetClip = AudioClip.Create("Recording", lengthInSamples, sourceClip.channels, AudioSettings.outputSampleRate, false);
        targetClip.SetData(target, 0);
        return targetClip;
    }

	void Update ()
    {

        if( Input.GetKeyDown(KeyCode.R) )
        {
            if (!recording)
            {
                StartRecording();
            }
            else if (recording)
            {
                StopRecording();
            }
        }

        if( recording )
        {   
            recordingTimer += Time.deltaTime;

            if (recordingTimer >= maxRecordingTime)
                StopRecording();
        }

	}

    public void StartRecording()
    {
        Debug.Log("Begin Recording");
        recording = true;
        clip = Microphone.Start(Microphone.devices[0], false, maxRecordingTime, AudioSettings.outputSampleRate);
        recordingTimer = 0;
    }

    public void StopRecording()
    {
        Debug.Log("Stop Recording");
        recording = false;
        Microphone.End(Microphone.devices[0]);
        clipData = new float[clip.samples];
        clip.GetData(clipData, 0);

        asource.clip = TransferClips(clip, recordingTimer); ;
        asource.Play();
    }
    
}
