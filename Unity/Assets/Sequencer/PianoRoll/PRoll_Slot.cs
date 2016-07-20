using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace Sequencer
{
    namespace PianoRoll
    {

        public class PRoll_Slot : MonoBehaviour
        {

            // TextMesh tm;

            public PRoll_Controller Controller;
            public byte PositionIndex;
            public byte PitchIndex;

            void Start()
            {
               // tm = GameObject.Find("DebugText").GetComponent<TextMesh>();
            }

            void Update()
            {

            }

            public void OnTap()
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position, PositionIndex, PitchIndex, Controller);
                PRoll_NoteDrawer.EndNoteDraw(transform.position);
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
                byte duration = PRoll_NoteDrawer.EndNoteDraw(relativePosition);
                //inject note to controller
                Controller.AddNote( PositionIndex, Settings.MidiFromPitchIndex(PitchIndex, Controller.octave), 1, duration);
            }


        }



    }

}