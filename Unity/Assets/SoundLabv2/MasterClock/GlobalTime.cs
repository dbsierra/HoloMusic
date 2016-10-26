using UnityEngine;
using System.Collections;
using MusicUtilities;

namespace Timeline
{
    public class GlobalTime : MonoBehaviour
    {
        private uint masterSample;
        public uint MasterSample { get { return masterSample; } }
        private float masterTime;
        public float MasterTime { get { return masterTime; } }

        public static GlobalTime Instance;
        public bool MetronomeOn;
        public Timeline.Metronome metronome;
        private bool metronomePlay;
        public bool RecordWithPickup;

        int step;
        int sample;
        int globalSample;
        int syncSample;
        public int SyncSample { get { return syncSample; } }
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
        int recordingSteps_s;
        int recordTimer_s;
        int stepRecord;
        public int MaxRecordingSteps { get { return recordingSteps;  } }
        bool resetOnNextStep;

        public enum State
        {
            paused = 0,
            stopped = 1,
            playing = 2,
            enterPickup = 3,
            pickup = 4,
            recording = 5
        };
        public State state;




        #region Initializers
        //------------------------------------------------------------------------------//
        void Start()
        {
            Init();
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
            recordingSteps_s = recordingSteps * Settings.BeatLength_s;
            Debug.Log(recordingSteps_s);
        }
        //------------------------------------------------------------------------------//
        #endregion


        #region Transport functions
        //------------------------------------------------------------------------------//
        void ResetPlayhead()
        {
            step = 0;
            sample = beatLength_s;
            globalSample = 0;
        }
        public void Play()
        {
            if(state == State.paused || state == State.stopped )
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
            state = State.stopped;
            ResetPlayhead();
            metronome.Reset();
            OnStop();
        }
        public void Record()
        {
            if ( RecordWithPickup )
            {
                state = State.enterPickup;
                metronome.pickup = true;
            }
            else
            {
                state = State.recording;
                OnPlay();
                metronome.pickup = false;
                EnterRecording();
            }   
        }
        public void RecordStop()
        {
            Stop();
            ExitRecording();
        }
        //------------------------------------------------------------------------------//
        #endregion

        bool startTiming;
        int sampleTimer;
        float startTime;

        #region Execution logic
        //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
        void OnAudioFilterRead(float[] data, int channels)
        {
            if (state == State.playing || state == State.pickup || state == State.enterPickup || state == State.recording)
            {
                //we are coming from pickup mode and are ready to record        
                if (resetOnNextStep)
                {
                    resetOnNextStep = false;
                    recordTimer_s = 0;
                    state = State.recording;
                    metronome.pickup = false;
                    EnterRecording();
                    OnPlay();
                    startTiming = false;

                }

                syncSample = globalSample;

                //foreach sample of this block of audio data
                for (int i = 0; i < data.Length; i += channels)
                {
                    if (startTiming)
                        sampleTimer++;

                    masterSample++;
                    // masterTime = (float)masterSample / (float)Settings.SampleRate;

                    //our recording time has finished, stop playing until ready to playback again
                    if (state == State.recording)
                    {
                        if (recordTimer_s >= recordingSteps_s)
                        {
                            Debug.Log(recordTimer_s);
                            recordTimer_s = 0;
                            RecordStop();
                            break;
                        }
                    }


                    //  [....STEP....]  //
                    if (sample >= beatLength_s)
                    {
                        //-----Step book keeping------//

                        //if step exceeds max, wrap around
                        if (step >= maxSteps)
                        {
                            step = 0;
                            globalSample = 0;
                        }
                        //------------------------//

                        //-----Do step stuff-----//
                        //we are in pickup mode and have reached our last step
                        if (state == State.pickup && step >= pickupSteps)
                        {

                            resetOnNextStep = true;
                            ResetPlayhead();
                            metronome.Reset();
                            break;
                        }

                        //play metronome on these steps
                        if (step % 4 == 0)
                        {
                            if( state == State.enterPickup)
                            {
                                EnterPickup();
                                state = State.pickup;
                                Debug.Log("pickup now");
                            }

                            metronome.NextHit();
                        }

                        //Fire step event
                        if (OnStep != null)
                            OnStep(step);
                        //------------------------//

                        //inc and reset
                        step++;
                        if (state == State.recording)
                            stepRecord++;
                        sample = 0;
                    }
                    else
                        sample++;


                    if (state == State.recording)
                        recordTimer_s++;

                    if ( state == State.pickup || state == State.recording || MetronomeOn )
                        data[i] = metronome.NextSample();
                    

                    //if we are in stereo, duplicate the sample for L+R channels
                    if (channels == 2)
                    {
                        data[i + 1] = data[i];
                    }

                    globalSample++;
                }
            }
            else
            {
                for (int i = 0; i < data.Length; i += channels)
                {
                    masterSample++;
                   // masterTime = (float)masterSample / (float)Settings.SampleRate;
                }
            }
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


        }
        //-------------------------,.-'`'-.,.-'`'-.,.-'`'-.,----------------------------//
        #endregion


    }

}