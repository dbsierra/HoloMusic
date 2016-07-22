using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace Sequencer.PianoRoll
{

        public class PRoll_Slot : MonoBehaviour
        {

            public PRoll_Controller Controller;
            public byte PositionIndex;
            public byte PitchIndex;

            void Start()
            {
            }

            public void OnTap()
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position, PositionIndex, PitchIndex, Controller);
                InjectNote(PRoll_NoteDrawer.EndNoteDraw(transform.position) );
            }

            public void OnNavigationStarted( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position, PositionIndex, PitchIndex, Controller);
            }
            public void OnNavigationUpdated( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.DrawNote(relativePosition.x);
            }
            public void OnNavigationCompleted( Vector3 relativePosition )
            {
                InjectNote(PRoll_NoteDrawer.EndNoteDraw(relativePosition) );
            }
            private void InjectNote( byte duration )
            {
                Controller.AddNote(PositionIndex, Settings.MidiFromPitchIndex(PitchIndex, Controller.octave), 1, duration);
            }

        }


}