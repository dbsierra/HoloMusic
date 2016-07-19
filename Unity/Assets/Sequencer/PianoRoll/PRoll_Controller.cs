using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MusicUtilities;
using MusicDevice;


namespace Sequencer.PianoRoll
{

    /// <summary>
    /// Piano Roll Controller, the hub for a piano roll instance.
    /// Represents a sequence of MIDI notes of length 4 bars/16*4 beats for 1/16th note resolution
    /// 
    /// Tasks:
    ///     - Instantiate, maintain and destroy all visual GUI for this piano roll
    ///     - Instantiate and maintain all of the slots for this piano roll
    ///     - Store sequenced note information for this piano roll
    ///     - Keep track of current position in the sequence
    ///     
    /// </summary>
    public class PRoll_Controller : MonoBehaviour
    {

        //The main data structure containing the note sequence
        List<NoteEvent>[] matrix;

        //current step in the sequence
        byte step;

        bool ready;

        //the sound producing instrument this will trigger
        FMSynthesizer instrument;

        //The current sample number
        int sample;

        #region MonoBehavior functions
        void Start()
        {
            //16 bars, 1/16th note resolution
            matrix = new List<NoteEvent>[16 * 4];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new List<NoteEvent>();
            }
            step = 0;
            instrument = new FMSynthesizer();
            ready = true;

            AddNote(0, Settings.getMIDI("C#4"), 1, 1);
            AddNote(1, Settings.getMIDI("F#4"), 1, 1);
            AddNote(2, Settings.getMIDI("E4"), 1, 1);
            AddNote(3, Settings.getMIDI("C#4"), 1, 1);
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (ready)
            {
                for (int i = 0; i < data.Length; i = i + channels)
                {
                    if (sample >= Settings.BeatLength_s)
                    {
                        OnStep();
                        sample = 0;
                    }
                    else
                        sample++;


                    float s = instrument.NextSample();

                    data[i] = .1f * s;
                    if (channels == 2)
                    {
                        data[i + 1] = data[i];
                    }
                }
            }

        }
        #endregion

        /// <summary>
        /// Create a new piano roll user interface
        /// </summary>
        private void CreatePianoRollGUI()
        {
            //instantiate piano roll prefab.


        }

        /// <summary>
        /// Gets called on every step of the sequence
        /// </summary>
        private void OnStep()
        {
            //Debug.Log("Step: " + step + " " + matrix[step].Count);
            foreach (NoteEvent n in matrix[step])
            {
                if (n.noteOn)
                {
                    instrument.NoteOn(n.note);
                }
                else
                    instrument.NoteOff(n.note);
            }

            step += 1;
            if (step >= matrix.Length)
                step = 0;
        }

        /// <summary>
        /// Create and add a note to a specific location 
        /// </summary>
        /// <param name="beat">which beat to place the note at</param>
        /// <param name="pitch"></param>
        /// <param name="velocity"></param>
        /// <param name="duration"></param>
        void AddNote(byte beat, byte pitch, byte velocity, byte duration)
        {
            AddNote(beat, new MIDINote(pitch, duration, velocity));
        }

        /// <summary>
        /// Add a note to the matrix at a specific location
        /// </summary>
        /// <param name="beat">location to store the note</param>
        /// <param name="n">the note to store</param>
        void AddNote(byte beat, MIDINote n)
        {
            matrix[beat].Add(new NoteEvent(n, true));
            int endBeat = Mathf.Clamp(beat + n.duration, 0, matrix.Length - 1);
            matrix[endBeat].Add(new NoteEvent(n, false));
        }
    }

 

}
