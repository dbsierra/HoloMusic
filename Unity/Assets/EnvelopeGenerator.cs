using UnityEngine;
using System.Collections;
using MusicUtilities;

public class EnvelopeGenerator
{
    //--/\__
    public float Attack; //Attack duration
    public float Decay; //Decay duration
    public float Sustain; //Sustain level (0 - 1)
    public float Release; //Release duration

    public enum State
    {
        attack = 0,
        decay = 1,
        sustain = 2,
        release = 3,
        off = 4
    };
    private State state;
    public State CurState{ get { return state; } }


    private float t;    //current time
    private float env;  //Final value to output
    public float Env { get { return env; } }
    

    private string name;

    public EnvelopeGenerator(string n)
    {
        name = n;
    }

    public void GateOpen()
    {
        state = State.attack;

        //Ensure nothing is 0
        Attack = Mathf.Max(Settings.inc, Attack );
        Decay = Mathf.Max(Settings.inc, Decay);
        Sustain = Mathf.Clamp(Sustain, 0, 1);
        Release = Mathf.Max(Settings.inc, Release);

        releaseEnter = attackEnter = decayEnter = false;

        

        t = env = 0;
    }
    public void GateClose()
    {
        state = State.release;
    }



    bool attackEnter;
    bool decayEnter;
    bool releaseEnter;

    float startTime;

    float envSnapshot;

    /// <summary>
    /// Gets called every sample. 
    /// </summary>
    /// <returns></returns>
    public float GetSample()
    {
        if (state == State.attack)
        {
            if(!attackEnter)
            {
                t = 0;
                attackEnter = true;
               // Debug.Log(name + " AttackStart: " + MasterClock.Instance.MyTime + " " + env);
            }
            if (t >= Attack)
            {
                //Debug.Log(name + "AttackEnd: " + MasterClock.Instance.MyTime + " " + env);
                state = State.decay;
                env = 1;
            } 
            else
            {
                env = t/Attack;
            }
                    
        }

        else if (state == State.decay)
        {
            if (!decayEnter)
            {
                t = 0;
                decayEnter = true;
               // Debug.Log(name + " DecayStart: " + MasterClock.Instance.MyTime + " " + env);
            }
            if (env <= Sustain && state != State.release)
            {
               // Debug.Log(name + " DecayEnd: " + MasterClock.Instance.MyTime + " " + env);
                env = Sustain;
                state = State.sustain; 
            }
            else
            {
                //Debug.Log(env);
                env = 1 - ((1-Sustain) * (t / Decay));
            }
        }

        else if (state == State.release)
        {
            if (!releaseEnter)
            {
                t = 0;
                envSnapshot = env;
                releaseEnter = true;
  
               // Debug.Log(name + " ReleaseStart: " + MasterClock.Instance.MyTime + " " + env);
            }
            if (env <= 0 || t >= Release)
            {
               // Debug.Log(name + " ReleaseEnd: " + MasterClock.Instance.MyTime + " " + env);
                state = State.off;
                env = 0;
            }
            else
            {
                env -= Settings.inc *  envSnapshot * (1/Release) ;
                
            }
                
        }

        t += Settings.inc;


        return env;
    }
}