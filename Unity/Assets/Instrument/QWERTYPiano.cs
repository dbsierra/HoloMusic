﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;

public class QWERTYPiano : MonoBehaviour {

    public int Octave = 4;
    Dictionary<KeyCode, MIDINote> keyMap = new Dictionary<KeyCode, MIDINote>();
    FMSynthesizer synth;
    bool playing;


    // Use this for initialization
    void Start () {
        keyMap[KeyCode.A] = new MIDINote(48, 1);
        keyMap[KeyCode.W] = new MIDINote(49, 1);
        keyMap[KeyCode.S] = new MIDINote(50, 1);
        keyMap[KeyCode.E] = new MIDINote(51, 1);
        keyMap[KeyCode.D] = new MIDINote(52, 1);
        keyMap[KeyCode.F] = new MIDINote(53, 1);
        keyMap[KeyCode.T] = new MIDINote(54, 1);
        keyMap[KeyCode.G] = new MIDINote(55, 1);
        keyMap[KeyCode.Y] = new MIDINote(56, 1);
        keyMap[KeyCode.H] = new MIDINote(57, 1);
        keyMap[KeyCode.U] = new MIDINote(58, 1);
        keyMap[KeyCode.J] = new MIDINote(59, 1);

        synth = new FMSynthesizer();
    }
	
	// Update is called once per frame
	void Update () {
	    if( Input.GetKeyDown(KeyCode.A) )
        {
            synth.NoteOn(keyMap[KeyCode.A]);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            synth.NoteOff(keyMap[KeyCode.A]);
        }

        if (Input.GetKeyDown(KeyCode.W))
            synth.NoteOn(keyMap[KeyCode.W]);
        else if (Input.GetKeyUp(KeyCode.W))
            synth.NoteOff(keyMap[KeyCode.W]);
        
        if (Input.GetKeyDown(KeyCode.S))
            synth.NoteOn(keyMap[KeyCode.S]);
        else if (Input.GetKeyUp(KeyCode.S))
            synth.NoteOff(keyMap[KeyCode.S]);

        if (Input.GetKeyDown(KeyCode.E))
            synth.NoteOn(keyMap[KeyCode.E]);
        else if (Input.GetKeyUp(KeyCode.E))
            synth.NoteOff(keyMap[KeyCode.E]);

        if (Input.GetKeyDown(KeyCode.D))
            synth.NoteOn(keyMap[KeyCode.D]);
        else if (Input.GetKeyUp(KeyCode.D))
            synth.NoteOff(keyMap[KeyCode.D]);

        if (Input.GetKeyDown(KeyCode.F))
            synth.NoteOn(keyMap[KeyCode.F]);
        else if (Input.GetKeyUp(KeyCode.F))
            synth.NoteOff(keyMap[KeyCode.F]);

        if (Input.GetKeyDown(KeyCode.T))
            synth.NoteOn(keyMap[KeyCode.T]);
        else if (Input.GetKeyUp(KeyCode.T))
            synth.NoteOff(keyMap[KeyCode.T]);

        if (Input.GetKeyDown(KeyCode.G))
            synth.NoteOn(keyMap[KeyCode.G]);
        else if (Input.GetKeyUp(KeyCode.G))
            synth.NoteOff(keyMap[KeyCode.G]);

        if (Input.GetKeyDown(KeyCode.Y))
            synth.NoteOn(keyMap[KeyCode.Y]);
        else if (Input.GetKeyUp(KeyCode.Y))
            synth.NoteOff(keyMap[KeyCode.Y]);

        if (Input.GetKeyDown(KeyCode.H))
            synth.NoteOn(keyMap[KeyCode.H]);
        else if (Input.GetKeyUp(KeyCode.H))
            synth.NoteOff(keyMap[KeyCode.H]);

        if (Input.GetKeyDown(KeyCode.U))
            synth.NoteOn(keyMap[KeyCode.U]);
        else if (Input.GetKeyUp(KeyCode.U))
            synth.NoteOff(keyMap[KeyCode.U]);

        if (Input.GetKeyDown(KeyCode.J))
            synth.NoteOn(keyMap[KeyCode.J]);
        else if (Input.GetKeyUp(KeyCode.J))
            synth.NoteOff(keyMap[KeyCode.J]);
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        //for each sample of this block of audio data
        for (int i = 0; i < data.Length; i = i + channels)
        {
            if (synth != null )
            {
                float s = synth.NextSample();

                data[i] = .5f * s;
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }
            }

        }


    }
}