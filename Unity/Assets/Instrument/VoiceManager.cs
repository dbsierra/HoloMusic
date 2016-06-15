using UnityEngine;
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
        private byte voiceCount;
        public byte VoiceCount { get { return voiceCount; } }
        private LinkedList<Voice> voices;
        private Dictionary<MIDINote, Voice> noteMapper;
        byte currentActiveVoices;

        public VoiceManager(byte vcount)
        {
            voiceCount = vcount;
            voices = new LinkedList<Voice>();

        }

        public void NoteOn(MIDINote n)
        {
            Voice v = GetNextVoice();
            v.NoteOn(n);
            n.voice = v;
            
            if (currentActiveVoices < voiceCount)
                currentActiveVoices++;
        }

        public void NoteOff(MIDINote n)
        {
            Voice v = n.voice;
            v.NoteOff();
        }

        public float NextSample()
        {
            
            float s = 0;
            foreach(Voice v in voices)
            {
                s += v.NextSample();
            }
            if (currentActiveVoices > 0)
                return s * 1 / currentActiveVoices;
            else
                return s;
        }

        public void AddVoice(Voice v)
        {
            voices.AddLast(v);
        }
        private Voice GetNextVoice()
        {
            Voice v = voices.First.Value;
            voices.RemoveFirst();
            voices.AddLast(v);
            return v;
        }
        public void FinishVoice(Voice v)
        {
            Debug.Log("here");
            voices.Remove(v);
            voices.AddLast(v);
            currentActiveVoices--;
        }
    }

}