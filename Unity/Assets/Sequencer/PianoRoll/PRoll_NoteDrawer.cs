using UnityEngine;
using System.Collections;

namespace Sequencer.PianoRoll
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

        //the xscale that the note geo should be set to for each note of the 16 note bar
        private static float[] thresholdValues;
        private static int thresholdRank;
        private static float maxScale;

        private static float ScaleOfSquare = 1f; //the relative scale of the PRoll slot square
        private static GameObject Note;

        private static Vector3 startPos;

        private static Transform currentNoteBeingDrawn;

        

        static PRoll_NoteDrawer()
        {
            Note = PRoll_Options.Instance.Note;

            thresholdValues = new float[] { 1, 3.862931f, 6.713108f, 9.483335f, 12.44833f, 15.3f, 18.15f, 21f, 23.85f, 26.7f, 29.6f, 32.4f, 35.3f, 38.15f, 41f, 43.85f };
            maxScale = thresholdValues[thresholdValues.Length - 1];
        }

        /// <summary>
        /// Begin drawing a note. Initializes variabes.
        /// </summary>
        /// <param name="startPos">Represents the position that is the center of the piano roll slot to draw the note on</param>
        public static void BeginNoteDraw( Vector3 startPos )
        {
            float absoluteSizeY = .0035f;
            startPos.y = startPos.y + (ScaleOfSquare / -4) * absoluteSizeY / (ScaleOfSquare / 2);
            startPos.x = startPos.x - (ScaleOfSquare / -4) * absoluteSizeY / (ScaleOfSquare / 2);
            currentNoteBeingDrawn = ((GameObject)(GameObject.Instantiate(Note, startPos, Quaternion.identity))).transform;
            startPos = Input.mousePosition;
            thresholdRank = 0;
        }

        public static void DrawNote(float relativeScale)
        {
            float scale = Mathf.Clamp( relativeScale * maxScale, 1, maxScale );

            currentNoteBeingDrawn.localScale = new Vector3(scale, 1, 1);

            if( thresholdRank < thresholdValues.Length-1 )
            {
                if (scale > thresholdValues[thresholdRank+1])
                    thresholdRank++;
            }

        }

        public static void EndNoteDraw( Vector3 endPos )
        {
            currentNoteBeingDrawn.localScale = new Vector3( thresholdValues[thresholdRank], 1, 1);
        }

    }


}