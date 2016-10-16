using UnityEngine;
using System.Collections;


namespace Sequencer.PianoRoll
{

    public class PRoll_Options : MonoBehaviour
    {

        public static PRoll_Options Instance;

        public GameObject Note;

        void Start()
        {
            Instance = this;
        }

        void Update()
        {
        }


    }
}