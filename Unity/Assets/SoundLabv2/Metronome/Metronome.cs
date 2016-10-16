using UnityEngine;
using System.Collections;

namespace Timeline
{
    public class Metronome : MonoBehaviour
    {
        public AudioClip FirstHit;
        float[] FirstHitSamples;
        public AudioClip SecondHit;
        float[] SecondHitSamples;
        public AudioClip puHit1;
        float[] puHit1Samples;
        public AudioClip puHit2;
        float[] puHit2Samples;
        public AudioClip puHit3;
        float[] puHit3Samples;
        public AudioClip puHit4;
        float[] puHit4Samples;

        int sample;
        int currentBeat;

        public bool pickup;

        // Use this for initialization
        void Start()
        {
            Initialize();
        }
        public void Initialize()
        {
            currentBeat = -1;
            FirstHitSamples = new float[FirstHit.samples];
            SecondHitSamples = new float[SecondHit.samples];
            puHit1Samples = new float[puHit1.samples];
            puHit2Samples = new float[puHit2.samples];
            puHit3Samples = new float[puHit3.samples];
            puHit4Samples = new float[puHit4.samples];

            FirstHit.GetData(FirstHitSamples, 0);
            SecondHit.GetData(SecondHitSamples, 0);
            puHit1.GetData(puHit1Samples, 0);
            puHit2.GetData(puHit2Samples, 0);
            puHit3.GetData(puHit3Samples, 0);
            puHit4.GetData(puHit4Samples, 0);
        }

        public void Reset()
        {
            sample = 0;
            currentBeat = -1;
        }
        public void NextHit()
        {
            sample = 0;
            if (++currentBeat >= 4)
               currentBeat = 0;

            this.pickup = pickup;
        }

        public float NextSample()
        {
            float nextSample = 0;

            if (currentBeat == 0)
            {
                if (sample < FirstHitSamples.Length)
                    nextSample = FirstHitSamples[sample];
            }
            else
            {
                if (sample  < SecondHitSamples.Length)
                    nextSample = SecondHitSamples[sample];
            }

            if( this.pickup )
            {
                if( currentBeat == 0 )
                {
                    if (sample < puHit1Samples.Length)
                        nextSample += puHit1Samples[sample];
                }
                else if (currentBeat == 1)
                {
                    if (sample < puHit2Samples.Length)
                        nextSample += puHit2Samples[sample];
                }
                else if (currentBeat == 2)
                {
                    if (sample < puHit3Samples.Length)
                        nextSample += puHit3Samples[sample];
                }
                else if (currentBeat == 3)
                {
                    if (sample < puHit4Samples.Length)
                        nextSample += puHit4Samples[sample];
                }
                nextSample /= 2;
            }

            sample++;

            return nextSample;
            //if (++currentBeat >= 4)
            //   currentBeat = 0;
        }

    }

}