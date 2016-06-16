using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace MusicDevice
{

    /// <summary>
    /// This is the final container for your instrument, be it a synthesizer, sampler, or what have you.
    /// It gets fed note on and off data, and returns an audio sample to be sent to the final mixer.
    /// </summary>
    public abstract class Instrument : IMIDIDevice
    {
        public byte VoiceCount;

        private VoiceManager vm;

        public abstract void NoteOn(MIDINote n);
        public abstract void NoteOff(MIDINote n);
        public abstract float NextSample();

    }

}
