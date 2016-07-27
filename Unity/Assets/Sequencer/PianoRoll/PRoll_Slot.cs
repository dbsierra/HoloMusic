using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace Sequencer.PianoRoll
{
    public class PRoll_Slot : MonoBehaviour
    {
        public bool active; //active means this slot has a note inside of it and should be played along with all the animations required
        public bool locked; //just means this slot cannot be modified, as a note is in its space, but it is not the initial slot holding the note, so no need to play animations or sounds.

        public PRoll_Controller Controller;
        public byte PositionIndex;
        public byte PitchIndex;
        public MIDINote Note;
        public PRoll_Note NoteGeo;

        GameObject DebugCube;

        void Start()
        {
            DebugCube = GameObject.Find("DebugCube");
        }

        public void InsertNoteGeo()
        {
            if( NoteGeo )
            {
                GameObject.Destroy(NoteGeo);
            }
            PRoll_NoteDrawer.InsertNoteGeo(this);
        }

        public void OnTap()
        {
            if (!active && !locked)
            {
                PRoll_NoteDrawer.BeginNoteDraw(this);
                InjectNote(PRoll_NoteDrawer.EndNoteDraw());
            } 
            else
            {
                NoteGeo.Delete();
            }  
        }

        public void OnNavigationStarted( Vector3 relativePosition )
        {
            if( !active && !locked)
                PRoll_NoteDrawer.BeginNoteDraw(this);

            DebugCube.transform.localScale = new Vector3(.055f, .055f, .055f);
        }
        public void OnNavigationUpdated( Vector3 relativePosition )
        {
            if (!active && !locked)
                PRoll_NoteDrawer.DrawNote(relativePosition.x);

            DebugCube.transform.localRotation = Quaternion.Euler(new Vector3(0, relativePosition.x * 180f, 0));
        }
        public void OnNavigationCompleted( Vector3 relativePosition )
        {
            if (!active && !locked)
                InjectNote(PRoll_NoteDrawer.EndNoteDraw() );

            DebugCube.transform.localScale = new Vector3(.02f, .02f, .02f);
        }
        private void InjectNote( byte duration )
        {
            Note.duration = duration;
            Controller.AddNote(PositionIndex, Note);
        }


        public void NotePlayingAnimation()
        {
            NoteGeo.PlayingAnimation();
        }
        public void NoteStoppingAnimation()
        {
            NoteGeo.StoppingAnimation();
        }

    }
}