using UnityEngine;
using System.Collections;

namespace Sequencer
{
    namespace PianoRoll
    {

        public class PRoll_Slot : MonoBehaviour
        {

           // TextMesh tm;

            void Start()
            {
               // tm = GameObject.Find("DebugText").GetComponent<TextMesh>();
            }

            void Update()
            {

            }

            public void OnTap()
            {

                //PRoll_NoteDrawer.BeginNoteDraw(transform.position);
                PRoll_NoteDrawer.BeginNoteDraw(transform.position);
            }

            public void OnHoldStart()
            {
               
                Debug.Log("HI");
               // PRoll_NoteDrawer.BeginNoteDraw(transform.position);
            }


            public void OnNavigationStarted( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position);
            }

            public void OnNavigationUpdated( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.DrawNote(relativePosition.x);
                //tm.text = relativePosition.x + " " + relativePosition.y + " " + relativePosition.z;
            }
            public void OnNavigationCompleted( Vector3 relativePosition )
            {
                PRoll_NoteDrawer.EndNoteDraw(relativePosition);
            }
        }



    }

}