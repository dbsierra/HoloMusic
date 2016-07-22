using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicDevice;

namespace MusicUtilities
{

    public class NoteEvent
    {
        public MIDINote note;
        public bool noteOn;
        public bool active;

        public NoteEvent(bool active=false)
        {
            this.active = active;
        }
        public NoteEvent(MIDINote n, bool nOn, bool active=true)
        {
            note = n;
            noteOn = nOn;
            this.active = active;
        }
    }

    /// <summary>
    /// Represents a single midi note
    /// </summary>
    public class MIDINote
    {
        public byte midi;
        public byte pitchLetterIndex; //just the index of the pitch letter (0-11, C-B)
        public byte duration; //for sequencers, notes have a defined duration
        public byte velocity;
        public float frequency;
        public bool noteOn; //for sequencers, need to know when the note is on and off
        public Voice voice;

        public MIDINote() { }

        public MIDINote(byte midi, byte duration, byte velocity, bool noteOn=true)
        {
            this.midi = midi;
            this.duration = duration;
            this.velocity = velocity;
            this.frequency = Settings.getFreq(midi);
            this.noteOn = noteOn;
            this.pitchLetterIndex = Settings.PitchIndexFromMidi(midi);
        }
        public MIDINote(byte midi, byte velocity)
        {
            this.midi = midi;
            this.velocity = velocity;
            this.frequency = Settings.getFreq(midi);
            this.pitchLetterIndex = Settings.PitchIndexFromMidi(midi);
        }
    }
    

    public class Settings
    {

        public static int BPM;
        public static float BeatLength;
        public static int BeatLength_s;
        public static int SampleRate;

        public static byte MAX_VOICES = 16;

        private static Dictionary<string, NoteInfo> noteLookUp = new Dictionary<string, NoteInfo>();
        private static Dictionary<byte, NoteInfo> noteMidiLookUp = new Dictionary<byte, NoteInfo>();
        private static Dictionary<string, int> notes = new Dictionary<string, int>();
        private static Dictionary<int, string> pitchIndexToString = new Dictionary<int, string>();

        /// <summary>
        /// Gets stored in dictionary for lookup table of midi notes/frequencies
        /// </summary>
        public struct NoteInfo
        {
            public byte midi;
            public float frequency;
            public NoteInfo(byte m, float f)
            {
                midi = m;
                frequency = f;
            }
        }

        public static float inc;
        public static float TWO_PI = 6.28318530718f;

        static Settings()
        {
            SampleRate = AudioSettings.outputSampleRate;
            inc = 1.0f / AudioSettings.outputSampleRate;

            //hard coded BPM for now
            BPM = 120;
            BeatLength = 60.0f / ((float)BPM * 4.0f); // * 4 to make our BPM resolution in 1/16th notes
            BeatLength_s = (int)(BeatLength * SampleRate);

            notes.Add("C", 0);
            notes.Add("C#", 1);
            notes.Add("D", 2);
            notes.Add("D#", 3);
            notes.Add("E", 4);
            notes.Add("F", 5);
            notes.Add("F#", 6);
            notes.Add("G", 7);
            notes.Add("G#", 8);
            notes.Add("A", 9);
            notes.Add("A#", 10);
            notes.Add("B", 11);
            pitchIndexToString.Add(0, "C");
            pitchIndexToString.Add(1, "C#");
            pitchIndexToString.Add(2, "D");
            pitchIndexToString.Add(3, "D#");
            pitchIndexToString.Add(4, "E");
            pitchIndexToString.Add(5, "F");
            pitchIndexToString.Add(6, "F#");
            pitchIndexToString.Add(7, "G");
            pitchIndexToString.Add(8, "G#");
            pitchIndexToString.Add(9, "A");
            pitchIndexToString.Add(10, "A#");
            pitchIndexToString.Add(11, "B");

            for (byte i = 0; i < 10; i++)
            {
                noteLookUp.Add("C" + i, new NoteInfo((byte)(0 + i * 12), noteToFrequency("C" + i)));
                noteLookUp.Add("C#" + i, new NoteInfo((byte)(1 + i * 12), noteToFrequency("C#" + i)));
                noteLookUp.Add("D" + i, new NoteInfo((byte)(2 + i * 12), noteToFrequency("D" + i)));
                noteLookUp.Add("D#" + i, new NoteInfo((byte)(3 + i * 12), noteToFrequency("D#" + i)));
                noteLookUp.Add("E" + i, new NoteInfo((byte)(4 + i * 12), noteToFrequency("E" + i)));
                noteLookUp.Add("F" + i, new NoteInfo((byte)(5 + i * 12), noteToFrequency("F" + i)));
                noteLookUp.Add("F#" + i, new NoteInfo((byte)(6 + i * 12), noteToFrequency("F#" + i)));
                noteLookUp.Add("G" + i, new NoteInfo((byte)(7 + i * 12), noteToFrequency("G" + i)));
                noteLookUp.Add("G#" + i, new NoteInfo((byte)(8 + i * 12), noteToFrequency("G#" + i)));
                noteLookUp.Add("A" + i, new NoteInfo((byte)(9 + i * 12), noteToFrequency("A" + i)));
                noteLookUp.Add("A#" + i, new NoteInfo((byte)(10 + i * 12), noteToFrequency("A#" + i)));
                noteLookUp.Add("B" + i, new NoteInfo((byte)(11 + i * 12), noteToFrequency("B" + i)));

                noteMidiLookUp.Add((byte)(0 + i * 12), new NoteInfo((byte)(0 + i * 12), noteToFrequency("C" + i)));
                noteMidiLookUp.Add((byte)(1 + i * 12), new NoteInfo((byte)(1 + i * 12), noteToFrequency("C#" + i)));
                noteMidiLookUp.Add((byte)(2 + i * 12), new NoteInfo((byte)(2 + i * 12), noteToFrequency("D" + i)));
                noteMidiLookUp.Add((byte)(3 + i * 12), new NoteInfo((byte)(3 + i * 12), noteToFrequency("D#" + i)));
                noteMidiLookUp.Add((byte)(4 + i * 12), new NoteInfo((byte)(4 + i * 12), noteToFrequency("E" + i)));
                noteMidiLookUp.Add((byte)(5 + i * 12), new NoteInfo((byte)(5 + i * 12), noteToFrequency("F" + i)));
                noteMidiLookUp.Add((byte)(6 + i * 12), new NoteInfo((byte)(6 + i * 12), noteToFrequency("F#" + i)));
                noteMidiLookUp.Add((byte)(7 + i * 12), new NoteInfo((byte)(7 + i * 12), noteToFrequency("G" + i)));
                noteMidiLookUp.Add((byte)(8 + i * 12), new NoteInfo((byte)(8 + i * 12), noteToFrequency("G#" + i)));
                noteMidiLookUp.Add((byte)(9 + i * 12), new NoteInfo((byte)(9 + i * 12), noteToFrequency("A" + i)));
                noteMidiLookUp.Add((byte)(10 + i * 12), new NoteInfo((byte)(10 + i * 12), noteToFrequency("A#" + i)));
                noteMidiLookUp.Add((byte)(11 + i * 12), new NoteInfo((byte)(11 + i * 12), noteToFrequency("B" + i)));
            }
        }

        public static byte PitchIndexFromMidi(byte m)
        {

            while( m >= 12 )
            {
                m -= 12;
            }

            return m;
        }
        public static byte MidiFromPitchIndex(byte index, byte octave)
        {
            if( index >= 12 )
            {
                Debug.LogError("Attemping to access pitch outside of 12 note octave");
                    return 0;
            }
            return getMIDI( pitchIndexToString[index]+""+octave );
        }


        public static float getFreq(byte m)
        {
            return noteMidiLookUp[m].frequency;
        }
        public static float getFreq(string n)
        {
            return noteLookUp[n].frequency;
        }
        public static byte getMIDI(string n)
        {
            return noteLookUp[n].midi;
        }

        private static float noteToFrequency(string n)
        {
            return Mathf.Pow(2, (stringToNote(n) - 57) / 12) * 440;
        }

        private static float stringToNote(string s)
        {
            float note = 0;

            int sharp = 0;
            int flat = 0;
            int octave = 4;

            //dealing with a sharp or flat
            if (s.Length == 3)
            {
                if (s[1] == 'b') flat = 1;
                else if (s[1] == '#') sharp = 1;

                int.TryParse("" + s[2], out octave);
            }
            else
            {
                int.TryParse("" + s[1], out octave);
            }

            note = notes["" + s[0]];

            return note + (12 * octave + 12) + sharp - flat;
        }

    }

}