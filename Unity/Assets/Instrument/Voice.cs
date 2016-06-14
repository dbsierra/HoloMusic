﻿using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace MusicDevice
{
    /// <summary>
    /// This is the container for the meat of your instrument, the sound generating guts. A voice object will take note on and off data
    /// and return to you an audio sample. For polyphonic instruments, this is what gets instanced by the Voice Manager, so only put things in 
    /// here that you would want on a per-instance basis.
    /// </summary>
    public abstract class Voice : IMIDIDevice
    {
        public abstract void NoteOn(MIDINote n);
        public abstract void NoteOff(MIDINote n);
        public abstract float NextSample();
    }

}