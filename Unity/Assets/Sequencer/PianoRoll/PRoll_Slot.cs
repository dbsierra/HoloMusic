using UnityEngine;
using System.Collections;

namespace Sequencer
{
    namespace PianoRoll
    {

        public class PRoll_Slot : MonoBehaviour
        {

            // Use this for initialization
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void OnTap()
            {

                //PRoll_NoteDrawer.BeginNoteDraw(transform.position);

            }

            public void OnHoldStart()
            {
                PRoll_NoteDrawer.BeginNoteDraw(transform.position);
                Debug.Log("HI");
            }
        }


    }

}