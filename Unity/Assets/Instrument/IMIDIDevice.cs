using UnityEngine;
using System.Collections;
using MusicUtilities;

interface IMIDIDevice {

    void NoteOn(MIDINote n);
    void NoteOff(MIDINote n);
    float NextSample();

}
