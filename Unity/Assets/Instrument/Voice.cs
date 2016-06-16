using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace MusicDevice
{
    /// <summary>
    /// This is the container for the meat of your instrument, the sound generating guts. A voice object will take note on and off data
    /// and return to you an audio sample. For polyphonic instruments, this is what gets instanced by the Voice Manager, so only put things in 
    /// here that you would want on a per-instance basis.
    /// </summary>
    public interface Voice
    {
        VoiceManager parentManager { get; set; }
        string Name { get; set; }
        float Amplitude { get; set; }
        void NoteOn(MIDINote n);
        void NoteOff();
        void Done();
        float NextSample();
        
    }

}
