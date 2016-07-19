using UnityEngine;
using System.Collections;

namespace Sequencer
{
    namespace PianoRoll
    {

        public class PRoll_Slot : MonoBehaviour
        {

            // TextMesh tm;

            public int position;
            public int pitch;

            void Start()
            {
               // tm = GameObject.Find("DebugText").GetComponent<TextMesh>();
            }

            void Update()
            {

            }

            public void OnTap()
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position, position, pitch);
                PRoll_NoteDrawer.EndNoteDraw(transform.position);
            }


            public void OnNavigationStarted( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position, position, pitch);
            }
            public void OnNavigationUpdated( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.DrawNote(relativePosition.x);
            }
            public void OnNavigationCompleted( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.EndNoteDraw(relativePosition);
            }


        }



    }

}