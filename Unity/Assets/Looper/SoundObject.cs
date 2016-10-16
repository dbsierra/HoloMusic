using UnityEngine;
using System.Collections;
using MusicUtilities;

[RequireComponent(typeof(GUITexture))]
public class SoundObject : MonoBehaviour {

    //root for the visuals of this sound object
    public GameObject Visuals;  

    //Audio In
    Microphone mic; 

    //audio clip and it's corresponding sample array that recieves and stores the incoming microphone data
    AudioClip clip;
    float[] clipData;
    AudioSource asource;

    bool recording;
    bool pickup;
    float recordingTimer;   //time of recording in seconds
    int recordingStartSample;  //start sample of the portion we want to keep
    int recordingEndSample;    //end sample of the portion we want to keep
    int maxRecordingTime = 15;

    bool muted;

    //The index in the array of sound objects that exist, used by RootController
    public int Index;


    int step;



    void OnEnable()
    {
        MasterClock.OnStep += OnStep;
    }
    void OnDisable()
    {
        MasterClock.OnStep -= OnStep;
    }

	void Start () {
        mic = new Microphone();
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
        //fill the array for the incoming audio clip that was recorded
        float[] source = new float[sourceClip.samples];
        sourceClip.GetData(source, 0);

        //What is the max length of the new clip going to be? Ensure it doesn't surpass the source clip size
        int maxLengthInSamples = (int)(clipLength * AudioSettings.outputSampleRate);
        maxLengthInSamples = Mathf.Min(maxLengthInSamples, source.Length);


        int lengthInSamples = recordingEndSample - recordingStartSample;
        float[] target = new float[lengthInSamples];

        Debug.Log(recordingEndSample + " - " + recordingStartSample + " = " + lengthInSamples + " " +  source.Length );

        for( int i=0; i < lengthInSamples; i++ )
        {
            target[i] = source[i + recordingStartSample];
        }

        AudioClip targetClip = AudioClip.Create("Recording", lengthInSamples, sourceClip.channels, AudioSettings.outputSampleRate, false);
        targetClip.SetData(target, 0);

        DrawAudioWave.Instance.FillTexture(target);

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


    void OnStep(int step)
    {
        this.step = step;

        if(recording)
        {
            Debug.Log(step);
            if( step == 16 )
            {
                recordingStartSample = MasterClock.Instance.LocalSample;
                Debug.Log("start: " + recordingStartSample);
            }
            if( step == 48 )
            {
                recordingEndSample = MasterClock.Instance.LocalSample;
                Debug.Log("end: " + recordingEndSample);
            }
        }
    }

    int globalSample = 0;
    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i = i + channels)
        {
            if (recording)
            {
                ;
            }
            else
            {
                if ( step % 32 == 0 )
                    clipSampleCounter = 0;

                if (clipSampleCounter < clipData.Length)
                    data[i] = clipData[clipSampleCounter++];
            }

            // data[i] = (Mathf.Sin( testf * tao * ((float)globalSample/(float)Settings.SampleRate) )/2+.5f) * test * data[i];
            data[i] = Mathf.Sin(( 440 * 2 * Mathf.PI * globalSample / Settings.SampleRate ) + data[i]*10f ) * 1f * data[i];

            globalSample++;
            //if we are in stereo, duplicate the sample for L+R channels
            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
        }
    }

    int clipSampleCounter;

    public void StartRecording()
    {
        recordingTimer = 0;
        step = 0;
        recordingStartSample = recordingEndSample = 0;

        MasterClock.Instance.RecordWithPickup();
        clip = Microphone.Start(Microphone.devices[0], false, 16 * Settings.BeatLength_s, AudioSettings.outputSampleRate);
        
        pickup = true;
        recording = true;
    }

    public void StopRecording()
    { 

        MasterClock.Instance.MetronomeOn = false;
        Microphone.End(Microphone.devices[0]);

        clipData = new float[clip.samples];
        clip.GetData(clipData, 0);
        asource.clip = TransferClips(clip, recordingTimer);

        //replace the new clipData with the desired portion of audio
        clipData = new float[asource.clip.samples];
        asource.clip.GetData(clipData, 0);
       
        FinalizeRecording();
    }

    void FinalizeRecording()
    {
        step = 0;
        clipSampleCounter = 0;
        Debug.Log("finished on step: " + step);

        MasterClock.Instance.Reset();
        recording = false;

        asource.Play();
    }
    
}
