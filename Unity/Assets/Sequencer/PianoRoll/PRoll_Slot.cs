using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace Sequencer.PianoRoll
{
    public class PRoll_Slot : MonoBehaviour
    {
        public bool active; //active means this slot has a note inside of it and should be played

        public PRoll_Controller Controller;
        public byte PositionIndex;
        public byte PitchIndex;
        public MIDINote Note;
        public PRoll_Note NoteGeo;

        void Start()
        {
        }

        public void OnTap()
        {
            if (!active)
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position, PositionIndex, PitchIndex, this);
                InjectNote(PRoll_NoteDrawer.EndNoteDraw(transform.position));
            } 
            else
            {

            }  
        }

        public void OnNavigationStarted( Vector3 relativePosition )
        {
            if( !active )
                PRoll_NoteDrawer.BeginNoteDraw(transform.position, PositionIndex, PitchIndex, this);
        }
        public void OnNavigationUpdated( Vector3 relativePosition )
        {
            if (!active)
                PRoll_NoteDrawer.DrawNote(relativePosition.x);
        }
        public void OnNavigationCompleted( Vector3 relativePosition )
        {
            if (!active)
                InjectNote(PRoll_NoteDrawer.EndNoteDraw(relativePosition) );
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