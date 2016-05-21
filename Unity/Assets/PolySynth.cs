using UnityEngine;
using System.Collections;

public class PolySynth {

	#region Voice struct object
	private struct Voice
	{
		public bool streaming;
		public float startTime;
		public float env;
		public float freq;
		public float timeSincePress;
		public float sampleValue;
	}
	//Initialize this voice
	private void VoiceTrigger(ref Voice v, float f, float sT)
	{
		v.startTime = sT;
		v.freq = f;
		v.env = 0;
		v.streaming = true;
		v.timeSincePress = 0;
	}
	//Calculate the envelope for a given voice. time is lookup value to curve
	private void VoiceEnvelope(ref Voice v)
	{
		float env = 0;
		float time = v.timeSincePress;

		if ( time <= Attack)
			env = time / Attack;
		
		else if (time > Attack && time <= Decay)
			env = 1 - ((time - Attack) / (Decay - Attack));   

		else
			env = 0;

		//finished
		if (time >= Attack + Decay)
		{
			env = 0;
		}
		v.env = env;
	}
	private Voice[] voices; //list of all voices
	private Voice v; //currently processed voice
	#endregion

	int activeCount;
	int oldestVoice;

	public float Attack;
	public float Decay;
	float a;
	public float FMModIndex;
	float fout;

	//constructor
	public PolySynth(){
		voices = new Voice[6];
		activeCount = oldestVoice = 0;
		Attack = 0.03f;
		Decay = .25f;
		samples = new float[voices.Length];
	}

	//called on every note event
	public void Trigger(float f, float t){

		//find first free voice and inititalize it
		if( activeCount < voices.Length )
		{

			for( int i=0; i < voices.Length; i++ )
			{	
				if( !voices[i].streaming )
				{
					VoiceTrigger(ref voices[i], f, t);
					activeCount++;

					//Debug.Log ( f + " -------------------------------------------------------- " + " note on");
					a = 1/(float)activeCount;
					//PrintLog();
					break;
				}
			}
		}
		else{
			//else replace oldest voice
			//Debug.Log ("-------------------------------------------------------- " + " note replace");
			//PrintLog();
			oldestVoice++;
			if( oldestVoice >= voices.Length )
				oldestVoice = 0;
			VoiceTrigger( ref voices[oldestVoice++], f, t );
		}
	}

	private bool rec;
	private WriteWav ww1;
	private WriteWav ww2;

	public float v1_sample;
	public float v2_sample;
	public float[] samples;

	public void Record()
	{
		if( !rec )
		{
			rec = true;
			ww1 = new WriteWav();
			ww2 = new WriteWav();

			ww1.StartWriting("v1");
			ww2.StartWriting("v2");
		}

	}
	public void StopRecord()
	{
		if( rec )
		{
			rec = false;
			//WriteWav.WriteHeader();

			ww1.WriteHeader();
			ww2.WriteHeader();
			Debug.Log("rec stop");
		}

	}
	
	//Called on every sample, only perform computations that absolutely need to be per-sample here!
	public float[] GetSample(float t)
	{
		fout = 0; 

		//for each voice, if streaming, return its sample
		if( activeCount > 0 )
		{
			for( int i=0; i< voices.Length; i++ )
			{
				samples[i] = voices[i].sampleValue;
				if( voices[i].streaming )
				{
					voices[i].timeSincePress = t - voices[i].startTime;
					
					VoiceEnvelope(ref voices[i]);
					voices[i].sampleValue = a * FM( voices[i].freq, t, FMModIndex * voices[i].freq ) * voices[i].env;
					fout += voices[i].sampleValue;

					if( voices[i].timeSincePress >= Attack + Decay )
					{
						voices[i].timeSincePress = 0;
						voices[i].streaming = false;
						activeCount--;
						//Debug.Log ( i + " " + voices[i].freq + " -------------------------------------------------------- " + " note off");
					}
				}

			}
		}

 	
		return samples;
	}
	
	#region Synth routines
	private float sin(float f, float t)
	{
		return Mathf.Sin(t * 2 * Mathf.PI * f);
	}
	private float saw(float t, float f)
	{
		return t * f - Mathf.Round(t * f);
	}
	private float FM(float f, float t, float i)
	{
		float modulator = Mathf.Sin(t * 2 * Mathf.PI * i );
		return Mathf.Sin(t * 2 * Mathf.PI * f + modulator);
	}
	#endregion
	
	public void PrintLog(){
		foreach( Voice v in voices )
		{
			Debug.Log (v.streaming + " - f:" + v.freq + " - env:" + v.env + " - tSP:" + v.timeSincePress + " - s:" + v.sampleValue + " - sT:" + v.startTime);
		}
	}

}
