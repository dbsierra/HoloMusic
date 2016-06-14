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

        public VoiceManager(byte vcount)
        {
            voiceCount = vcount;
        }

        public void NoteOn(MIDINote n)
        {
            GetNextVoice().NoteOn(n);
        }

        public void NoteOff(MIDINote n)
        {
            //FinishVoice()
        }

        public float NextSample()
        {
            return GetNextVoice().NextSample();
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