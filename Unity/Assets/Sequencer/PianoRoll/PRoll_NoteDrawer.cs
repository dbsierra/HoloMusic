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
        private static byte thresholdRank;
        private static int maxThresholdInc;
        private static float maxScale;

        private static float ScaleOfSquare = 1f; //the relative scale of the PRoll slot square
        private static GameObject Note;

        private static Vector3 startPos;

        private static Transform currentNoteBeingDrawn;

        static PRoll_NoteDrawer()
        {
            Note = PRoll_Options.Instance.Note;

            //TODO: proceduralize this instead of hard-coding the values by finding them yourself
            thresholdValues = new float[] { 1, 3.862931f, 6.713108f, 9.483335f, 12.44833f, 15.3f, 18.15f, 21f, 23.85f, 26.7f, 29.6f, 32.4f, 35.3f, 38.15f, 41f, 43.85f };
            maxScale = thresholdValues[thresholdValues.Length - 1];
        }

        /// <summary>
        /// Begin drawing a note. Initializes variabes.
        /// </summary>
        /// <param name="startPos">Represents the position that is the center of the piano roll slot to draw the note on</param>
        public static void BeginNoteDraw( PRoll_Slot slot )
        {
            float absoluteSizeY = .0035f;

            Vector3 startPos = new Vector3();
            startPos.y = slot.transform.position.y + (ScaleOfSquare / -4) * absoluteSizeY / (ScaleOfSquare / 2);
            startPos.x = slot.transform.position.x - (ScaleOfSquare / -4) * absoluteSizeY / (ScaleOfSquare / 2);

            currentNoteBeingDrawn = ((GameObject)(GameObject.Instantiate(Note))).transform;
            currentNoteBeingDrawn.SetParent( slot.transform, false);
            float nSize = .0035f; //absoulte size in meters of the note geometry
            currentNoteBeingDrawn.transform.localPosition = new Vector3(nSize * .5f, nSize * -.5f, 0);

            PRoll_Note note = currentNoteBeingDrawn.GetComponent<PRoll_Note>();
            note.Init(slot, slot.PitchIndex, slot.PositionIndex);
            slot.NoteGeo = note;

            thresholdRank = 0;

            //TODO: Keep track of earliest placed note in a row, and use that to determine the max scale. If no other notes, than use based off of final threshold value
            int indexOfNextNote = slot.Controller.GetIndexOfNextNote(slot.PositionIndex, slot.PitchIndex);
            
            maxThresholdInc = Mathf.Clamp( indexOfNextNote - slot.PositionIndex - 1, 0, thresholdValues.Length-1 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeHandPosX"> goes -1 to 1 (left to right) </param>
        public static void DrawNote(float relativeHandPosX)
        {
            float scale = Mathf.Clamp( relativeHandPosX * maxScale, 1, thresholdValues[maxThresholdInc]);


            if( thresholdRank < thresholdValues.Length-1 )
            {
                if (scale >= thresholdValues[thresholdRank+1])
                    thresholdRank++;
            }

            if( thresholdRank > 0 )
            {
                if (scale <= thresholdValues[thresholdRank - 1])
                    thresholdRank--;
            }

            currentNoteBeingDrawn.localScale = new Vector3(scale, 1, 1);
        }

        public static void InsertNoteGeo(PRoll_Slot slot)
        {
            if( slot.Note == null )
            {
                Debug.LogError("No note information in this slot to insert note geometry: " + slot.PositionIndex + " " + slot.PitchIndex);
                return;
            }
            BeginNoteDraw(slot);
            thresholdRank = (byte)(slot.Note.duration - 1);
            EndNoteDraw();
        }

        //returns duration of note
        public static byte EndNoteDraw( )
        {
            currentNoteBeingDrawn.localScale = new Vector3( thresholdValues[thresholdRank], 1, 1);
            return (byte)(thresholdRank+1);
        }

    }


}