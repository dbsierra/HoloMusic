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
        private byte notesPerSegment = 16;
        public byte octave = 4;

        //The main data structure containing the note sequence
        PRoll_Slot[,] matrix;

        //note stream contains notes that need to be released. The key corresponds to the next sample count at which to release the notes
        Dictionary<uint, List<PRoll_Slot>> noteStream = new Dictionary<uint, List<PRoll_Slot>>();

        byte step;   //current step in the sequence
        byte mtStep; //main thread step: used for triggering things in the main thread (not OnAudioFilterRead(), so e.g. Update() ... etc)

        //signifies when piano roll is done initializing
        bool ready;

        //the sound producing instrument this will trigger
        FMSynthesizer instrument;

        uint globalSample;  //The current sample number
        uint sample;        //used for sequencer timing. Resets when == beath length

        public GameObject NoteSlot;         //gets instanced, represents one slot on the piano roll
        public Transform NoteSlotContainer; //the container for all the slots
        public Transform TimelineMarker;
        public Renderer strip;

        //main thread triggers from audio thread
        bool hitIt;
        bool deHighlightNote;
        List<PRoll_Slot> deHighlightNote_list;
        bool highlightNote;
        List<PRoll_Slot> highlightNote_list;
 
        #region MonoBehavior functions
        void Start()
        {
            matrix = new PRoll_Slot[notesPerSegment, 12];
            
            
            step = mtStep = 0;
            instrument = new FMSynthesizer();
           
            InitializePianoRoll(new Vector2(.01f, .01f), -0.0002f);

            ready = true;

            // AddNote(0, Settings.getMIDI("C#4"), 1, 1);
            // AddNote(4, Settings.getMIDI("G#4"), 1, 1);
            // AddNote(8, Settings.getMIDI("E4"), 1, 1);
            // AddNote(12, Settings.getMIDI("A3"), 1, 1);

            deHighlightNote_list = new List<PRoll_Slot>();
            highlightNote_list = new List<PRoll_Slot>();
        }

        void Update()
        {
            if ( hitIt )
            {
                hitIt = false;

                MoveTimelineTracker();
                //strip.material.SetColor("_Color", new Color(1, 1f, 1f, .2f));

                //Debug.Log(highlightNote + " " + deHighlightNote);

                if (highlightNote)
                {
                    highlightNote = false;
                    foreach (PRoll_Slot n in highlightNote_list)
                    {
                        n.NotePlayingAnimation();
                    }
                    highlightNote_list.Clear();
                }

                //De-highlight any notes that are done playing
                if (deHighlightNote)
                {
                    deHighlightNote = false;
                    foreach (PRoll_Slot n in deHighlightNote_list)
                    {
                        n.NoteStoppingAnimation();
                    }
                    deHighlightNote_list.Clear();
                }

                mtStep++;
                if (mtStep >= notesPerSegment)
                    mtStep = 0;
            }

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
                foreach (PRoll_Slot n in noteStream[endSample] )
                {
                    instrument.NoteOff(n.Note);

                    //Main thread animations
                    deHighlightNote_list.Add(n);
                }

                //Main thread animations
                deHighlightNote = true;

                noteStream.Remove(endSample);
            }
        }

        private void AddNoteToStream(PRoll_Slot n )
        {
            instrument.NoteOn(n.Note);

            //get sample at which we should call noteOff on this note
            uint endSample = globalSample + (uint)(n.Note.duration * Settings.BeatLength_s);

            //if not already part of the dictionary, add it
            if ( !noteStream.ContainsKey(endSample) )
                noteStream[endSample] = new List<PRoll_Slot>();

            noteStream[endSample].Add(n);
        }

        /// <summary>
        /// Create a new piano roll user interface
        /// </summary>
        private void InitializePianoRoll( Vector2 size, float zOffset )
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

                    matrix[c, r] = slot;

                    slot.Note = new MIDINote(Settings.MidiFromPitchIndex(r, octave), 1, 1);
                    slot.PositionIndex = c;
                    slot.PitchIndex = r;
                    slot.Controller = this; 
                }
            }
        }

        private void RefreshPianoRoll()
        {

        }

        /// <summary>
        /// Get rid of all notes in the Piano Roll
        /// </summary>
        private void FlushPianoRoll()
        {
            for (byte r = 0; r < 12; r++)
            {
                for (byte c = 0; c < 16; c++)
                {

                }
            }
        }


        /// <summary>
        /// Gets called on every step of the sequence
        /// </summary>
        private void OnStep()
        {
            hitIt = true;
            for (int j = 0; j < 12; j++)
             {
                 if( matrix[step, j].active )
                 {
                    AddNoteToStream(matrix[step, j]);
                    highlightNote_list.Add(matrix[step, j]);
                    highlightNote = true;
                 }
             }


            step += 1;
            if (step >= notesPerSegment)
                step = 0;
        }

        private void MoveTimelineTracker()
        {
            float moveBy = -.01f;

            if (mtStep == 0)
                TimelineMarker.localPosition = new Vector3( 0, 0, 0.0005f );
            else
                TimelineMarker.Translate( new Vector3(moveBy, 0, 0), Space.Self );
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
            matrix[beat, pitchIndex].Note = n;
            matrix[beat, pitchIndex].active = true;
        }

        public void RemoveNote(byte positionIndex, byte pitchIndex)
        {
          //  Debug.Log(positionIndex + " " + pitchIndex);
            matrix[positionIndex, pitchIndex].active = false;
        }
    }

 

}
