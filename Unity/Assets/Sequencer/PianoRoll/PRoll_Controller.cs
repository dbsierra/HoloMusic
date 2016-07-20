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
    /// A sequencer steps through at the specified BPM. For each step, it should play whatever notes located there for their duration.
    /// 
    /// Problem:
    /// How do we deal with duration? One method is to specify note on and note off messages
    /// 
    /// 
    /// </summary>
    public class PRoll_Controller : MonoBehaviour
    {

        //TODO: separate certain things outside of individual sequencer objects
        private int notesPerSegment = 16;
        public byte octave = 4;

        //The main data structure containing the note sequence
        //List<NoteEvent>[] matrix2;
        NoteEvent[,] matrix;
        Dictionary<uint, List<MIDINote>> noteStream = new Dictionary<uint, List<MIDINote>>();

        //current step in the sequence
        byte step;

        bool ready;

        //the sound producing instrument this will trigger
        FMSynthesizer instrument;

        //The current sample number
        int sample;
        uint globalSample;

        public GameObject NoteSlot;
        public Transform NoteSlotContainer;

        #region MonoBehavior functions
        void Start()
        {
            //16 bars, 1/16th note resolution
            /*
            matrix2 = new List<NoteEvent>[16];
            for (int i = 0; i < matrix2.Length; i++)
            {
                matrix2[i] = new List<NoteEvent>();
            }
            */
            matrix = new NoteEvent[notesPerSegment, 12];
            
            for (int i = 0; i < notesPerSegment; i++)
            {
                for( int j=0; j < 12; j++ )
                {
                    matrix[i, j] = new NoteEvent();
                }
            }
            
            step = 0;
            instrument = new FMSynthesizer();
            ready = true;

            CreatePianoRollGUI(new Vector2(.01f, .01f), -0.0002f);

            //AddNote(0, Settings.getMIDI("C#4"), 1, 1);
            //AddNote(4, Settings.getMIDI("G#4"), 1, 3);
            //AddNote(10, Settings.getMIDI("E4"), 1, 1);
            //AddNote(14, Settings.getMIDI("A3"), 1, 1);

        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (ready)
            {
                for (int i = 0; i < data.Length; i = i + channels)
                {

                    ReleaseNotesFromStream(globalSample);

                    //Sequencer step timer
                    if (sample >= Settings.BeatLength_s)
                    {
                        OnStep();
                        sample = 0;
                    }
                    else
                        sample++;

                    

                    //obtain our sample for audio playback!
                    float s = instrument.NextSample();

                    //if we are in stereo, duplicate the sample for L+R channels
                    data[i] = .1f * s;
                    if (channels == 2)
                    {
                        data[i + 1] = data[i];
                    }

                    globalSample++;
                }
            }

        }
        #endregion

        private void ReleaseNotesFromStream( uint endSample )
        {
            if( noteStream.ContainsKey(endSample) )
            {
                foreach(MIDINote n in noteStream[endSample] )
                {
                    instrument.NoteOff(n);
                }
                noteStream.Remove(endSample);
            }
        }

        private void AddNoteToStream( MIDINote n )
        {
            instrument.NoteOn(n);

            //get sample at which we should call noteOff on this note
            uint endSample = globalSample + (uint)(n.duration * Settings.BeatLength_s);

            //if not already part of the dictionary, add it
            if ( !noteStream.ContainsKey(endSample) )
                noteStream[endSample] = new List<MIDINote>();

            noteStream[endSample].Add(n);
        }

        /// <summary>
        /// Create a new piano roll user interface
        /// </summary>
        private void CreatePianoRollGUI( Vector2 size, float zOffset )
        {
            //Instantiate piano roll prefab.
            //12 rows of 16 columns
            for(byte r =0; r<12; r++ )
            {
                for( byte c=0; c<16; c++ )
                {
                    GameObject g = (GameObject)GameObject.Instantiate(NoteSlot);
                    g.transform.SetParent( NoteSlotContainer, false);
                    g.transform.localPosition = new Vector3(size.x * c, size.y * r, zOffset);

                    PRoll_Slot slot = g.GetComponent<PRoll_Slot>();
                    if( slot == null )
                    {
                        Debug.LogError("Missing PRoll_Slot class");
                        break;
                    }

                    slot.PositionIndex = c;
                    slot.PitchIndex = r;
                    slot.Controller = this; 
                }
            }

        }

        /// <summary>
        /// Gets called on every step of the sequence
        /// </summary>
        private void OnStep()
        {
            
            for (int j = 0; j < 12; j++)
            {
                if( matrix[step, j].active )
                {
                    if (matrix[step, j].noteOn)
                    {
                        AddNoteToStream(matrix[step, j].note);

                        //instrument.NoteOn(matrix[step, j].note);
                    }
                    else
                    {
                        ;// instrument.NoteOff(matrix[step, j].note);
                    }                     
                }
            }
            
            /*
            foreach (NoteEvent n in matrix2[step])
            {
                if (n.noteOn)
                {
                    instrument.NoteOn(n.note);
                }
                else
                    instrument.NoteOff(n.note);
            }
            */

            step += 1;
            if (step >= notesPerSegment)
                step = 0;
        }


        /// <summary>
        /// Given the note, return the position index of the next note. If no other note, returns length of row
        /// </summary>
        /// <param name="noteSlot"></param>
        public int GetIndexOfNextNote( int startPosition, int pitchIndex )
        {
            int i = startPosition;
            for( i = startPosition; i < notesPerSegment; i++ )
            {
                if (matrix[i, pitchIndex].active)
                    break;
            }
            return i;
        }

        /// <summary>
        /// Create and add a note to a specific location 
        /// </summary>
        /// <param name="beat">which beat to place the note at</param>
        /// <param name="pitch"></param>
        /// <param name="velocity"></param>
        /// <param name="duration"></param>
        public void AddNote(byte beat, byte pitch, byte velocity, byte duration)
        {
            AddNote(beat, new MIDINote(pitch, duration, velocity) );
        }

        /// <summary>
        /// Add a note to the matrix at a specific location
        /// </summary>
        /// <param name="beat">location to store the note</param>
        /// <param name="n">the note to store</param>
        public void AddNote(byte beat, MIDINote n)
        {
            int pitchIndex = n.pitchLetterIndex;
            matrix[beat, pitchIndex] = new NoteEvent(n, true);

           // matrix2[beat].Add(new NoteEvent(n, true));

           // int endBeat = Mathf.Clamp(beat + n.duration + 1, 0, notesPerSegment - 1);
           // matrix[endBeat, pitchIndex] = new NoteEvent(n, false);

            //matrix2[endBeat].Add(new NoteEvent(n, false));
        }

        public void RemoveNote(byte positionIndex, byte pitchIndex)
        {
            matrix[positionIndex, pitchIndex].active = false;
        }
    }

 

}
