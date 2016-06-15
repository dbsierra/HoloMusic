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

        public VoiceManager(byte vcount)
        {
            voiceCount = vcount;
        }

        public void NoteOn(MIDINote n)
        {
            Voice v = GetNextVoice();
            noteMapper.Add(n, v);
            v.NoteOn(n);
            n.voice = v;
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
        private void FinishVoice(Voice v)
        {
            voices.Remove(v);
            voices.AddLast(v);
        }
    }

}