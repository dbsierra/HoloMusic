using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace Timeline
{
    public class GlobalTime : MonoBehaviour
    {

        public static GlobalTime Instance;
        public bool MetronomeOn;
        public Timeline.Metronome metronome;
        private bool metronomePlay;

        int step;
        int sample;
        int globalSample;
        public int GlobalSample { get { return globalSample; } }
        int maxSteps;
        int maxBars;
        int beatLength_s;

        public delegate void StepAction(int step);
        public static event StepAction OnStep;
        public delegate void PickupAction();
        public static event PickupAction EnterPickup;
        public delegate void RecordingAction();
        public static event RecordingAction EnterRecording;
        public delegate void RecordingExitAction();
        public static event RecordingExitAction ExitRecording;
        public delegate void PauseAction();
        public static event PauseAction OnPause;
        public delegate void StopAction();
        public static event StopAction OnStop;
        public delegate void PlayAction();
        public static event PlayAction OnPlay;

        //recording
        int pickupSteps;
        int recordingSteps;
        public int MaxRecordingSteps { get { return recordingSteps;  } }
        bool resetOnNextStep;
        bool CallExitRecording;

        public enum State
        {
            playing = 0,
            paused = 1,
            pickup = 2,
            recording = 3
        };
        public State state;

        bool ready;

        // Use this for initialization
        void Start()
        {
            Init();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
                Play();
            if (Input.GetKeyDown(KeyCode.X))
                Pause();
            if (Input.GetKeyDown(KeyCode.C))
                Stop();
            if (Input.GetKeyDown(KeyCode.V))
                Record();

            if( CallExitRecording )
            {
                ExitRecording();
                CallExitRecording = false;
            }
        }
        void Init()
        {
            //Init variables
            Instance = this;

            maxBars = 2;
            maxSteps = maxBars * Settings.NoOfStepsInBar;
            beatLength_s = Settings.BeatLength_s;
            ResetPlayhead();

            //recording
            pickupSteps = 16;
            recordingSteps = maxSteps;


            ready = true;
        }
        void ResetPlayhead()
        {
            step = 0;
            sample = beatLength_s;
            globalSample = 0;
        }

        public void Play()
        {
            if(state == State.paused)
            {
                state = State.playing;
                OnPlay();
            }
        }
        public void Pause()
        {
            if (state == State.playing || state == State.recording)
            {
                state = State.paused;
               OnPause();
            }
        }
        public void Stop()
        {
            Pause();
            ResetPlayhead();
            metronome.Reset();
            OnStop();
        }
        public void Record()
        {
            if (state == State.playing)
            {
                Stop();
            }

            metronome.pickup = true;
            state = State.pickup;
        }


        void OnAudioFilterRead(float[] data, int channels)
        {
            Debug.Log(channels);
            if (ready && (state == State.playing || state == State.pickup || state == State.recording) )
            {
                //foreach sample of this block of audio data
                for (int i = 0; i < data.Length; i+=channels )
                {

                    //We have hit a step
                    if (sample >= beatLength_s)
                    {       
                        //-----Step book keeping------//
                        //we are coming from pickup mode and are ready to record        
                        if( resetOnNextStep )
                        {
                            state = State.recording;                         
                            ResetPlayhead();
                            resetOnNextStep = false;
                        }
                        //our recording time has finished, stop playing until ready to playback again
                        if (state == State.recording)
                        {
                            if (step >= recordingSteps)
                            {
                                Stop();
                                CallExitRecording = true;
                                break;
                            }                          
                        }
                        //if step exceeds max, wrap around
                        if (step >= maxSteps)
                        {
                            step = 0;
                            globalSample = 0;
                        }
                        //------------------------//
                        
                        
                        //-----Do step stuff-----//
                        //we are in pickup mode and have reached our last step
                        if (state == State.pickup && step >= pickupSteps-1)
                        {
                            resetOnNextStep = true;
                            metronome.pickup = false;
                        }
                        //play metronome on these steps
                        if ( step % 4 == 0 && (state == State.recording || state == State.pickup) )
                        { 
                            metronome.NextHit();
                        }
                        //Fire step event
                        if (OnStep != null)
                            OnStep(step);
                        //------------------------//

                        //inc and reset
                        step++;
                        sample = 0;
                    }
                    else
                        sample++;

                    if ( state == State.recording || state == State.pickup )
                    {
                       data[i] = metronome.NextSample();     
                    }

                    //if we are in stereo, duplicate the sample for L+R channels
                    if (channels == 2)
                    {
                       data[i + 1] = data[i];
                    }

                    globalSample++;
                }
            }


        }



    }

}