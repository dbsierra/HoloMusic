using UnityEngine;
using System.Collections;
using MusicUtilities;

public interface IMIDIDevice {

    void NoteOn(MIDINote n);
    void NoteOff(MIDINote n);
    float NextSample();

}
