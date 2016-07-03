using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace MusicDevice
{
    /// <summary>
    /// This is the container for the sound generating part of your instrument. A voice object will take note on and off events
    /// and return to you an audio sample. For polyphonic instruments, this is what gets instanced by the Voice Manager, so only put things in 
    /// here that you would want on a per-instance basis.
    /// </summary>
    public interface Voice
    {
        //All voices must be part of a VoiceManager
        VoiceManager parentManager { get; set; }        

        //identification
        string Name { get; set; }

        //0 - 1 : what we are multiplying the samples by
        float Gain { get; set; }

        //handles what happens when note on is received
        void NoteOn(MIDINote n);

        //handles what happens when note off is received
        void NoteOff();

        //called when done producing sound (after noteoff and any release finish)
        void Done();    

        //DSP goes here!
        float NextSample();  
    }

}
