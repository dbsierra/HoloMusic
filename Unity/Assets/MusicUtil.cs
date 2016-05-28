using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MusicUtil {

    public static int BPM;
    public static float BeatLength;

    private static Dictionary<string, float> noteLookUp = new Dictionary<string, float>();
    private static Dictionary<string, int> notes = new Dictionary<string, int>();

    static MusicUtil()
    {
        BPM = 120;
        BeatLength = 60.0f/( (float)BPM * 4.0f);

        notes.Add("C", 0);
        notes.Add("D", 2);
        notes.Add("E", 4);
        notes.Add("F", 5);
        notes.Add("G", 7);
        notes.Add("A", 9);
        notes.Add("B", 11);

        for( int i=0; i< 10; i++ )
        {
            noteLookUp.Add("C" + i, noteToFrequency("C" + i));
            noteLookUp.Add("C#" + i, noteToFrequency("C#" + i));
            noteLookUp.Add("D" + i, noteToFrequency("D" + i));
            noteLookUp.Add("D#" + i, noteToFrequency("D#" + i));
            noteLookUp.Add("E" + i, noteToFrequency("E" + i));
            noteLookUp.Add("F" + i, noteToFrequency("F" + i));
            noteLookUp.Add("F#" + i, noteToFrequency("F#" + i));
            noteLookUp.Add("G" + i, noteToFrequency("G" + i));
            noteLookUp.Add("G#" + i, noteToFrequency("G#" + i));
            noteLookUp.Add("A" + i, noteToFrequency("A" + i));
            noteLookUp.Add("A#" + i, noteToFrequency("A#" + i));
            noteLookUp.Add("B" + i, noteToFrequency("B" + i));
        }

    }

    public static float getFreq(string n)
    {
        return noteLookUp[n];
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
