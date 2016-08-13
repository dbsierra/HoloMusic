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
    public class PRoll_Controller : MonoBehaviour, IButtonReceiver
    {

        //TODO: separate certain things outside of individual sequencer objects
        private byte notesPerMeasure = 16;
        public byte octave = 4;

        //The main data structure containing the note sequence
        PRoll_Slot[,] matrix;

        //note stream contains notes that need to be released. The key corresponds to the next sample count at which to release the notes
        Dictionary<uint, List<PRoll_Slot>> noteStream = new Dictionary<uint, List<PRoll_Slot>>();

        byte step;   //current step in the sequence
        byte mtStep; //main thread step: used for triggering things in the main thread (not OnAudioFilterRead(), so e.g. Update() ... etc)

        //signifies when piano roll is done initializing
        bool ready;

        

        uint globalSample;  //The current sample number
        uint sample;        //used for sequencer timing. Resets when == beath length

        public GameObject NoteSlot;         //gets instanced, represents one slot on the piano roll
        public Transform NoteSlotContainer; //the container for all the slots
        public Transform NoteGeoContainer; //container for all of the note geometries
        public Transform TimelineMarker;
        public Renderer strip;
        public Transform PianoKeys;
        private Material[] PianoKeyMaterials;
        public UIFMSynthesizer instrument; //the sound producing instrument this will trigger

        //main thread triggers from audio thread
        bool hitIt;
        bool deHighlightNote;
        List<PRoll_Slot> deHighlightNote_list;
        bool highlightNote;
        List<PRoll_Slot> highlightNote_list;

        bool start;

        public GameObject root;

        #region MonoBehavior functions
        void Start()
        {
            matrix = new PRoll_Slot[notesPerMeasure, 12];
            
            step = mtStep = 0;
           // instrument = new FMSynthesizer();
           
            InitializePianoRoll(new Vector2(.01f, .01f));

            deHighlightNote_list = new List<PRoll_Slot>();
            highlightNote_list = new List<PRoll_Slot>();

            ready = true;

            //init piano key materials array
            PianoKeyMaterials = new Material[12];
            int i = 0;
            foreach( Renderer r in PianoKeys.GetComponentsInChildren<Renderer>() )
            {
                PianoKeyMaterials[i++] = r.material;
            }
        }

        IEnumerable waitToInit()
        {
            yield return new WaitForSeconds(1);
            ready = true;
        }

        void Update()
        {
            if (ready && !start)
            {
                // AddNote(0, Settings.getMIDI("C#4"), 1, 1);
               // AddNote(4, Settings.getMIDI("G#4"), 1, 2);

               // AddNote(8, Settings.getMIDI("E4"), 1, 1);
              //  AddNote(12, Settings.getMIDI("A4"), 1, 1);
                RefreshPianoRoll();
                start = true;
            }

            if ( hitIt )
            {
                
                hitIt = false;

                if (highlightNote)
                {
                    //highlightNote = false;
                    foreach (PRoll_Slot n in highlightNote_list)
                    {
                        n.NotePlayingAnimation();

                        if (n.PitchIndex != 1 && n.PitchIndex != 3 && n.PitchIndex != 6 && n.PitchIndex != 8 && n.PitchIndex != 10)
                            PianoKeyMaterials[n.PitchIndex].SetColor("_EmissionColor", new Color(0, .2f, 1));
                        else
                            PianoKeyMaterials[n.PitchIndex].SetColor("_EmissionColor", new Color(0, .45f, 1));
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

                        if( n.PitchIndex != 1 && n.PitchIndex != 3 && n.PitchIndex != 6 && n.PitchIndex != 8 && n.PitchIndex != 10 )
                            PianoKeyMaterials[n.PitchIndex].SetColor("_EmissionColor", new Color(.4f, .4f, .4f  ));
                        else
                            PianoKeyMaterials[n.PitchIndex].SetColor("_EmissionColor", new Color(.06f, .06f, .06f));
                    }
                    deHighlightNote_list.Clear();
                }

                MoveTimelineTracker();

                mtStep++;
                if (mtStep >= notesPerMeasure)
                    mtStep = step;  //Ensures that they stay in sync!!
            }

            if (Input.GetKeyDown(KeyCode.C))
                Flush();
        }

        uint oldSample;
        void OnAudioFilterRead(float[] data, int channels)
        {
            if (start)
            {
                for (int i = 0; i < data.Length; i = i + channels)
                {

                    ReleaseNotesFromStream(globalSample);

                    //Sequencer step timer
                    if (sample >= Settings.BeatLength_s)
                    {
                        OnStep();
                        sample = 0;
                        oldSample = globalSample;
                    }
                    else
                        sample++;

                    //obtain our sample for audio playback!
                   // float s = instrument.NextSample();

                    //if we are in stereo, duplicate the sample for L+R channels
                   // data[i] = .1f * s;
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
        private void InitializePianoRoll( Vector2 size )
        {
            //Instantiate piano roll prefab.
            //12 rows of 16 columns
            for(byte r =0; r<12; r++ )
            {
                for( byte c=0; c<16; c++ )
                {
                    GameObject g = (GameObject)GameObject.Instantiate(NoteSlot);
                    g.transform.SetParent( NoteSlotContainer, false);
                    g.transform.localPosition = new Vector3(size.x * -c, size.y * r, 0);

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
            if (step >= notesPerMeasure)
                step = 0;
        }

        private void MoveTimelineTracker()
        {
            float moveBy = -.01f;

            //Debug.Log(step + " " + mtStep);

            TimelineMarker.localPosition = new Vector3(mtStep * moveBy, 0, .0005f);
        }

        /// <summary>
        /// Given the note, return the position index of the next note. If no other note, returns length of row
        /// </summary>
        /// <param name="noteSlot"></param>
        public int GetIndexOfNextNote( int startPosition, int pitchIndex )
        {
            int i = startPosition;
            for( i = startPosition; i < notesPerMeasure; i++ )
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
        public void AddNote(byte positionIndex, MIDINote n)
        {
            int pitchIndex = n.pitchLetterIndex;
            
            //The first slot the note occupies must be set to active
            matrix[positionIndex, pitchIndex].Note = n;
            matrix[positionIndex, pitchIndex].active = true;

            //ensure all the slots that this note occupies are set to locked
            for ( int i = positionIndex + 1; i < positionIndex + n.duration; i++)
            {
                if (i >= notesPerMeasure)
                    break;

                matrix[i, pitchIndex].locked = true;
                matrix[i, pitchIndex].NoteGeo = matrix[positionIndex, pitchIndex].NoteGeo;
                matrix[i, pitchIndex].Note = n;
            }
            
        }
        
        /// <summary>
        /// Remove a note from the matrix at the specified location
        /// </summary>
        /// <param name="positionIndex"></param>
        /// <param name="pitchIndex"></param>
        public void RemoveNote(byte positionIndex, byte pitchIndex)
        {
            if ( matrix[positionIndex, pitchIndex].active )
            {
                matrix[positionIndex, pitchIndex].active = false;
                GameObject.Destroy(matrix[positionIndex, pitchIndex].NoteGeo.gameObject);

                //Set needed properties for all of the proceeding notes for the duration
                for (int i = positionIndex + 1; i <= positionIndex + matrix[positionIndex, pitchIndex].Note.duration; i++)
                {
                    if (i >= notesPerMeasure)
                        break;

                    matrix[i, pitchIndex].locked = false;
                    matrix[i, pitchIndex].NoteGeo = matrix[i, pitchIndex].NoteGeo;
                }
            }
            //check to make sure the note doesn't have any geo associated with it
            else if( !matrix[positionIndex, pitchIndex].locked )
            {
                if (matrix[positionIndex, pitchIndex].NoteGeo != null)
                    GameObject.Destroy(matrix[positionIndex, pitchIndex].NoteGeo.gameObject);
            }

        }

        /// <summary>
        /// Entrance animation
        /// </summary>
        public void Enter()
        {
            if( root )
            {
                root.SetActive(true);
            }
            else
            {
                Debug.LogError("No root specified for this piano roll");
            }
        }

        /// <summary>
        /// Exit animation
        /// </summary>
        public void Exit()
        {
            if (root)
            {
                root.SetActive(false);
            }
            else
            {
                Debug.LogError("No root specified for this piano roll");
            }
        }

        /// <summary>
        /// Scan through piano roll and insert any notes that exist in the matrix but don't have geo;
        /// </summary>
        private void RefreshPianoRoll()
        {
            for (byte r = 0; r < 12; r++)
            {
                for (byte c = 0; c < 16; c++)
                {
                    if (matrix[c, r].active)
                    {
                        matrix[c, r].InsertNoteGeo();
                    }
                }
            }
        }

        /// <summary>
        /// Get rid of all notes in the Piano Roll
        /// </summary>
        private void Flush()
        {
            for (byte r = 0; r < 12; r++)
            {
                for (byte c = 0; c < 16; c++)
                {
                    RemoveNote(c, r);
                }
            }
        }


        public void OnTap(string name)
        {
            if( name.Equals("PianoRoll_hide_button") )
            {
                Exit();
            }
            else if (name.Equals("PianoRoll_show_button"))
            {
                Enter();
            }
        }
    }

 

}
