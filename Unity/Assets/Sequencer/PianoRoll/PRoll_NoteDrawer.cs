using UnityEngine;
using System.Collections;

namespace Sequencer
{
    namespace PianoRoll
    {

        /// <summary>
        /// 
        /// PRoll slot is .01 x .01 meters
        /// The pivot of the note cube is at the bottom of a cube. We want to center this cube within the square.
        /// .0025 meters to get 1/4 down the square from the middle of the square (when it is .01 x .01 meters). 
        /// This is the value we want if the cube has scale y = .5 (half the size of square)
        /// 
        /// The y position of the note is a function of its scale in Y, and the size of the square.
        /// 
        /// f(scaleY) = (ScaleOfSquare/-4) * (AbsoluteSizeY)/(ScaleOfSquare/2)
        /// 
        /// </summary>

        public static class PRoll_NoteDrawer
        {

            private static float ScaleOfSquare = 1f; //the relative scale of the PRoll slot square
            private static GameObject Note;

            private static Vector3 startPos;

            static PRoll_NoteDrawer()
            {
                Note = PRoll_Options.Instance.Note;
            }

            /// <summary>
            /// Begin drawing a note. Initializes variabes.
            /// </summary>
            /// <param name="startPos">Represents the position that is the center of the piano roll slot to draw the note on</param>
            public static void BeginNoteDraw( Vector3 startPos )
            {
                float absoluteSizeY = .0035f;
                startPos.y = startPos.y + (ScaleOfSquare / -4) * absoluteSizeY / (ScaleOfSquare / 2);
                GameObject.Instantiate(Note, startPos, Quaternion.identity);
                startPos = Input.mousePosition;
            }

            public static void DrawNote()
            {

                float delta = Vector3.Distance( startPos, Input.mousePosition );
                Debug.Log(delta);
            }

        }


    }
}