using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

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
    [Range(0.1f, 1f)]
    public float decay = .5f;
	[Range(0,2)]
	public float FMIndex;
    #endregion

    public Sequencer sequencer;
    private PolySynth ps;

    private float sampling_frequency;
    private float[] samples;
    private int sample = 0;

    private int oldStep = -1;
    private int step = 0;

    //private float[] d1;
    //private float[] d2;

    FMSynthContainer voice;

    void Start()
    {
		wavWriter = new WriteWav();
        sampling_frequency = AudioSettings.outputSampleRate;

        AudioSettings.GetDSPBufferSize(out wavWriter.bufferSize, out wavWriter.numBuffers);

		ps = new PolySynth();

        voice = new FMSynthContainer();
    }


    public void NoteOn(float f)
    {
        //ps.Trigger(f, t);

        voice.NoteOn(f);
    }

    public void NoteOff()
    {

    }

    float t;

    void OnAudioFilterRead(float[] data, int channels)
    {
		if( sequencer.Ready )
		{

            //d1 = new float[data.Length];
            //d2 = new float[data.Length];

            //for each sample of this block of audio data
            for (int i = 0; i < data.Length; i = i + channels)
	        {
				t = sample / sampling_frequency;

                //calculate step
                step = (int)Mathf.Floor(t * speed) % sequencer.Size;

				//TRIGGERED!
				if(step != oldStep)
				{
                    oldStep = step;
                    foreach (float f in sequencer.GetNotes(step))
	                {
                        ;// ps.Trigger(f, t);
	                }
				}
				//-----------

				//samples = ps.GetSample(t);
				//data[i] = gain * (samples[0] + samples[1] + samples[2] + samples[3]);

                data[i] = gain * voice.GetSample();

				//d1[i] = samples[0];
				//d2[i] = samples[1];	
	            if (channels == 2){
					data[i + 1] = data[i];
					//d1[i+1] = samples[0];
					//d2[i+1] = samples[1];
				}
	               
	            sample += 1;
	        }
		}

        if (recOutput)
        {
             wavWriter.ConvertAndWrite(data);
        }
    }

    void Update()
    {

		ps.Attack = attack;
		ps.Decay  = decay;
        ps.FMModIndex = FMIndex;

        voice.Attack = attack;
        voice.Decay = decay;
        voice.ModIndex = FMIndex;

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


