﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;

namespace MusicDevice
{
    

    /// <summary>
    /// Manages voices, routing the proper note to the corresponding voice instance, and mixes them all down to one final sample
    /// </summary>
    public class VoiceManager : IMIDIDevice
    {
        private int voiceCount;
        public int VoiceCount { get { return voiceCount; } }

        private Voice[] voiceList;
        private Dictionary<Voice, int> voiceIndexMapper = new Dictionary<Voice, int>();
        private LinkedList<int> voiceListIndex;
        byte currentActiveVoices;
        float totalAmp;

        private uint sampleTimer;
        private uint oldSample;

        public VoiceManager(Voice[] voices)
        {
            voiceCount = voices.Length;
            voiceList = voices;
            voiceListIndex = new LinkedList<int>();
            for( int i=0; i< voiceCount; i++)
            {
                voiceListIndex.AddLast(i);
                voiceIndexMapper[voices[i]] = i;
                voices[i].parentManager = this;
            }
            
        }

        public void InitVoiceManager(Voice[] voices)
        {

        }

        public void NoteOn(MIDINote n)
        {
            Voice v = GetNextVoice();
            
            v.NoteOn(n);
            n.voice = v;

            if (currentActiveVoices < voiceCount)
                currentActiveVoices++;
        }

        float startTime;
        float time;
        public void NoteOff(MIDINote n)
        {
            if (n.voice != null)
                n.voice.NoteOff();
            else
                Debug.LogError("Trying to turn off a note that was never assigned a voice - note with pitch index: " + n.pitchLetterIndex);
        }

        public float NextSample()
        {
            if(sampleTimer - oldSample >= 1000)
            {
                oldSample = sampleTimer = 0;

            }
            sampleTimer += 1;

            float s = 0;
            totalAmp = 0;
            foreach(Voice v in voiceList)
            {
                v.UpdateParams();
                s += v.NextSample();

                totalAmp += v.Gain;
            }
            if (totalAmp >= 1)
            {
                return s / totalAmp;
            }
            else
                return s;
        }

        private Voice GetNextVoice()
        {
            int index = voiceListIndex.First.Value;
            voiceListIndex.RemoveFirst();
            voiceListIndex.AddLast(index);
            return voiceList[index];
        }
        public void FinishVoice(Voice v)
        {
            int index = voiceIndexMapper[v];
            voiceListIndex.Remove(index);
            voiceListIndex.AddFirst(index);
            currentActiveVoices--;
        }
    }

}