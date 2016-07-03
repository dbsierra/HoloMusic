using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using MusicUtilities;

public class SynthManager : MonoBehaviour
{
    #region WAV variables
	private bool recOutput;
	WriteWav wavWriter;
	WriteWav wavWriter2;
    #endregion

	#region Tunables
    [Range(1, 8)]
    public float speed = 2f;
    [Range(0,1)]
    public float gain = 0.05f;
    [Range(0.001f, .3f)]
    public float attack = .1f;
    [Range(0.1f, 5f)]
    public float decay = .5f;
    [Range(0,1)]
    public float sustain = .5f;
    [Range(0.1f, 5f)]
    public float release = .1f;
    [Range(0,1)]
	public float FMIndex;
    [Range(0, 6)]
    public float FMFreq;
    [Range(1, 16)]
    public byte VoiceCount;
    #endregion

    private float sampling_frequency;
    private int sample = 0;

    private int oldStep = -1;
    private int step = 0;

    private float voiceAttenuator;
    private FMSynthContainer[] voices;
    private byte overflowCounter;
    private float finalSample;

    void Start()
    {
		wavWriter = new WriteWav();
        sampling_frequency = AudioSettings.outputSampleRate;

        AudioSettings.GetDSPBufferSize(out wavWriter.bufferSize, out wavWriter.numBuffers);

        VoiceCount = (byte)Mathf.Min(VoiceCount, 16);

        voices = new FMSynthContainer[VoiceCount];

        for ( int i=0; i< voices.Length; i++ )
        {
            voices[i] = new FMSynthContainer("eg"+i);
            //Debug.Log(voices[i].name);
        }

        voiceAttenuator = 1f / voices.Length;
    }


    public void NoteOn(MIDINote n)
    {
       
        //find first free voice. If no free voices, assign to index of overflowCounter, and increment overflowCounter, 
        //reset to 0 if it exceeds voices.Length
        bool found = false;                                 
        for( byte i=0; i< voices.Length; i++ )
        {
            //Debug.Log(voices[i].name + " " + voices[i].Playing + " " + voices.Length);
            if ( !voices[i].Playing )
            {
               // Debug.Log(voices[i].name + " is now playing. " + i);
                found = true;
                voices[i].NoteOn(n);
                break;
            }
        }
        if( !found )
        {
            Debug.Log(overflowCounter);
            voices[overflowCounter++].NoteOn(n);
            if (overflowCounter >= voices.Length)
                overflowCounter = 0;
        }

        //Debug.Log("NoteOn " + n.frequency + " " + Time.time);

    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        //for each sample of this block of audio data
        for (int i = 0; i < data.Length; i = i + channels)
        {
            finalSample = 0;
            foreach (FMSynthContainer v in voices)
            {
                finalSample += v.GetNextSample();
                
            }

            //data[i] = gain * voices[0].GetNextSample();
            data[i] = gain * finalSample;// voices[0].GetNextSample();

            if (channels == 2)
            {
                data[i + 1] = data[i];
            }

            sample += 1;
        }


         if (recOutput)
         {
             wavWriter.ConvertAndWrite(data);
         }
    }

    void Update()
    {

        foreach (FMSynthContainer v in voices)
        {
            v.Attack = attack;
            v.Decay = decay;
            v.Sustain = sustain;
            v.Release = release;
            v.ModIndex = FMIndex;
            v.ModFreq = FMFreq;
        }

        
        if (Input.GetKeyDown("r"))
        {   
            if (recOutput == false)
            {
				wavWriter.StartWriting("s1.wav");
                recOutput = true;
            }
            else
            {
                recOutput = false;
				wavWriter.WriteHeader();
                Debug.Log("rec stop");
            }
        }
        
    }

   

}


