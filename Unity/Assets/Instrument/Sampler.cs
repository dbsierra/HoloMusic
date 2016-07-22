using UnityEngine;
using System.Collections;

public class Sampler : MonoBehaviour {

    AudioSource asource;
    float[] clipData;
    int clipIndex;
    bool ready;
    uint globalSample;
    

    void Start () {
        asource = GetComponent<AudioSource>();
        clipData = new float[asource.clip.samples * asource.clip.channels];
        asource.clip.GetData(clipData, 0);

        ready = true;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (ready)
        {
            for (int i = 0; i < data.Length; i = i + channels)
            {

                //if we are in stereo, duplicate the sample for L+R channels
                data[i] = 0;
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }

                globalSample++;

            }
        }
    }

}
